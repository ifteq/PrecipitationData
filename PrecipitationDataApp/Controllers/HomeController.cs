using PrecipitationDataApp.Helpers;
using PrecipitationDataApp.Models.Home;
using PrecipitationDataAppModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrecipitationDataApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FileUpload(HomeViewModel vm)
        {
            if (vm.UploadedFile != null) {

                StreamReader streamreader = new StreamReader(vm.UploadedFile.InputStream);

                //Would like to use data table
                //var dataTable = new DataTable();

                var lineNumber = 1;
                var precipitationData = new List<PrecipitationData>();
                var startYear = 0;
                var endYear = 0;

                var dbname = DbHelper.CreateDbTable();

                while (!streamreader.EndOfStream)
                {
                    var line = streamreader.ReadLine();
                    if (lineNumber == 5 && !line.Contains("[Years="))
                    {
                        return Json(new { error = "FileFormatIncorrect", code = 409 });
                    }

                    if (lineNumber == 5) { //Check if the Years value is set in the top headers of the doc

                        var editLine = line.Replace("[Years=", "^");
                        var years = editLine.Substring((editLine.IndexOf('^')+1), 9).Split('-');
                        startYear = Convert.ToInt32(years[0]);
                        endYear = Convert.ToInt32(years[1]);
                    }

                    if (lineNumber > 5) { //Data to be saved is from line 6 onwards

                        var gridRefXY = DataReaderHelper.GetGridRef(line);

                        //gridRefXY is null then we are on the line with monthly values
                        if (precipitationData.Count > 0 && gridRefXY == null)
                        {
                            var pdLast = precipitationData.Last();
                            var pdLastDate = pdLast.Date.HasValue ? pdLast.Date : null;
                            if (pdLastDate == null)
                            {
                                var precipData = DataReaderHelper.GetPrecipitationData(pdLast.GridRefXY, line, startYear);
                                precipitationData.Remove(pdLast);
                                precipitationData.AddRange(precipData);
                                foreach (var p in precipData)
                                {
                                    DbHelper.InsertIntoDb(p, dbname);
                                }                               
                            }
                            else { 
                                var precipData = DataReaderHelper.GetPrecipitationData(pdLast.GridRefXY, line, (pdLast.Date.Value.Year+1));
                                precipitationData.AddRange(precipData);
                                foreach (var p in precipData)
                                {
                                    DbHelper.InsertIntoDb(p, dbname);
                                }
                            }
                        }
                        else
                        { //First line with grid ref values
                            precipitationData.Add(new PrecipitationData { GridRefXY = gridRefXY });
                        }                    
                    }

                    lineNumber++;
                }

                DbHelper.CloseConnection();

                return Json(new { error = "Success", code = 200, value = dbname });
            }

            return new JsonResult();
        }
    }
}
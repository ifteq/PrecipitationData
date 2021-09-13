using PrecipitationDataAppModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrecipitationDataApp.Helpers
{
    public static class DataReaderHelper
    {
        public static GridRef GetGridRef(string gridLine) {

            var gridRef = new GridRef();

            if (gridLine.Contains("Grid-ref="))
            {

                var xyref = gridLine.Replace("Grid-ref=", "").Replace(" ", "");
                var xysplit = xyref.Split(',');
                gridRef.Xref = Convert.ToInt32(xysplit[0]);
                gridRef.Yref = Convert.ToInt32(xysplit[1]);

            }
            else { return null; }

            return gridRef;
        }

        public static List<PrecipitationData> GetPrecipitationData(GridRef gridRef, string gridLine, int year)
        {
            var precipitationData = new List<PrecipitationData>();
            var dataSplit = gridLine.Split(' ');
            var month = 1;

            foreach (var value in dataSplit)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var pd = new PrecipitationData { GridRefXY = gridRef };
                    pd.Value = Convert.ToInt32(value);
                    pd.Date = new DateTime(year, month, 1);
                    precipitationData.Add(pd);
                    month++;
                }
            }
         
            return precipitationData;
        }
    }
}
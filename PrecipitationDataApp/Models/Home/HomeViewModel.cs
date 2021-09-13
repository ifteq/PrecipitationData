using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrecipitationDataApp.Models.Home
{
    public class HomeViewModel
    {
        public HttpPostedFileBase UploadedFile { get; set; }

        public string Message { get; set; }
    }
}
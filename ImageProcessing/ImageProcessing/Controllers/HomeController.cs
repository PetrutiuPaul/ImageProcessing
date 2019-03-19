using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace ImageProcessing.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            if (Directory.Exists(Server.MapPath("~/UploadedFiles")))
            {
                foreach (var file in Directory.GetFiles(Server.MapPath("~/UploadedFiles")))
                {
                    if (file.Contains("current"))
                    {
                        ViewBag.Current = Path.GetFileName(file);
                    }
                    else
                    {

                        ViewBag.Result = Path.GetFileName(file);
                    }
                }
            }

            return View();
        }

        public ActionResult UploadFiles(HttpPostedFileBase file)
        {
            if (!ModelState.IsValid) return View("Index");
            try
            {
                if (file != null)
                {
                    if (Directory.Exists(Server.MapPath("~/UploadedFiles")))
                    {
                        Directory.Delete(Server.MapPath("~/UploadedFiles"),true);
                    }

                    Directory.CreateDirectory(Server.MapPath("~/UploadedFiles"));
                    var path = Path.Combine(Server.MapPath("~/UploadedFiles"),
                        $"current{Path.GetExtension(file.FileName)}");
                    file.SaveAs(path);
                }
                ViewBag.FileStatus = "File uploaded successfully.";
            }
            catch (Exception e)
            {
                ViewBag.FileStatus = "Error while file uploading."; ;
            }
            return View("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
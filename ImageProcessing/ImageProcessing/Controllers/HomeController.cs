using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace ImageProcessing.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (!Directory.Exists(Server.MapPath("~/UploadedFiles"))) return View();
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

            return View();
        }

        public ActionResult UploadFiles(HttpPostedFileBase file)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index");
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
                        $"current-{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}");
                    file.SaveAs(path);
                }
                ViewBag.FileStatus = "File uploaded successfully.";
            }
            catch (Exception e)
            {
                ViewBag.FileStatus = "Error while file uploading."; ;
            }
            return RedirectToAction("Index");
        }

        public ActionResult ImageToBlack()
        {
            foreach (var file in Directory.GetFiles(Server.MapPath("~/UploadedFiles")))
            {
                if (!file.Contains("current")) continue;
                using (var bitmap = new Bitmap(file))
                {
                    for (var i = 0; i < bitmap.Width; i++)
                    {
                        for (var j = 0; j < bitmap.Height; j++)
                        {
                            var pixel = bitmap.GetPixel(i, j);
                            var greyColor = (pixel.B + pixel.G + pixel.B) / 3;

                            bitmap.SetPixel(i,j, Color.FromArgb(255,greyColor,greyColor,greyColor) );
                        }
                    }
                    bitmap.Save(Server.MapPath($"~/UploadedFiles/result-{DateTime.Now.Ticks}" + Path.GetExtension(file)));
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Move()
        {
            string current = string.Empty;
            string result = string.Empty;
            foreach (var file in Directory.GetFiles(Server.MapPath("~/UploadedFiles")))
            {


                if (file.Contains("result"))
                {
                    result = file;
                }
                else
                {
                    if (file.Contains("current"))
                    {
                        current = file;
                    }
                }
            }

            var source = Server.MapPath("~/UploadedFiles/" + Path.GetFileName(result));
            var dest = Server.MapPath("~/UploadedFiles/current-" + DateTime.Now.Ticks + Path.GetExtension(result));

            System.IO.File.Copy(source, dest);
            System.IO.File.Delete(Server.MapPath("~/UploadedFiles/"+Path.GetFileName(current)));


            return RedirectToAction("Index");
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
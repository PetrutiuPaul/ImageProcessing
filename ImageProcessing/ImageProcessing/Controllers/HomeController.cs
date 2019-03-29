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

        public ActionResult ImageMediation()
        {
            foreach (var file in Directory.GetFiles(Server.MapPath("~/UploadedFiles")))
            {
                if (!file.Contains("current")) continue;
                using (var bitmap = new Bitmap(file))
                {
                    using (var bitmap2 = new Bitmap(file))
                    {
                        
                    for (var i = 1; i < bitmap.Width -1; i++)
                    {
                        for (var j = 1; j < bitmap.Height-1; j++)
                        {
                            bitmap2.SetPixel(i, j, MidColor(bitmap,i,j));
                        }
                    }
                    bitmap2.Save(Server.MapPath($"~/UploadedFiles/result-{DateTime.Now.Ticks}" + Path.GetExtension(file)));
                    }
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult FilterUp()
        {
            foreach (var file in Directory.GetFiles(Server.MapPath("~/UploadedFiles")))
            {
                if (!file.Contains("current")) continue;
                using (var bitmap = new Bitmap(file))
                {
                    using (var bitmap2 = new Bitmap(file))
                    {
                        
                    for (var i = 1; i < bitmap.Width - 1; i++)
                    {
                        for (var j = 1; j < bitmap.Height - 1; j++)
                        {
                            bitmap2.SetPixel(i, j, UpColor(bitmap, i, j));
                        }
                    }
                    bitmap2.Save(Server.MapPath($"~/UploadedFiles/result-{DateTime.Now.Ticks}" + Path.GetExtension(file)));
                    }
                }
            }

            return RedirectToAction("Index");
        }


        public ActionResult BiomedFilter()
        {
            foreach (var file in Directory.GetFiles(Server.MapPath("~/UploadedFiles")))
            {
                if (!file.Contains("current")) continue;
                using (var bitmap = new Bitmap(file))
                {
                    using (var bitmap2 = new Bitmap(file))
                    {        
                        for (var i = 1; i < bitmap.Width - 1; i++)
                        {
                            for (var j = 1; j < bitmap.Height - 1; j++)
                            {
                                bitmap2.SetPixel(i, j, BiomedColor(bitmap, i, j));
                            }
                        }
                        bitmap2.Save(Server.MapPath($"~/UploadedFiles/result-{DateTime.Now.Ticks}" + Path.GetExtension(file)));
                    }
                }
            }

            return RedirectToAction("Index");
        }

        private static Color MidColor(Bitmap bitmap, int i, int j)
        {
            var r = 0;
            var g = 0;
            var b = 0;

            for (var ii = -1; ii <= 1; ii++)
            {
                for (var jj = -1; jj <= 1; jj++)
                {
                    r += bitmap.GetPixel(i + ii, j + jj).R;
                    g += bitmap.GetPixel(i + ii, j + jj).G;
                    b += bitmap.GetPixel(i + ii, j + jj).B;
                }
            }

            return Color.FromArgb(r / 9, g / 9, b / 9);
        }

        private static Color UpColor(Bitmap bitmap, int i, int j)
        {
            var pixel = bitmap.GetPixel(i, j);
            var r = 0;
            var g = 0;
            var b = 0;

            for (var ii = -1; ii <= 1; ii++)
            {
                for (var jj = -1; jj <= 1; jj++)
                {
                    r += bitmap.GetPixel(i + ii, j + jj).R;
                    g += bitmap.GetPixel(i + ii, j + jj).G;
                    b += bitmap.GetPixel(i + ii, j + jj).B;
                }
            }

            return Color.FromArgb(pixel.R - (r / 9) < 0 ? 0 : pixel.R - (r / 9) > 255 ? 255 : pixel.R - (r / 9),
                                pixel.G - (g / 9) < 0 ? 0 : pixel.G - (g / 9) > 255 ? 255 : pixel.G - (g / 9),
                                pixel.B - (b / 9) < 0 ? 0 : pixel.B - (b / 9) > 255 ? 255 : pixel.B - (b / 9));
        }

        private static Color BiomedColor(Bitmap bitmap, int i, int j)
        {
            var pixel = bitmap.GetPixel(i, j);
            var r = 0;
            var g = 0;
            var b = 0;

            for (var ii = -1; ii <= 1; ii++)
            {
                for (var jj = -1; jj <= 1; jj++)
                {
                    r += bitmap.GetPixel(i + ii, j + jj).R;
                    g += bitmap.GetPixel(i + ii, j + jj).G;
                    b += bitmap.GetPixel(i + ii, j + jj).B;
                }
            }


            return Color.FromArgb(-r + 10 * pixel.R < 0 ? 0 : -r + 10 * pixel.R > 255 ? 255 : -r + 10 * pixel.R,
                -g + 10 * pixel.G < 0 ? 0 : -g + 10 * pixel.G > 255 ? 255 : -g + 10 * pixel.G,
                -b + 10 * pixel.B < 0 ? 0 : -b + 10 * pixel.B > 255 ? 255 : -b + 10 * pixel.B);
            
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

        public ActionResult Negative()
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

                            bitmap.SetPixel(i, j, Color.FromArgb(255, 255 - pixel.R, 255 - pixel.G, 255 - pixel.B));
                        }
                    }
                    bitmap.Save(Server.MapPath($"~/UploadedFiles/result-{DateTime.Now.Ticks}" + Path.GetExtension(file)));
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Contrast()
        {
            var contrast = Math.Pow((100.0 + 30) / 100.0, 2);
            
            foreach (var file in Directory.GetFiles(Server.MapPath("~/UploadedFiles")))
            {
                if (!file.Contains("current")) continue;
                using (var bitmap = new Bitmap(file))
                {
                    for (var i = 0; i < bitmap.Width; i++)
                    {
                        for (var j = 0; j < bitmap.Height; j++)
                        {
                            var oldColor = bitmap.GetPixel(i, j);
                            var red = ((((oldColor.R / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                            var green = ((((oldColor.G / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                            var blue = ((((oldColor.B / 255.0) - 0.5) * contrast) + 0.5) * 255.0;
                            if (red > 255) red = 255;
                            if (red < 0) red = 0;
                            if (green > 255) green = 255;
                            if (green < 0) green = 0;
                            if (blue > 255) blue = 255;
                            if (blue < 0) blue = 0;

                            var newColor = Color.FromArgb(oldColor.A, (int)red, (int)green, (int)blue);
                            bitmap.SetPixel(i,j,newColor);
                        }
                    }
                    bitmap.Save(Server.MapPath($"~/UploadedFiles/result-{DateTime.Now.Ticks}" + Path.GetExtension(file)));
                }
            }

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
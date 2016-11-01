using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Repairmen.Helpers;
using System.Configuration;

namespace Repairmen.Controllers.Helpers
{
    public class ImageHelper
    {
        private int confLent { get; set; }

        private Guid file { get; set; }

        private DateTime date { get; set; }

        private string fileName { get; set; }

        public static CustomConfig Config { get; internal set; }

        public ImageHelper(Guid adID, DateTime date)
        {
            string f = adID.ToString() + ".txt";
            fileName = getFolder(date) + f;           
        }

        // Get adequate folder (create, if not exists)
        private string getFolder(DateTime dt)
        {
            string Year = dt.Year.ToString();
            string Month = dt.Month.ToString();

            Config = ConfigurationManager.GetSection("customSection") as CustomConfig;

            string root = Config.ImgRoot;
            confLent = root.Length;
            string dirPath = root + Year + "\\" + Month + "\\";
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            return dirPath;
        }

        // Save Image
        public string SaveImage(string base64Img)
        {
            try
            {
                File.WriteAllText(fileName, base64Img);
                string fileWithoutRoot = fileName.Substring(confLent);
                return fileWithoutRoot;
            }
            catch
            {
                return "";
            }
        }

        //Delete image
        public void DeleteImage()
        {
            try
            {
                File.Delete(fileName);
            }
            catch
            {

            }
        }

        // Read Image
        public string ReadImage(string path)
        {
            string base64text = File.ReadAllText(path);
            return base64text;
        }
    }
}
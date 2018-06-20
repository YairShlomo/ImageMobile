using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
//using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
//using ImageService.Modal;
using ImageService.Logging;
using System.Configuration;
namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal
    {
        #region Members
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size
        private static Regex r = new Regex(":"); //we init this once so that if the function is repeatedly called
                                                 //it isn't stressing the garbage man
        string returnVal;
        string finalTargetPath;
        #endregion
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageServiceModal"/> class.
        /// </summary>
        public ImageServiceModal()
        {
            //m_OutputFolder = ConfigurationManager.AppSettings["OutputDir"];
            string dummy = ConfigurationManager.AppSettings["OutputDir"];
            string dirName = AppDomain.CurrentDomain.BaseDirectory;
            FileInfo fileInfo = new FileInfo(dirName);
            DirectoryInfo parentDir = fileInfo.Directory.Parent.Parent.Parent.Parent;
            string parentDirName = parentDir.FullName;
            m_OutputFolder = parentDirName + dummy;

            try
            {
                m_thumbnailSize = Int32.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }
            // 
        }
        /// <summary>
        /// Gets or sets the output folder.
        /// </summary>
        /// <value>
        /// The output folder.
        /// </value>
        public string OutputFolder
        {
            get { return m_OutputFolder; }
            set { m_OutputFolder = value; }
        }
        /// <summary>
        /// Gets or sets the size of the thumbnail.
        /// </summary>
        /// <value>
        /// The size of the thumbnail.
        /// </value>
        public int ThumbnailSize
        {
            get { return m_thumbnailSize; }
            set { m_thumbnailSize = value; }
        }


        /// <summary>
        /// The Function Addes A file to the system
        /// </summary>
        /// <param name="path">The Path of the Image from the file</param>
        /// <param name="result"></param>
        /// <returns>
        /// Indication if the Addition Was Successful
        /// </returns>
        public string AddFile(string path, out bool result)
        {
            FileAttributes attr = File.GetAttributes(path);
            result = true;
            try
            {
                string strResult;
                if (!((attr & FileAttributes.Directory) == FileAttributes.Directory))
                {
                   // Debug_program debug = new Debug_program();
                    //  debug.write("addfile");
                   // debug.write("addfile");
                    //create output directory if doesnt exist
                    Directory.CreateDirectory(OutputFolder);
                    File.SetAttributes(OutputFolder, FileAttributes.Hidden);
                    string fullNamePath = Path.GetFileName(path);
                    string thumbnailPath = OutputFolder + "\\Thumbnails";
                    Directory.CreateDirectory(thumbnailPath);
                    // DateTime creation = File.GetCreationTime(path);
                    DateTime creation;

                    try
                    {
                        creation = GetDateTakenFromImage(path);

                    }
                    catch (Exception e)
                    {
                        creation = File.GetCreationTime(path);
                        //debug.write("exception" + e.Message+path);

                    }
                    string yearOfCreation = creation.Year.ToString();
                    string monthOfCreation = creation.Month.ToString();
                    Directory.CreateDirectory(OutputFolder + "\\" + yearOfCreation);
                    Directory.CreateDirectory(thumbnailPath + "\\" + yearOfCreation);
                    //Create the directory for the monthOfCreation
                    string targetPathDir = OutputFolder + "\\" + yearOfCreation + "\\" + monthOfCreation;
                    DirectoryInfo dir = Directory.CreateDirectory(targetPathDir);
                    string targetPath = targetPathDir + "\\" + fullNamePath;
                    string targetPathThumbnail = thumbnailPath + "\\" + yearOfCreation + "\\" + monthOfCreation;
                    DirectoryInfo dirThumbnail = Directory.CreateDirectory(thumbnailPath + "\\" + yearOfCreation + "\\" + monthOfCreation);
                    string pathExtension = Path.GetExtension(targetPath);
                    targetPath = IsFileExist(targetPath, pathExtension);
                    File.Move(path, targetPath);
                    finalTargetPath = targetPath.ToString();

                    Image thumbImage = Image.FromFile(targetPath);
                    thumbImage = thumbImage.GetThumbnailImage(m_thumbnailSize, m_thumbnailSize, () => false, IntPtr.Zero);
                    string finalTargetPathThumb = IsFileExist(targetPathThumbnail.ToString() + "\\" + fullNamePath, pathExtension);
                    thumbImage.Save(finalTargetPathThumb);
                    result = true;
                    returnVal = finalTargetPathThumb + ';' + finalTargetPath;
                    return returnVal;
                }
                else
                {

                    strResult = "File didn't added-wrong Image path ";

                }


                return strResult;
            }
            catch (Exception e)
            {
                returnVal = finalTargetPath + ';' ;

                result = false;
                //return e.ToString()+"njnjnj";
                return returnVal;
            }

        }

        /// <summary>
        /// Determines whether [is file exist] [the specified target path].
        /// </summary>
        /// <param name="targetPath">The target path.</param>
        /// <param name="pathExtension">The path extension.</param>
        /// <returns></returns>
        public string IsFileExist(string targetPath, string pathExtension)
        {
           // Debug_program debug = new Debug_program();
            int counter = 1;
            while (File.Exists(targetPath))
            {
                //  debug.write("inside "+targetPath);
                string noExtesnsion = targetPath.Replace(pathExtension, "");
                int numericValue;
                if (Int32.TryParse(noExtesnsion.Substring(noExtesnsion.Length - 1), out numericValue))
                {
                    noExtesnsion = noExtesnsion.Substring(0, noExtesnsion.Length - 1);
                }
                targetPath = noExtesnsion + counter + pathExtension;
                counter++;
            }

            // debug.write("after "+targetPath);

            return targetPath;
        }
        //retrieves the datetime WITHOUT loading the whole image
        public static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                System.Drawing.Imaging.PropertyItem propItem = myImage.GetPropertyItem(36867);
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                return DateTime.Parse(dateTaken);
            }
        }

        public string DeleteFile(string path, out bool result)
        {
            try
            {
                File.Delete(path);
                result = true;
                return "removed photo" + path + "successfully";

            }
            catch (Exception e)
            {
                result = false;
                return "failed remove photo" + path;
            }
            
        }
    }
}
    


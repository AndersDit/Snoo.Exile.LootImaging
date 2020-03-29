using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snoo.Exile.LootImaging
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\Users\Anders Ditlevsen\Desktop\LootImages";

            // Get Directory
            DirectoryInfo directory = GetDirectory(filePath);

            // Get all images, jpg, jpeg, png
            List<FileInfo> imageFiles = GetImageFiles(directory, SearchOption.TopDirectoryOnly, new string[] { "jpg", "jpeg", "png" });


            // Load as Images
            List<Image> images = GetImages(imageFiles);

            // Calculate new image size
            int xx = 0; // new max length of loot image
            int yy = 0; // new max height of loot image

            foreach(Image image in images)
            {
                xx += image.Width;

                if(yy <= image.Height)
                {
                    yy = image.Height;
                }
            }

            int xxx = images.Sum(x => x.Width);
            int yyy = images.OrderByDescending(x => x.Height).FirstOrDefault().Height;

            // Put images together
            Bitmap finalLootImage = new Bitmap(xx, yy);
            Graphics g = Graphics.FromImage(finalLootImage);
            g.Clear(SystemColors.AppWorkspace);

            int totalWidth = 0;
            
            foreach(Image image in images)
            {
                totalWidth += image.Width;
                g.DrawImage(image, new Point(totalWidth, 0));
            }

            // Save image to disk
            finalLootImage.Save($"{filePath}\\FinalLootImage.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            finalLootImage.Dispose();
        }

        public static List<Image> GetImages(List<FileInfo> imageFiles)
        {
            List<Image> images = new List<Image>();

            foreach(FileInfo imageFile in imageFiles)
            {
                try
                {
                    Image image = Image.FromFile(imageFile.FullName);

                    if (image != null)
                    {
                        images.Add(image);
                    }
                }
                catch (Exception ex)
                {
                    Log($"Error while loading imagefile to image. Filename was {imageFile.Name}", ex);
                }
            }

            return images;
        }

        public static List<FileInfo> GetImageFiles(DirectoryInfo directory, SearchOption so, string[] extensionWhitelist)
        {
            List<FileInfo> imageFiles = new List<FileInfo>();

            try
            {
                foreach (FileInfo imageFile in directory.GetFiles("*.*", so)?.Where(x => extensionWhitelist.Contains(x.Extension.ToLower())))
                {
                    imageFiles.Add(imageFile);
                }
            }
            catch (Exception ex)
            {
                Log("Error whilst fetching image files.", ex);
            }

            return imageFiles;
        }

        public static DirectoryInfo GetDirectory(string filePath)
        {
            DirectoryInfo di = null;

            try
            {
                di = GetDirectory(filePath);
            }
            catch (Exception ex)
            {
                Log("Error whilst trying to fetch directory", ex);
            }

            return di;
        }

        public static void Log(string message)
        {
            Console.WriteLine(message);
        }

        public static void Log(string message, Exception exception)
        {
            Console.WriteLine(message);
            Console.WriteLine(exception.ToString());
        }
    }
}

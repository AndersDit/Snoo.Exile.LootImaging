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
            if(args != null && args.Length > 0)
            {
                if(args[0].ToString().ToLower() == "help")
                {
                    Log("<directory path> | Please provide filepath to the directory in which loot images to combine is found.");
                    Log("Supported format: .jpeg, .jpg, .png");
                }
                else
                {
                    string filePath = args[0];

                    if (!string.IsNullOrWhiteSpace(filePath))
                    {
                        bool directoryExists = Directory.Exists(filePath);

                        if (directoryExists)
                        {
                            CombineLootImages(filePath);
                        }
                        else
                        {
                            Log("Directory not found.");
                        }
                    }
                    else
                    {
                        Log("Arguments missing");
                    }
                }
            }
            else
            {
                Log("Arguments missing");
            }
        }

        public static void CombineLootImages(string filePath)
        {

            // Get Directory
            DirectoryInfo directory = GetDirectory(filePath);

            // Get all images, jpg, jpeg, png
            List<FileInfo> imageFiles = GetImageFiles(directory, SearchOption.TopDirectoryOnly, new string[] { ".jpg", ".jpeg", ".png" });

            // Load as Images
            List<Image> images = GetImages(imageFiles);

            // Put images together
            Bitmap finalLootImage = DrawImages(images);

            // Save image to disk
            SaveImages(finalLootImage, $"{filePath}\\FinalLootImage.jpg");
        }

        public static Bitmap DrawImages(List<Image> images)
        {
            int xx = images.Sum(x => x.Width); // new max length of loot image
            int yy = images.OrderByDescending(x => x.Height).FirstOrDefault().Height; // new max height of loot image

            Bitmap finalLootImage = new Bitmap(xx, yy);
            Graphics g = Graphics.FromImage(finalLootImage);
            g.Clear(SystemColors.AppWorkspace);

            int totalWidth = 0;

            foreach (Image image in images)
            {
                g.DrawImage(image, new Point(totalWidth, 0));
                totalWidth += image.Width;
            }

            return finalLootImage;
        }

        public static void SaveImages(Bitmap finalLootImage, string finalFilePath)
        {
            // Save image to disk
            finalLootImage.Save(finalFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
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
                di = new DirectoryInfo(filePath);
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

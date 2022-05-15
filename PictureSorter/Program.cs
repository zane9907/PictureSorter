using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Text;
using PictureSorterAddons;

namespace PictureSorter
{
    class Program
    {
        static int FindExtensionPoint(string fileName)
        {
            return fileName.LastIndexOf('.');
        }

        static int NumberOfCopies(string fileName, string destDirName)
        {
            int count = 0;

            DirectoryInfo dirInfo = new DirectoryInfo(destDirName);
            var files = dirInfo.EnumerateFiles().OrderBy(x => x.Name).ToList();

            //int i = 0;
            //while (i < files.Count && !files[i].Name.Contains(fileName))
            //{
            //    i++;
            //}
            //if (i < files.Count)
            //{
            //    count++;
            //    i++;
            //    while (i < files.Count && files[i].Name.Contains(fileName))
            //    {
            //        count++;
            //        i++;
            //    }
            //}

            foreach (var item in files)
            {
                string name = Path.GetFileNameWithoutExtension(item.FullName);
                if (name.ToLower().Contains(fileName.ToLower()))
                {
                    count++;
                }
            }

            return count;
        }


        static void Main(string[] args)
        {
            //var asd = CodePagesEncodingProvider.Instance.GetEncoding(437)
            //    .GetString(new byte[] { 219, 219, 219, 176, 176, 176, 176 });

            //Console.WriteLine(asd);
            //;           




            var baseFolder = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent;

            //var baseFolder = new DirectoryInfo("from");

            var files = baseFolder.EnumerateFiles().Where(
                x => Enum.GetNames(typeof(ExtensionType)).Contains(x.Extension.ToLower().Replace(".", ""))).ToList();


            foreach (var file in files)
            {
                string creationMonth = file.LastWriteTime.Month.ToString();
                int creationYear = file.LastWriteTime.Year;

                string newFolderPath = baseFolder.FullName + "\\" +
                    creationYear + " " + Enum.Parse(typeof(Months), creationMonth).ToString();


                if (!Directory.Exists(newFolderPath))
                {
                    Directory.CreateDirectory(newFolderPath);
                }


                try
                {
                    File.Move(file.FullName, Path.Combine(newFolderPath, file.Name));
                }
                catch (IOException)
                {
                    string existingFileName = file.Name;

                    string modifiedName = existingFileName.Insert(
                        FindExtensionPoint(existingFileName),
                        $" ({NumberOfCopies(Path.GetFileNameWithoutExtension(existingFileName),  newFolderPath)})"
                        );
                    

                    File.Move(file.FullName, Path.Combine(newFolderPath, modifiedName), false);
                }

                Console.WriteLine($"{file.FullName} MOVED TO {newFolderPath}");
            }

            Console.WriteLine("END");
            Console.ReadLine();
        }
    }
}

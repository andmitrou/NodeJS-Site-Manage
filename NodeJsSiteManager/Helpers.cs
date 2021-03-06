﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
namespace NodeJsSiteManager
{
    public class Helpers
    {
        public static List<T> ReadJsonFile<T>(string filepath)
        {
            var contents = System.IO.File.ReadAllText(filepath);
            var result = JsonConvert.DeserializeObject<List<T>>(contents);
            return result;
        }
          
        public static void MoveFile(string sourcePath,string destinationPath)
        {
            File.Copy(sourcePath, destinationPath);
            File.Delete(sourcePath);
        }

        public static void CopyDirectory(string sourceDir, string targetDir)
        {
            if(!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(Directory.GetParent(sourceDir).FullName, targetDir));

            
            foreach (string newPath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(Directory.GetParent(sourceDir).FullName, targetDir), true);
        }

    }
}

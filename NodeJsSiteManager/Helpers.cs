using System;
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
        public static List<T> ReadJsonFile<T>(string filepath){
            var contents = System.IO.File.ReadAllText(filepath);
            var result = JsonConvert.DeserializeObject<List<T>>(contents);
            return result;
        }

       
    }
}

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DapperRepository.Core.Configuration
{
    public class ConnConfig
    {
        public static string GetConnectionString(string connKey)
        {
            if (string.IsNullOrEmpty(connKey))
                return string.Empty;

            string filePath = CommonHelper.MapPath("~/App_Data/DbConnSettings.json");

            if (!File.Exists(filePath))
                return string.Empty;

            string text = File.ReadAllText(filePath);

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            Dictionary<string, string> connStrDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

            return connStrDict[connKey];
        }
    }
}

using System;
using System.IO;
using System.Web.Script.Serialization;

namespace RimWorldModDevProbe.Core
{
    public class ProbeConfig
    {
        public string GamePath { get; set; }
        public string ModsPath { get; set; }

        private static readonly string ConfigFileName = "config.json";

        public static ProbeConfig Load()
        {
            var configPath = GetConfigPath();
            if (File.Exists(configPath))
            {
                try
                {
                    var json = File.ReadAllText(configPath);
                    var serializer = new JavaScriptSerializer();
                    return serializer.Deserialize<ProbeConfig>(json);
                }
                catch { }
            }
            return new ProbeConfig();
        }

        public void Save()
        {
            var configPath = GetConfigPath();
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(this);
            File.WriteAllText(configPath, json);
        }

        public static void CreateDefault()
        {
            var config = new ProbeConfig
            {
                GamePath = "",
                ModsPath = ""
            };

            var configPath = GetConfigPath();
            var defaultContent = @"{
  ""gamePath"": """",
  ""modsPath"": """",
  ""_comment"": ""Set gamePath to your RimWorld installation directory (e.g. D:/Games/RimWorld), modsPath is optional""
}";
            File.WriteAllText(configPath, defaultContent);
        }

        public static string GetConfigPath()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var projectDir = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\"));
            return Path.Combine(projectDir, ConfigFileName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ProudMedStatusAPI
{
    public class Config
    {
        private readonly string _configPath;
        private readonly Dictionary<string, string> _config = new();

        public string ConnectionStringJSD => GetValue("ConnectionString_JSD");
        public string DomainAPI => GetValue("DomainAPI");
        public int TimeProcess => int.TryParse(GetValue("Timeprocess"), out int t) ? t : 10;
        public int DayClearData => int.TryParse(GetValue("DayClearData"), out int d) ? d : 5;
        public int DayClearLog => int.TryParse(GetValue("DayClearLog"), out int d) ? d : 5;

        public Config(string configPath = "Config.ini")
        {
            // หาไฟล์จาก folder เดียวกับ .exe เสมอ
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            _configPath = Path.Combine(exeDir, "Config", configPath);
            Load();
        }

        private void Load()
        {
            if (!File.Exists(_configPath))
                throw new FileNotFoundException($"ไม่พบไฟล์ Config.ini ที่: {_configPath}");

            foreach (var line in File.ReadAllLines(_configPath))
            {
                var clean = Regex.Replace(line, @"#.*$", "").Trim();
                if (string.IsNullOrWhiteSpace(clean)) continue;

                var idx = clean.IndexOf('=');
                if (idx < 0) continue;

                var key = clean[..idx].Trim();
                var value = clean[(idx + 1)..].Trim();
                _config[key] = value;
            }
        }

        private string GetValue(string key)
        {
            return _config.TryGetValue(key, out var val) ? val : string.Empty;
        }
    }
}
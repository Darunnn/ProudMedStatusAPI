using System;
using System.IO;
using System.Text;

namespace ProudMedStatusAPI
{
    public class LogManager
	{
        private readonly string _logDir;
        private readonly string _errorDir;
        private readonly int _dayClearLog;
        private readonly object _lock = new();

        public LogManager(Config config)
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            _logDir    = Path.Combine(exeDir, "logs");
            _errorDir  = Path.Combine(exeDir, "logs", "error");
            _dayClearLog = config.DayClearLog;

            Directory.CreateDirectory(_logDir);
            Directory.CreateDirectory(_errorDir);
        }

        // ---- public methods ----

        public void Info(string message)  => Write(_logDir,  "INFO",  message);
        public void Error(string message) => Write(_errorDir,"ERROR", message);

        public void ClearOldLogs()
        {
            DeleteOldFiles(_logDir,   _dayClearLog);
            DeleteOldFiles(_errorDir, _dayClearLog);
        }

        // ---- private ----

        private void Write(string dir, string level, string message)
        {
            try
            {
                string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                string filePath = Path.Combine(dir, fileName);
                string line     = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";

                lock (_lock)
                {
                    File.AppendAllText(filePath, line + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch { }
        }

        private void DeleteOldFiles(string dir, int days)
        {
            try
            {
                foreach (var file in Directory.GetFiles(dir, "*.log"))
                {
                    var created = File.GetCreationTime(file);
                    if ((DateTime.Now - created).TotalDays > days)
                    {
                        File.Delete(file);
                        Info($"ลบ log เก่า: {Path.GetFileName(file)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Write(_logDir, "ERROR", $"ClearOldLogs failed: {ex.Message}");
            }
        }
    }
}

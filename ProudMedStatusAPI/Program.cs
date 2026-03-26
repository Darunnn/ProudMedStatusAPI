using System;
using System.Windows.Forms;

namespace ProudMedStatusAPI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            const string appName = "ProudMedStatusAPI";

            bool createdNew;
            var mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                MessageBox.Show(
                    "โปรแกรมนี้กำลังทำงานอยู่แล้ว",
                    "แจ้งเตือน",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // ---- Logger (ต้องสร้างก่อน Exception Handlers) ----
            Config config;
            LogManager log;
            try
            {
                config = new Config();
                log = new LogManager(config);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"ไม่สามารถโหลด Config.ini ได้\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                mutex.ReleaseMutex();
                return;
            }

            // ---- Global Exception Handlers ----
            Application.ThreadException += (_, e) =>
            {
                log.Error($"ThreadException: {e.Exception}");
                MessageBox.Show(
                    $"เกิดข้อผิดพลาด:\n{e.Exception.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            };

            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                log.Error($"UnhandledException: {e.ExceptionObject}");
            };

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // ---- Worker ----
            var worker = new DispenseWorker(config, log);

            // ---- Main Form ----
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var form = new ProudMedStatusAPI(log);

            // เชื่อม worker → UI
            worker.OnStatsUpdated = (pending, success) =>
                form.UpdateStats(pending, success);

            worker.Start();
            log.Info("Application started");

            Application.Run(form);

            // ---- Shutdown ----
            worker.Stop();
            log.Info("Application stopped");
            mutex.ReleaseMutex();
        }
    }
}
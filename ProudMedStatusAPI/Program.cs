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

            var mutex = new Mutex(true, appName, out bool createdNew);

            if (!createdNew)
            {
                MessageBox.Show(
                    "โปรแกรมนี้กำลังทำงานอยู่แล้ว",
                    "แจ้งเตือน",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // ---- Config + Logger ----
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

            // ---- Worker (สร้างที่นี่ที่เดียว ไม่ให้ Form สร้างซ้ำ) ----
            var worker = new DispenseWorker(config, log);

            // ---- Main Form ----
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ส่ง worker เข้า Form ตรงๆ
            var form = new ProudMedStatusAPI(config, log, worker);

            // Start หลังสร้าง Form เสร็จ เพื่อให้ UI callback พร้อมรับได้ทันที
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
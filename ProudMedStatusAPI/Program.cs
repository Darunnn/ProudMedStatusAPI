namespace ProudMedStatusAPI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
       
        static void Main()
        {
            string appName = "ProudMedStatusAPI";
            Mutex mutex;
            bool createdNew = false;

            mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                // มี instance อื่นทำงานอยู่แล้ว
                MessageBox.Show("โปรแกรมนี้กำลังทำงานอยู่แล้ว", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            mutex.ReleaseMutex();
            // ⭐ เพิ่ม: สร้าง logger ก่อน
           
            // ⭐ เพิ่ม: Global Exception Handlers
          

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ProudMedStatusAPI());
           
            
        }
    }
}
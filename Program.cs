using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaintainReport {
    static class Program {

        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static async Task Main(string[] args) {

         string temp = HashPassword("Test12345");
         return;




            if (args?.Length > 0 && args?[0]?.ToLower() == "console") {
                //CDC_系統指標 (console版本)
                MaintainReportConsole maintainReportConsole = new MaintainReportConsole();
                maintainReportConsole.ExecuteJob();
            } else if (args?.Length > 0 && args?[0]?.ToLower() == "feedback") {
                //CDC_系統指標 (console版本,只發問題反映)
                MaintainReportConsole maintainReportConsole = new MaintainReportConsole();
                maintainReportConsole.Feedback();
            } else if (args?.Length > 0 && args?[0]?.ToLower() == "alarm") {
                //CDC_系統指標 (console版本,只發主動告警)
                MaintainReportConsole maintainReportConsole = new MaintainReportConsole();
                maintainReportConsole.Alarm();
            } else if (args?.Length > 0 && args?[0]?.ToLower() == "checklogin") {
                //偵測系統運作情況 (console版本)
                DetectSystemStatus detectSystemStatus = new DetectSystemStatus();
                await detectSystemStatus.CheckLogin();
            } else {
                //CDC_系統指標 (winform版本)
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MaintainReport());
            }

        }


        private const int SaltByteSize = 32;
        private const int HashByteSize = 32;
        private const int Iterations = 1024;

        public static string HashPassword(string password)
        {


            byte[] salt;
            byte[] derived;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, SaltByteSize, Iterations, HashAlgorithmName.SHA512))
            {
                salt = deriveBytes.Salt;
                derived = deriveBytes.GetBytes(HashByteSize);
            }

            byte[] outputBytes = new byte[1 + sizeof(int) + SaltByteSize + HashByteSize];
            Buffer.BlockCopy(BitConverter.GetBytes(Iterations), 0, outputBytes, 1, sizeof(int));
            Buffer.BlockCopy(salt, 0, outputBytes, 1 + sizeof(int), SaltByteSize);
            Buffer.BlockCopy(derived, 0, outputBytes, 1 + sizeof(int) + SaltByteSize, HashByteSize);
            return Convert.ToBase64String(outputBytes);
        }
    }
}

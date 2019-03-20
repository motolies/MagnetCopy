using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MagnetCopyUI
{
    static class Program
    {

        const int WM_COPYDATA = 0x4A;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, ref COPYDATASTRUCT lParam);

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        public static void SendMessage(string msg)
        {
            List<Process> pro = Process.GetProcessesByName("MagnetCopyUI").ToList();
            if (pro.Count > 0)
            {
                byte[] buff = System.Text.Encoding.Default.GetBytes(msg);

                COPYDATASTRUCT cds = new COPYDATASTRUCT();
                cds.dwData = IntPtr.Zero;
                cds.cbData = buff.Length + 1;
                cds.lpData = msg;

                foreach (Process p in pro)
                    SendMessage(p.MainWindowHandle, WM_COPYDATA, 0, ref cds);
            }
        }

        static void RunProcess(string[] args, Mutex mutex)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm main = new MainForm();
            if (args.Length > 0)
                main.ReceivedDataProcess(args[0]);

            Application.Run(main);
            mutex.ReleaseMutex();

        }

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool bnew;
            Mutex mutex = new Mutex(true, "MagnetCopyUI", out bnew);

            if (bnew)
                RunProcess(args, mutex);
            else
            {
                if (args.Length < 1)
                    return;

                if (args[0] == "runas")
                    RunProcess(new string[0], mutex);
                else
                    SendMessage(args[0]);
            }
        }
    }
}

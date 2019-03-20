using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MagnetCopy
{
    class Program
    {

        static readonly string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);


        // https://ehdrn.tistory.com/295

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
            if (pro.Count < 1)
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(dir, "MagnetCopyUI.exe"),
                };
                Process p = new Process();
                p.StartInfo = processStartInfo;
                p.Start();

                while (true)
                {
                    try
                    {
                        var time = p.StartTime;
                        Thread.Sleep(1000);
                        break;
                    }
                    catch (Exception) { }
                }

                pro.Add(p);
            }

            byte[] buff = System.Text.Encoding.Default.GetBytes(msg);

            COPYDATASTRUCT cds = new COPYDATASTRUCT();
            cds.dwData = IntPtr.Zero;
            cds.cbData = buff.Length + 1;
            cds.lpData = msg;

            SendMessage(pro[0].MainWindowHandle, WM_COPYDATA, 0, ref cds);
        }


        static void Main(string[] args)
        {
            if (args.Length < 1)
                return;

            try
            {
                string param = args[0];

                SendMessage(param);
                LogWrite(param);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogWrite(ex.Message);
            }
            finally
            {
                //Console.ReadLine();
                Environment.Exit(0);
            }

        }

        static void LogWrite(string msg)
        {
            LogHelper.WriteText(Path.Combine(dir, "log.txt"), string.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg));
        }
    }
}

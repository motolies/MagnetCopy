using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MagnetCopyUI
{
    public partial class MainForm : Form
    {
        List<string> magnetList = new List<string>();


        const int WM_COPYDATA = 0x4A;

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        protected override void WndProc(ref Message m)
        {
            try
            {
                switch (m.Msg)
                {
                    case WM_COPYDATA:
                        COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));
                        ReceivedDataProcess(cds.lpData);
                        break;
                    default:
                        base.WndProc(ref m);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ReceivedDataProcess(string data)
        {
            // 데이터 받으면 처리할 부분
            magnetList.Add(data);
            magnetList = magnetList.Distinct().ToList();

            //중복 제거 후 화면에 출력
            string text = string.Join("\r\n", magnetList);
            richTextBox1.Text = text;
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void chkRegistry_CheckedChanged(object sender, EventArgs e)
        {
            bool result = false;

            var chk = sender as CheckBox;

            if (chk.Checked)
            {
                result = RegistryHelper.SetMagnet();
            }
            else
            {
                result = RegistryHelper.DeleteMagnet();
            }

            if (!result)
            {
                chkRegistry.CheckedChanged -= chkRegistry_CheckedChanged;
                chkRegistry.Checked = !chkRegistry.Checked;
                chkRegistry.CheckedChanged += chkRegistry_CheckedChanged;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chkRegistry.Checked = RegistryHelper.GetMagnet();
            chkRegistry.CheckedChanged += chkRegistry_CheckedChanged;
        }
    }
}

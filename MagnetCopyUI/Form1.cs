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
    public partial class Form1 : Form
    {
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

        private void ReceivedDataProcess(string data)
        {
            // 데이터 받으면 처리할 부분
            richTextBox1.AppendText(data + Environment.NewLine);
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void chkRegistry_CheckedChanged(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;

            if (chk.Checked)
            {
                RegistryHelper.SetMagnet();
            }
            else
            {
                RegistryHelper.DeleteMagnet();
            }
        }
    }
}

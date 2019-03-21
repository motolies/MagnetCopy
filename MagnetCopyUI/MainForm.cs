﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MagnetCopyUI
{
    public partial class MainForm : Form
    {

        static readonly string Dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static readonly string RegApp = "MagnetRegistry.exe";


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

        public static bool RunRunasRegistry(string param, out string result)
        {
            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.UseShellExecute = true;
            procInfo.FileName = Path.Combine(Dir, RegApp);
            procInfo.Arguments = param;
            procInfo.WorkingDirectory = Environment.CurrentDirectory;
            procInfo.Verb = "runas";

            Process p = new Process();
            p.StartInfo = procInfo;

            try
            {
                p.Start();
                result = p.StandardOutput.ReadToEnd();
                return true;
            }
            catch (Exception x)
            {
                result = x.Message;
                return false;
            }
            finally
            {
                //Wait for the process to end.
                p.WaitForExit();
                p.Close();
            }

        }

        private void chkRegistry_CheckedChanged(object sender, EventArgs e)
        {
            string result = string.Empty;
            var chk = sender as CheckBox;

            if (chk.Checked)
            {
                DialogResult dialog = MessageBox.Show("Magnet Link를 레지스트리에 등록하기 위해서는 관리자 권한이 필요합니다. 계속 진행하시겠습니까?", "권한요청", MessageBoxButtons.YesNo);
                if(dialog == DialogResult.Yes)
                    RunRunasRegistry("install", out result);
            }
            else
            {
                DialogResult dialog = MessageBox.Show("Magnet Link를 레지스트리에서 삭제하기 위해서는 관리자 권한이 필요합니다. 계속 진행하시겠습니까?", "권한요청", MessageBoxButtons.YesNo);
                if (dialog == DialogResult.Yes)
                    RunRunasRegistry("uninstall", out result);
            }

            chkRegistry.CheckedChanged -= chkRegistry_CheckedChanged;
            chkRegistry.Checked = RegistryHelper.GetMagnet();
            chkRegistry.CheckedChanged += chkRegistry_CheckedChanged;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chkRegistry.Checked = RegistryHelper.GetMagnet();
            chkRegistry.CheckedChanged += chkRegistry_CheckedChanged;
        }

        private void chkTopMost_CheckedChanged(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;
            this.TopMost = chk.Checked;
        }
    }
}

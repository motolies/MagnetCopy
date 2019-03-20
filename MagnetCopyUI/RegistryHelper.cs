using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MagnetCopyUI
{
    internal class RegistryHelper
    {
        static readonly string addr = "Magnet";
        static readonly string consoleExeFile = "MagnetCopyUI.exe";
        static readonly string msg = @"레지스트리 설정은 관리자 권한으로만 가능합니다.
관리자 권한으로 실행시키시겠습니까?
레지스트리 설정 후에는 다시 사용자 권한으로 실행시켜주세요.";


        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();

            if (identity != null)
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            return false;
        }

        public static void RunRunas()
        {
            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.UseShellExecute = true;
            procInfo.FileName = Application.ExecutablePath;
            procInfo.Arguments = "runas";
            procInfo.WorkingDirectory = Environment.CurrentDirectory;
            procInfo.Verb = "runas";

            Application.Exit();
            Process.Start(procInfo);
        }




        public static bool GetMagnet()
        {
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(addr);
            return key != null;
        }

        public static bool SetMagnet()
        {
            if (!IsAdministrator())
            {
                DialogResult result = MessageBox.Show(msg, "관리자 권한 실행", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    RunRunas();
                }
                else
                {
                    return false;
                }
            }



            string path = System.Windows.Forms.Application.StartupPath;
            bool exists = File.Exists(Path.Combine(path, consoleExeFile));

            if (!exists)
                throw new Exception("MagnetCopy.exe 파일을 찾을 수 없습니다. MagnetCopyUI.exe 파일과 같은 폴더에 넣어주세요.");

            string pathParam = string.Format("\"{0}\\{1}\" \"%L\"", path, consoleExeFile);

            RegistryKey key = Registry.ClassesRoot.CreateSubKey(addr, RegistryKeyPermissionCheck.ReadWriteSubTree);
            key.SetValue("", "");
            key.SetValue("URL Protocol", "", RegistryValueKind.String);
            key.SetValue("Content Type", "", RegistryValueKind.String);

            RegistryKey defaultIcon = key.CreateSubKey("DefaultIcon", RegistryKeyPermissionCheck.ReadWriteSubTree);
            defaultIcon.SetValue("", pathParam);

            RegistryKey shell = key.CreateSubKey("shell", RegistryKeyPermissionCheck.ReadWriteSubTree);
            shell.SetValue("", "");

            RegistryKey shellOpen = shell.CreateSubKey("open", RegistryKeyPermissionCheck.ReadWriteSubTree);
            shellOpen.SetValue("FriendlyAppName", "MagnetCopy", RegistryValueKind.String);

            RegistryKey shellOpenCommand = shellOpen.CreateSubKey("command", RegistryKeyPermissionCheck.ReadWriteSubTree);
            shellOpenCommand.SetValue("", pathParam);

            key.Close();
            return true;
        }

        public static bool DeleteMagnet()
        {
            if (!IsAdministrator())
            {
                DialogResult result = MessageBox.Show(msg, "관리자 권한 실행", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    RunRunas();
                }
                else
                {
                    return false;
                }
            }

            RegistryKey key = Registry.ClassesRoot;
            if (key != null)
            {
                key.DeleteSubKeyTree(addr);
                key.Close();
            }

            return true;
        }
    }
}

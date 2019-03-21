using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;


namespace MagnetRegistry
{
    internal class RegistryHelper
    {
        static readonly string Dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static readonly string AppExe = System.AppDomain.CurrentDomain.FriendlyName;


        static readonly string addr = "Magnet";
        static readonly string consoleExeFile = "MagnetCopyUI.exe";
        static readonly string msg = @"레지스트리 설정은 관리자 권한으로만 가능합니다.
관리자 권한으로 실행시키시겠습니까? [Y/n]";


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

        public static void RunRunas(string param)
        {
            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.UseShellExecute = true;
            procInfo.FileName = Path.Combine(Dir, AppExe);
            procInfo.Arguments = param;
            procInfo.WorkingDirectory = Environment.CurrentDirectory;
            procInfo.Verb = "runas";

            Process.Start(procInfo);
        }




        public static bool GetMagnet()
        {
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(addr);
            return key != null;
        }

        public static string SetMagnet()
        {
            if (!IsAdministrator())
            {
                Console.WriteLine(msg);
                string result = Console.ReadLine();
                if (result.ToLower() == "y")
                {
                    RunRunas("install");
                }
                else
                {
                    return "false";
                }
            }

            bool exists = File.Exists(Path.Combine(Dir, consoleExeFile));

            if (!exists)
                throw new Exception("MagnetCopy.exe 파일을 찾을 수 없습니다. MagnetCopyUI.exe 파일과 같은 폴더에 넣어주세요.");

            string pathParam = string.Format("\"{0}\\{1}\" \"%L\"", Dir, consoleExeFile);

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
            return "true";
        }

        public static string DeleteMagnet()
        {
            if (!IsAdministrator())
            {
                Console.WriteLine(msg);
                string result = Console.ReadLine();
                if (result.ToLower() == "y")
                {
                    RunRunas("uninstall");
                }
                else
                {
                    return "false";
                }
            }

            RegistryKey key = Registry.ClassesRoot;
            if (key != null)
            {
                key.DeleteSubKeyTree(addr);
                key.Close();
            }

            return "true";
        }
    }
}

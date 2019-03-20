using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnetCopyUI
{
    internal class RegistryHelper
    {
        public static string addr = "Magnet";
        public static string consoleExeFile = "MagnetCopy.exe";

        public static void SetMagnet()
        {

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

        }

        public static void DeleteMagnet()
        {

            RegistryKey key = Registry.ClassesRoot;
            if (key != null)
            {
                key.DeleteSubKeyTree(addr);
                key.Close();
            }
        }
    }
}

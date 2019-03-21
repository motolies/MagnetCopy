using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnetRegistry
{
    class Program
    {
        static void Main(string[] args)
        {
            string result = string.Empty;

            if (args.Length < 1)
                return;

            string opt = args[0];
            
            if(opt == "install")
            {
                result = RegistryHelper.SetMagnet();
            }
            else if(opt == "uninstall")
            {
                result = RegistryHelper.DeleteMagnet();
            }

            Console.Write(result);

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;

namespace virtualPrinter
{
    public static class PrinterClass // class which carries SetDefaultPrinter function
    {
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDefaultPrinter(string Printer);
    }

    class myPrinterClass
    {
       public static void getPrinterNames()
        {
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                MessageBox.Show(printer);
            }
        }
        public static void installPrinter(string printerName) //works on win 7,8,8.1,10 on both x84 and x64
        {
            //https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/rundll32-printui
            //  /if	        Installs a printer by using an .inf file.
            //  /b[name]	Specifies the base printer name.
            //  /@[file]	Specifies a command-line argument file and directly inserts the text in that file into the command line.
            //  /f[file]	Species the Universal Naming Convention (UNC) path and name of the .inf file name or the output file name, depending on the task that you are performing. Use /F[file] to specify a dependent .inf file.
            //  /r[port]	Specifies the port name.
            //  /m[model]	Specifies the driver model name. (This value can be specified in the .inf file.)

            string arg;
            arg = "printui.dll , PrintUIEntry /if /b " + "\"" + printerName + "\"" + @" /f C:\Windows\inf\ntprint.inf /r " + "\"" + "lpt1:" + "\"" + " /m " + "\"" + "Generic / Text Only" + "\""; //initial arg
            ProcessStartInfo p = new ProcessStartInfo();
            p.FileName = "rundll32.exe";
            p.Arguments = arg;
            p.WindowStyle = ProcessWindowStyle.Hidden;
            
            try
            {
                Process.Start(p);
                MessageBox.Show(printerName + " installed succesfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong. Try again!");
            }
        }
        public static bool printerExists(string printerName)
        {
            bool res = false;
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                if (printer == printerName)
                {
                    res = true;
                }
            }
            return res;
        }
        public static void uninstallPrinter(string printerName)
        {
            string arg;
            ProcessStartInfo p = new ProcessStartInfo();
            arg = "printui.dll, PrintUIEntry /dl /n " + "\"" + printerName + "\"";
            if (printerExists(printerName))
            {
                p.FileName = "rundll32.exe";
                p.Arguments = arg;
                p.WindowStyle = ProcessWindowStyle.Hidden;
                try
                {
                    Process.Start(p);
                    MessageBox.Show(printerName + " unistalled successfully");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException.ToString());
                }
                p = null;
            }
        }
       
        public static string GetLocalIPAddress() //erxomeno feature
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }



    }
}

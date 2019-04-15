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
			Winspool.AddLocalPort(@"C:\MyLocalPort.txt");
            string arg;
            arg = "printui.dll , PrintUIEntry /if /b " + "\"" + printerName + "\"" + @" /f C:\Windows\inf\ntprint.inf /r " + "\"" + @"C:\MyLocalPort.txt" + "\"" + " /m " + "\"" + "Generic / Text Only" + "\""; //initial arg
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

	   public static class Winspool
        {
            [StructLayout(LayoutKind.Sequential)]
            private class PRINTER_DEFAULTS
            {
                public string pDatatype;
                public IntPtr pDevMode;
                public int DesiredAccess;
            }

            [DllImport("winspool.drv", EntryPoint = "XcvDataW", SetLastError = true)]
            private static extern bool XcvData(
                IntPtr hXcv,
                [MarshalAs(UnmanagedType.LPWStr)] string pszDataName,
                IntPtr pInputData,
                uint cbInputData,
                IntPtr pOutputData,
                uint cbOutputData,
                out uint pcbOutputNeeded,
                out uint pwdStatus);

            [DllImport("winspool.drv", EntryPoint = "OpenPrinterA", SetLastError = true)]
            private static extern int OpenPrinter(
                string pPrinterName,
                ref IntPtr phPrinter,
                PRINTER_DEFAULTS pDefault);

            [DllImport("winspool.drv", EntryPoint = "ClosePrinter")]
            private static extern int ClosePrinter(IntPtr hPrinter);

            public static int AddLocalPort(string portName)
            {
                PRINTER_DEFAULTS def = new PRINTER_DEFAULTS();

                def.pDatatype = null;
                def.pDevMode = IntPtr.Zero;
                def.DesiredAccess = 1; //Server Access Administer

                IntPtr hPrinter = IntPtr.Zero;

                int n = OpenPrinter(",XcvMonitor Local Port", ref hPrinter, def);
                if (n == 0)
                    return Marshal.GetLastWin32Error();

                if (!portName.EndsWith("\0"))
                    portName += "\0"; // Must be a null terminated string

                // Must get the size in bytes. Rememeber .NET strings are formed by 2-byte characters
                uint size = (uint)(portName.Length * 2);

                // Alloc memory in HGlobal to set the portName
                IntPtr portPtr = Marshal.AllocHGlobal((int)size);
                Marshal.Copy(portName.ToCharArray(), 0, portPtr, portName.Length);

                uint needed; // Not that needed in fact...
                uint xcvResult; // Will receive de result here

                XcvData(hPrinter, "AddPort", portPtr, size, IntPtr.Zero, 0, out needed, out xcvResult);

                ClosePrinter(hPrinter);
                Marshal.FreeHGlobal(portPtr);

                return (int)xcvResult;
            }
        }

    }
}

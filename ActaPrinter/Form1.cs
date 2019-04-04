using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;


namespace virtualPrinter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                myPrinterClass.installPrinter("dokimastikos64aris");
            }
            else
            {
                myPrinterClass.installPrinter("dokimastikos32aris");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            myPrinterClass.getPrinterNames();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                try
                {
                    myPrinterClass.uninstallPrinter("dokimastikos64aris");
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                try
                {
                    myPrinterClass.uninstallPrinter("dokimastikos32aris");
                }
                catch (Exception)
                {

                    throw;
                }

            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
           
            if (Environment.Is64BitOperatingSystem)
            {
                try
                {
                    PrinterClass.SetDefaultPrinter("dokimastikos64aris");
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                try
                {
                    PrinterClass.SetDefaultPrinter("dokimastikos32aris");
                }
                catch (Exception)
                {

                    throw;
                }

            }


        }
    }

}


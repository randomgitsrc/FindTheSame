using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading; 

namespace LianLianKan
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            PrgMain prgmain = new PrgMain();
             
            Console.WriteLine("\n----------- 输入任意键结束程序 ---------------\n"); 
            Console.ReadKey(); 
        } 
    } 
}

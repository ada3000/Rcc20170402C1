using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunStdIn
{
    class Program
    {
        static void Main(string[] args)
        {
            Process proc = new Process();

            proc.StartInfo.FileName = "TaskA";

            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = true;
            
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.StandardOutputEncoding = Encoding.UTF8;

            proc.OutputDataReceived += Proc_OutputDataReceived;
            
            //proc.StartInfo.Sta = Encoding.UTF8;

            proc.EnableRaisingEvents = true;
            //proc.StartInfo.CreateNoWindow = true;
            

            proc.Exited += Proc_Exited;

            proc.Start();

            List<string> testData = new List<string> { "5", "-1 1 0 1 -1" };

            foreach(var test in testData)
            {
                Console.WriteLine("[SEND] " + test); 
                proc.StandardInput.WriteLine(test);
            }


            string line = null;
            while ((line = proc.StandardOutput.ReadLine()) != null)
                Console.WriteLine("[OUT] " + line);
            
            proc.WaitForExit();

            Console.ReadKey();
        }

        private static void Proc_Exited(object sender, EventArgs e)
        {
            Console.WriteLine("[SYS] TestEnd");
        }

        private static void Proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("[RESP] " + e.Data);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileWatcherCore;

namespace FileWatcherTest
{
    class Program
    {
        static void Main(string[] args)
        {
            FileWatcher fwc = new FileWatcher(@"%temp%\FW\input", @"%temp%\FW\output");

            System.Diagnostics.Process.Start(Environment.ExpandEnvironmentVariables(@"%temp%\FW\input"));
            System.Diagnostics.Process.Start(Environment.ExpandEnvironmentVariables(@"%temp%\FW\output"));

            fwc.Start();

            Console.ReadKey();
        }
    }
}

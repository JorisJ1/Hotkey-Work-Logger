using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Hotkey_Work_Logger {

    /// <summary>
    ///  Might speed up the Shortcut Key delay in Windows 10:
    /// - Turn off specific background apps at Settings -> Privacy -> Background Apps.
    /// - Killing "Application Frame Host" (ApplicationFrameHost.exe) (if you kill this you can't run modern apps).
    /// - Disabling the 'SysMain' service (previously called SuperFetch).
    /// </summary>
    class Program {
        static void Main(string[] args) {

            Console.WriteLine("Hotkey Work Logger");

            // Determine file paths.
            string workItemsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "workitems.ini");
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv.txt");

            // Read work items from the INI file.
            // If the file does not exist, create and fill it with some example work items.
            string[] workItemsIni;
            if (File.Exists(workItemsPath)) {
                workItemsIni = File.ReadAllLines(workItemsPath);
            } else {
                string[] defaultWorkItems = new string[] {
                    "1=Task 1",
                    "2=Task 2",
                    "3=Task 3",
                    "M=Misc work",
                    "B=Break"
                };
                File.WriteAllLines(workItemsPath, defaultWorkItems);
                workItemsIni = defaultWorkItems;
            }

            // Convert the items to a Dictionary for easy retrieval later.
            Dictionary<char, string> workItems = new Dictionary<char, string>();
            foreach (string workItemDescriptor in workItemsIni) {
                string[] workItemDescriptorSpl = workItemDescriptor.Split('=');
                char workItemKeyChar = workItemDescriptorSpl[0][0];
                workItems.Add(workItemKeyChar, workItemDescriptorSpl[1]);
            }

            // Show a list of work items.
            foreach (KeyValuePair<char, string> item in workItems) {
                Console.WriteLine("[" + item.Key + "] " + item.Value);
            }

            // Ask for input.
            Console.Write("Choose a work item, enter a [c]ustom item, [e]dit the list or [v]iew the log file: ");
            ConsoleKeyInfo key = Console.ReadKey();

            // Determine the CSV delimiter.
            string sep = CultureInfo.CurrentCulture.TextInfo.ListSeparator;

            if (key.KeyChar == 'e') {
                // Open the work items file with the default application.
                Process fileopener = new Process();
                fileopener.StartInfo.FileName = workItemsPath;
                fileopener.Start();
                Environment.Exit(0);
            } if (key.KeyChar == 'c') {

                Console.Write(Environment.NewLine + "Enter custom item and press [ENTER]:");
                string customItem = Console.ReadLine();
                string logLine = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + sep + customItem + Environment.NewLine;
                File.AppendAllText(logPath, logLine);
                Environment.Exit(0);

            } if (key.KeyChar == 'v') {
                // Open the log file with the default application.
                OpenLogFile(logPath);
            } else if (workItems.TryGetValue(char.ToUpper(key.KeyChar), out string workItemName)) {
                // A work item exists for the key entered; write an entry in the log file.
                string logLine = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + sep + workItemName + Environment.NewLine;
                File.AppendAllText(logPath, logLine);
                Environment.Exit(0);
            } else {
                Console.WriteLine(Environment.NewLine + "No work item found for this key! (press any key to close)");
            }   
        }

        private static void OpenLogFile(string path) {
            if (File.Exists(path)) {
                Process fileopener = new Process();
                fileopener.StartInfo.FileName = path;
                fileopener.Start();
                Environment.Exit(0);
            } else {
                Console.WriteLine(Environment.NewLine + "Log file not found! (press any key to close)");
            }
        }
    }
}

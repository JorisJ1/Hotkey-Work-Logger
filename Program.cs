using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Hotkey_Work_Logger {
    class Program {

        private static string WorkItemsPath;
        private static string LogPath;
        private static Dictionary<char, string> WorkItemsChoiceList;

        private static void Main(string[] args) {

            Console.WriteLine("Hotkey Work Logger");

            // Determine file paths.
            // Ini's are used because by default they open with the built-in Windows Notepad,
            // which can open very fast (at least faster then Notepad++) if configured correctly.
            WorkItemsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "workitems.ini");
            LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log_" + DateTime.Now.ToString("yyyyMMdd") + ".ini");

            FillWorkItemsChoiceList();

            // Check if the commandline parameters contain a character,
            // and if so process it and write and item to the log.
            // This is for automation purposes.
            for (int i = 0; i < args.Length; i++) {
                if (args[i].Length == 1) {
                    ProcessWorkItemKey(args[i][0]);
                    return;
                }
            }

            
            ShowWorkItemsChoiceList();
            WaitForAndProcessInput();
        }

        /// <summary>
        /// Show a list of work items to the user.
        /// </summary>
        private static void ShowWorkItemsChoiceList() {
            foreach (KeyValuePair<char, string> item in WorkItemsChoiceList) {
                Console.WriteLine("[" + item.Key + "] " + item.Value);
            }
        }

        /// <summary>
        /// Read the 'configuration' file that contains work item choices.
        /// </summary>
        private static void FillWorkItemsChoiceList() {
            WorkItemsChoiceList = new Dictionary<char, string>();

            // Read work items from the INI file.
            // If the file does not exist, create and fill it with some example work items.
            string[] workItemsIni;
            if (File.Exists(WorkItemsPath)) {
                workItemsIni = File.ReadAllLines(WorkItemsPath);
            } else {
                string[] defaultWorkItems = new string[] {
                    "1=Task 1",
                    "2=Task 2",
                    "3=Task 3",
                    "M=Misc work",
                    "B=Break"
                };
                File.WriteAllLines(WorkItemsPath, defaultWorkItems);
                workItemsIni = defaultWorkItems;
            }

            // Convert the items to a Dictionary for easy retrieval later.
            foreach (string workItemDescriptor in workItemsIni) {
                string[] workItemDescriptorSpl = workItemDescriptor.Split('=');
                char workItemKeyChar = workItemDescriptorSpl[0][0];
                WorkItemsChoiceList.Add(workItemKeyChar, workItemDescriptorSpl[1]);
            }
        }

        private static void WaitForAndProcessInput() {

            // Ask for input.
            Console.Write("Choose a work item, enter a [c]ustom item, [e]dit the list or [v]iew the log file: ");
            ConsoleKeyInfo key = Console.ReadKey();

            if (key.KeyChar == 'e') {
                // Open the work items file with the default application.
                Process fileopener = new Process();
                fileopener.StartInfo.FileName = WorkItemsPath;
                fileopener.Start();
                Environment.Exit(0);
            }
            if (key.KeyChar == 'c') {

                Console.Write(Environment.NewLine + "Enter custom item and press [ENTER]:");
                string customItem = Console.ReadLine();
                WriteLogAndExit(customItem);
            }
            if (key.KeyChar == 'v') {
                // Open the log file with the default application.
                OpenLogFile(LogPath);
            } else {
                ProcessWorkItemKey(key.KeyChar);
            }
        }

        private static void ProcessWorkItemKey(char keyChar) {

            if (WorkItemsChoiceList.TryGetValue(char.ToUpper(keyChar), out string workItemName)) {
                // A work item exists for the key entered; write an entry in the log file.
                WriteLogAndExit(workItemName);
            } else {
                Console.WriteLine(Environment.NewLine + "No work item found for this key! (press any key to close)");
            }
        }

        private static void WriteLogAndExit(string workItemName) {

            // Determine the CSV delimiter.
            string sep = CultureInfo.CurrentCulture.TextInfo.ListSeparator;

            string logLine = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + sep + workItemName + Environment.NewLine;
            File.AppendAllText(LogPath, logLine);
            Environment.Exit(0);
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

using System;
using System.IO;

class Logger {

    public static void Log (string logMessage) {

        //if necessary to debug with this tool, be sure to update the file directory as appropriate
        bool enabled = true;
        string directory = "C:\\Users\\Mutilar\\Documents\\GitHub\\BitNaughtsUnity\\Assets\\bitnaughts\\Scripts\\Handlers";
        string file_name = "log.txt";

        if (enabled) {
            using (StreamWriter w = File.AppendText (directory + "\\" + file_name)) {
                w.Write ("\r\nLog Entry : ");
                w.WriteLine ($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                w.WriteLine ("  :");
                w.WriteLine ($"  :{logMessage}");
                w.WriteLine ("-------------------------------");
            }
        }
    }
}
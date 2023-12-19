using System;
using System.IO;

public static class LogHelper
{
    public static void WriteToLog(string path, string message)
    {
        if (!path.EndsWith(@"\"))
        {
            path += @"\";
        }
        File.AppendAllText($@"{path}log.txt", $"{DateTime.Now}: {message}{Environment.NewLine}");
    }
}



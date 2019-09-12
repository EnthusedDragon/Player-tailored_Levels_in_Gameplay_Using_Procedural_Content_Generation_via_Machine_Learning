using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class DataRecorder
{
    private static string filePath = Application.dataPath + "/CSV/Saved_data.csv";

    private static void CreateFile()
    {
        if (!File.Exists(filePath))
        {
            var createdFile = File.CreateText(filePath);
            createdFile.Dispose();
        }
    }

    public static void WriteRecordToCSV(List<string> record, string fullPath = "")
    {
        if (fullPath == "")
        {
            fullPath = filePath;
        }

        if (!File.Exists(fullPath))
        {
            var createdFile = File.CreateText(fullPath);
            createdFile.Dispose();
        }

        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        sb.AppendLine(string.Join(delimiter, record));

        StreamWriter outStream = File.AppendText(fullPath);
        outStream.WriteLine(sb);
        outStream.Close();
    }

    public static void Screenshot(string fullPath)
    {
        ScreenCapture.CaptureScreenshot(fullPath);
    }
}

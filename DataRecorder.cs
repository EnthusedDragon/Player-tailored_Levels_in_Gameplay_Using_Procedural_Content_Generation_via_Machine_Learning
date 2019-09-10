using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class DataRecorder
{
    private string filePath;

    public DataRecorder()
    {
        filePath = Application.dataPath + "/CSV/Saved_data.csv";
        CreateFile();
    }

    public DataRecorder(string filePath, string fileName)
    {
        this.filePath = filePath + fileName;
        CreateFile();
    }

    private void CreateFile()
    {
        if (!File.Exists(filePath))
            File.CreateText(filePath);
    }

    public void AddRecord(List<string> record)
    {
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        sb.AppendLine(string.Join(delimiter, record));

        StreamWriter outStream = File.AppendText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
    }

    public void Screenshot(string path, string name)
    {
        ScreenCapture.CaptureScreenshot(path + name);
    }
}

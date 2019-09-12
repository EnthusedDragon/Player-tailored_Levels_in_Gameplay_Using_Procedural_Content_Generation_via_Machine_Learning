using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class DataRecorder
{
    private static string csvFilePath = Application.dataPath + "/CSV/Saved_data.csv";
    private static string screenshotFilePath = Application.dataPath + "/Screenshots/";


    public static int resWidth = 100;
    public static int resHeight = 100;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="record">List of row data </param>
    /// <param name="fullPath">e.g. C:\CSV\data.csv</param>
    public static void WriteRecordToCSV(List<string> record, string fullPath = "")
    {
        if (fullPath == "")
        {
            fullPath = csvFilePath;
        }

        if (!fullPath.Contains(".csv"))
        {
            Debug.LogError("Path must contain .csv at the end.");
            return;
        }

        if (!File.Exists(fullPath))
        {
            var createdFile = File.CreateText(fullPath);
            createdFile.Dispose();
        }

        string delimiter = ";";

        StringBuilder sb = new StringBuilder();

        sb.AppendLine(string.Join(delimiter, record));

        StreamWriter outStream = File.AppendText(fullPath);
        outStream.Write(sb);
        outStream.Close();
        outStream.Dispose();
    }

    /// <summary>
    /// Takes a screenshot
    /// </summary>
    /// <param name="fullPath">e.g. C:\Screenshots</param>
    /// <param name="name">e.g. screenshotName.png</param>
    public static string Screenshot(string fullPath = "", string name = "")
    {
        if(fullPath == "")
        {
            fullPath = screenshotFilePath;
        }

        Directory.CreateDirectory(fullPath);

        if (name == "")
        {   
            fullPath += $"Screenshot {resWidth}x{resHeight}{DateTime.UtcNow.ToString("dd_MM_yyyy-HH.mm.ss.fff")}.png";
        }
        else
        {
            if (!name.Contains(".png") || !name.Contains(".jpg"))
            {
                Debug.LogError("Name must contain .png or .jpg at the end.");
                return "";
            }

            fullPath += name;
        }

        return fullPath;
    }

    public static void ScreenshotSetDefaultRes(int width, int height)
    {
        resWidth = width;
        resHeight = height;
    }

    public static void TakeTheShot(Camera camera, string fullPath = "", string name = "", int width = 0, int height = 0)
    {
        if(width == 0)
        {
            width = resWidth;
        }

        if (height == 0)
        {
            height = resHeight;
        }

        RenderTexture rt = new RenderTexture(width, height, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Screenshot(fullPath, name);
        File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("Took screenshot to: {0}", filename));
    }
}

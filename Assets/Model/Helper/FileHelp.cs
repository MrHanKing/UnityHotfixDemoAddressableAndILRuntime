using System.IO;
using System;
using UnityEngine;

public class FileHelp
{


    /// <summary>
    /// 输出文件
    /// </summary>
    public static void WriteFile(string data, string fileName, string path)
    {

        //创建的路径 必须要有Resources/table 两个文件夹

        FileStream aFile = new FileStream(path + fileName + ".txt", FileMode.Create, FileAccess.Write);
        aFile.SetLength(0);
        StreamWriter sw = new StreamWriter(aFile);
        sw.Write(data);
        sw.Close();
        aFile.Close();
        Debug.Log("创建" + fileName + "成功");

    }

    /// <summary>
    /// 读取文件
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ReadFile(string fileName, string path)
    {
        string str = string.Empty;
        try
        {
            str = File.ReadAllText(path + fileName);
        }
        catch
        {

        }

        return str;
    }

    /// <summary>
    /// 判断路径中文件是否存在
    /// </summary>
    /// <returns></returns>
    public static bool IsFileIn(string path)
    {
        try
        {
            FileStream fs = new FileStream(path, System.IO.FileMode.Open);
            long lengh = fs.Length;
            byte[] bytes = new byte[lengh];
            fs.Read(bytes, 0, (int)lengh);
            fs.Close();
            fs = null;
            if (bytes.Length == 0)
                return false;
            return true;
        }
        catch (Exception e)
        {
            //Debug.Log(e.ToString());
            byte[] bytes = new byte[0];
            return false;
        }
        finally
        {

        }

    }
}

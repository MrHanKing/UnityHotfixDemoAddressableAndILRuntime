using UnityEngine;
using ETModel;
using System.Text;
using System.Security.Cryptography;

namespace ETModel
{
    public class StringUtilHelp
    {

        public static string RenewZero(string str, int len)
        {
            while (str.Length < len)
            {
                str = "0" + str;

            }
            return str;
        }

        public static string WatchFormat(float time, bool showHour = true)
        {
            string h = RenewZero(Mathf.Floor(time / 3600f).ToString(), 2);
            string m;
            string s = RenewZero((time % 60f).ToString(), 2);

            if (showHour)
            {
                m = RenewZero((Mathf.Floor(time / 60f) % 60f).ToString(), 2);
                return h + ":" + m + ":" + s;

            }
            else
            {
                m = RenewZero(Mathf.Floor(time / 60f).ToString(), 2);
                return m + ":" + s;
            }
        }

        /// <summary>
        /// 返回文本长度
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int GetTextLength(string text)
        {
            int count = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if ((int)text[i] > 127)
                    count += 2;
                else
                    count += 1;
            }
            return count;
        }


        /// <summary>
        /// 文本是否在范围内
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsRangeByText(string text,int mix ,int max)
        {
            int length = GetTextLength(text);
            if (length >= mix && length <= max)
                return true;
            return false;
        }


        /// <summary>
        /// 随机产生常用汉字
        /// </summary>
        /// <param name="count">要产生汉字的个数</param>
        /// <returns>常用汉字</returns>
        public static string GenerateChineseWord(int count)
        {
            string chineseWords = "";
            System.Random rm = new System.Random();
            Encoding gb = Encoding.GetEncoding("gb2312");

            for (int i = 0; i < count; i++)
            {
                // 获取区码(常用汉字的区码范围为16-55)
                int regionCode = rm.Next(16, 56);

                // 获取位码(位码范围为1-94 由于55区的90,91,92,93,94为空,故将其排除)
                int positionCode;
                if (regionCode == 55)
                {
                    // 55区排除90,91,92,93,94
                    positionCode = rm.Next(1, 90);
                }
                else
                {
                    positionCode = rm.Next(1, 95);
                }

                // 转换区位码为机内码
                int regionCode_Machine = regionCode + 160;// 160即为十六进制的20H+80H=A0H
                int positionCode_Machine = positionCode + 160;// 160即为十六进制的20H+80H=A0H

                // 转换为汉字
                byte[] bytes = new byte[] { (byte)regionCode_Machine, (byte)positionCode_Machine };
                chineseWords += gb.GetString(bytes);
            }
            return chineseWords;
        }

        /// <summary>
        /// 字符串是否带空格
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsSpace(string s)
        {
           if (s.IndexOf(" ") >= 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// MD5字符串加密
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string GenerateMD5(string txt)
        {
            using (MD5 mi = MD5.Create())
            {
                byte[] buffer = Encoding.Default.GetBytes(txt);
                //开始加密
                byte[] newBuffer = mi.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    sb.Append(newBuffer[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static string GetStringByInt(int Num) 
        {
            if (Num == 0)
                return "00";
            else if (Num > 0 && Num < 10)
                return "0" + Num.ToString();
            else
                return Num.ToString();
        }
    }
}
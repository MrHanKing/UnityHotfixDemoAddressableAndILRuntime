using System;

namespace ETModel
{
    public static class TimeHelper
    {
        private static readonly long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        /// <summary>
        /// 客户端时间
        /// </summary>
        /// <returns></returns>
        public static long ClientNow()
        {
            return (DateTime.UtcNow.Ticks - epoch) / 10000;
        }

        public static long ClientNowSeconds()
        {
            return (DateTime.UtcNow.Ticks - epoch) / 10000000;
        }

        /// <summary>
        /// 登陆前是客户端时间,登陆后是同步过的服务器时间
        /// </summary>
        /// <returns></returns>
        public static long Now()
        {
            return ClientNow();
        }

        /// <summary>
        /// 获取时间转换为中国时间
        /// </summary>
        public static string GetTimeConverToChina()
        {
            DateTime targetTime = DateTime.Now;
            try
            {
                TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                targetTime = TimeZoneInfo.ConvertTime(DateTime.Now, pstZone);

            }
            catch (System.Exception)
            {
            }
            string times = string.Format("{0}-{1}-{2} {3}:{4}:{5}", targetTime.Year, targetTime.Month, targetTime.Day, targetTime.Hour, targetTime.Minute, targetTime.Second);
            return times;
        }

        public static int GetNowDay() {
            return DateTime.Now.Day;
        }
    }
}
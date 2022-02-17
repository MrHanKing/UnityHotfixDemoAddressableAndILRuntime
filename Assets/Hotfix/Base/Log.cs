using System;
using UnityEngine;

namespace ETHotfix
{
	public static class Log
	{
		public static void Warning(string msg)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor ||
				Application.platform == RuntimePlatform.OSXEditor ||
				Application.platform == RuntimePlatform.LinuxEditor) 
			{
				ETModel.Log.Warning(msg);
			}

		}

		public static void Info(string msg)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor ||
				Application.platform == RuntimePlatform.OSXEditor ||
				Application.platform == RuntimePlatform.LinuxEditor)
			{
				ETModel.Log.Info(msg);
			}
		}

		public static void Error(Exception e)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor ||
				Application.platform == RuntimePlatform.OSXEditor ||
				Application.platform == RuntimePlatform.LinuxEditor)
			{
				ETModel.Log.Error(e.ToStr());
			}
		}

		public static void Error(string msg)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor ||
				Application.platform == RuntimePlatform.OSXEditor ||
				Application.platform == RuntimePlatform.LinuxEditor)
			{
				ETModel.Log.Error(msg);
			}
			
		}

		public static void Debug(string msg)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor ||
				Application.platform == RuntimePlatform.OSXEditor ||
				Application.platform == RuntimePlatform.LinuxEditor)
			{
				ETModel.Log.Debug(msg);
			}
			
		}
		
		public static void Msg(object msg)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor ||
				Application.platform == RuntimePlatform.OSXEditor ||
				Application.platform == RuntimePlatform.LinuxEditor)
			{
				Debug(Dumper.DumpAsString(msg));
			}
		}
	}
}
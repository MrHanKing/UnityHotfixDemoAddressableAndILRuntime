using System;
using UnityEngine;

namespace ETModel
{
	public static class Log
	{
		public static void Trace(string msg)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor ||
				Application.platform == RuntimePlatform.OSXEditor ||
				Application.platform == RuntimePlatform.LinuxEditor) 
			{
				UnityEngine.Debug.Log(msg);
			}
				
		}

		public static void Warning(string msg)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor ||
				Application.platform == RuntimePlatform.OSXEditor ||
				Application.platform == RuntimePlatform.LinuxEditor) 
			{
				UnityEngine.Debug.LogWarning(msg);
			}
				
		}

		public static void Info(string msg)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor ||
				Application.platform == RuntimePlatform.OSXEditor ||
				Application.platform == RuntimePlatform.LinuxEditor) 
			{

				UnityEngine.Debug.Log(msg);
			}
		}

		public static void Error(Exception e)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor ||
				Application.platform == RuntimePlatform.OSXEditor ||
				Application.platform == RuntimePlatform.LinuxEditor) 
			{
				UnityEngine.Debug.LogError(e.ToString());
			}
				
		}

		public static void Error(string msg)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor ||
				Application.platform == RuntimePlatform.OSXEditor ||
				Application.platform == RuntimePlatform.LinuxEditor) 
			{
				UnityEngine.Debug.LogError(msg);
			}

				
		}

		public static void Debug(string msg)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor ||
				Application.platform == RuntimePlatform.OSXEditor ||
				Application.platform == RuntimePlatform.LinuxEditor) 
			{
				UnityEngine.Debug.Log(msg);
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
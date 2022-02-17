using System;

namespace ETHotfix
{
	public static class EnumHelper
	{
		public static int EnumIndex<T>(int value)
		{
			int i = 0;
			foreach (object v in Enum.GetValues(typeof (T)))
			{
				if ((int) v == value)
				{
					return i;
				}
				++i;
			}
			return -1;
		}

		public static T FromString<T>(string str)
		{
            if (!Enum.IsDefined(typeof(T), str))
            {
                return default(T);
            }
            return (T)Enum.Parse(typeof(T), str);
        }

		static public bool TryConvert<T>(int value, out T result)
		{
			result = default(T);
			bool success = Enum.IsDefined(typeof(T), value);
			if (success)
			{
				result = (T)Enum.ToObject(typeof(T), value);
			}
			return success;
		}
	}
}
using System;
using System.Linq;
using System.Reflection;
namespace System.Windows.Forms
{
	public static  class Util
	{
		public static string GetPropertyStringValue(object inObject, string propertyName)
		{
			PropertyInfo[] props = inObject.GetType().GetProperties();
			PropertyInfo prop = props.Select(p => p).Where(p =>  p.Name == propertyName).FirstOrDefault();
				if (prop != null)
					return prop.GetValue(inObject,null).ToString();
			return "";
		}
		
		public static object GetPropertyValue(object inObject, string propertyName)
		{
			PropertyInfo[] props = inObject.GetType().GetProperties();
			PropertyInfo prop = props.Select(p => p).Where(p =>  p.Name == propertyName).FirstOrDefault();
				if (prop != null)
					return prop.GetValue(inObject,null).ToString();
			return null;
		}
	}
}


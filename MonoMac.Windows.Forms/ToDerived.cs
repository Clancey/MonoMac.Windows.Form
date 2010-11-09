using System;
using System.Reflection;
namespace MonoMac.Windows.Forms
{
	public class ToDerived
	{
		public static TDerived ToDerived<TBase, TDerived>(TBase tBase, BindingFlags bindingFlags)
		    where TDerived : TBase, new()
		{
		    bool allowNonPublic = ((bindingFlags & BindingFlags.NonPublic) == 
							BindingFlags.NonPublic);
		    TDerived tDerived = new TDerived();
		
		    foreach (PropertyInfo propBase in typeof(TBase).GetProperties(bindingFlags))
		    {
		        PropertyInfo propDerived = typeof(TDerived).GetProperty
						(propBase.Name, bindingFlags);
		        propDerived.SetValue(tDerived, propBase.GetValue(tBase, null), null);
		    }
		    return tDerived;
		}

	}
}


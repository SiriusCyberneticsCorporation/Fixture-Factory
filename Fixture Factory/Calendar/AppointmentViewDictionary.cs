using System;
using System.Collections;

namespace Calendar
{
	/// <summary>
	/// Summary description for AppointmentViewDictionary.
	/// </summary>
	public class AppointmentViewDictionary : DictionaryBase  
	{

		public AppointmentView this[ Appointment key ]  
		{
			get  
			{
				return( (AppointmentView) Dictionary[key] );
			}
			set  
			{
				Dictionary[key] = value;
			}
		}

		public ICollection Keys  
		{
			get  
			{
				return( Dictionary.Keys );
			}
		}

		public ICollection Values  
		{
			get  
			{
				return( Dictionary.Values );
			}
		}

		public void Add( Appointment key, AppointmentView value )  
		{
			Dictionary.Add( key, value );
		}

		public bool Contains( Appointment key )  
		{
			return( Dictionary.Contains( key ) );
		}

		public void Remove( Appointment key )  
		{
			Dictionary.Remove( key );
		}
	}
}

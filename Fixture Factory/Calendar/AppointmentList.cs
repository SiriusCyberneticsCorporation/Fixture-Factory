using System;
using System.Collections;

namespace Calendar
{
	/// <summary>
	/// Summary description for AppointmentList.
	/// </summary>
	public class AppointmentList : CollectionBase  
	{

		public Appointment this[ int index ]  
		{
			get  
			{
				return( (Appointment) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		public int Add( Appointment value )  
		{
			return( List.Add( value ) );
		}

		public int IndexOf( Appointment value )  
		{
			return( List.IndexOf( value ) );
		}

		public void Insert( int index, Appointment value )  
		{
			List.Insert( index, value );
		}

		public void Remove( Appointment value )  
		{
			List.Remove( value );
		}

		public bool Contains( Appointment value )  
		{
			// If value is not of type Int16, this will return false.
			return( List.Contains( value ) );
		}
	}		
}

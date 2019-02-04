/* Developed by Ertan Tike (ertan.tike@moreum.com) */

using System;
//using System.Collections.Generic;
using System.Text;

namespace Calendar
{
    public class ResolveAppointmentsEventArgs : EventArgs
    {
        public ResolveAppointmentsEventArgs(DateTime start, DateTime end)
        {
            m_StartDate = start;
            m_EndDate = end;
            m_Appointments = new AppointmentList();
		
			Appointment appointment = new Appointment();
			appointment.Title = "Appointment 1";
			appointment.StartDate = new DateTime ( 2007, 4, 5, 9, 0, 0);
			appointment.EndDate = new DateTime ( 2007, 4, 5, 10, 0, 0);
			m_Appointments.Add ( appointment );
		}

        private DateTime m_StartDate;

        public DateTime StartDate
        {
            get { return m_StartDate; }
            set { m_StartDate = value; }
        }

        private DateTime m_EndDate;

        public DateTime EndDate
        {
            get { return m_EndDate; }
            set { m_EndDate = value; }
        }

        private AppointmentList m_Appointments;

        public AppointmentList Appointments
        {
            get { return m_Appointments; }
            set { m_Appointments = value; }
        }
    }

    public delegate void ResolveAppointmentsEventHandler(object sender, ResolveAppointmentsEventArgs args);
}

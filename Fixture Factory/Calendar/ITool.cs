/* Developed by Ertan Tike (ertan.tike@moreum.com) */

using System;

namespace Calendar
{
    public interface ITool
    {
        DayView DayView
        {
            get;
            set;
        }

        void Reset();

        void MouseMove( System.Windows.Forms.MouseEventArgs e );
        void MouseUp( System.Windows.Forms.MouseEventArgs e );
        void MouseDown( System.Windows.Forms.MouseEventArgs e );
    }
}

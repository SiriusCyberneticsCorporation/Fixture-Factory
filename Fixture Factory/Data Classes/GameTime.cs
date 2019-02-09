using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixture_Factory.Data_Classes
{
	public class GameTime : IComparable
	{
		public Guid ID { get; set; }
		public int DayOfWeek { get; set; }
		public DateTime StartTime { get; set; }
		public int Priority { get; set; }

		public override string ToString()
		{
			return ((System.DayOfWeek)DayOfWeek).ToString().Substring(0, 3) + " - " + StartTime.ToString("HH:mm");
		}

		public int CompareTo(object obj)
		{
			return Comparison(this, (GameTime)obj);
		}
		public static bool operator <(GameTime gameTime1, GameTime gameTime2)
		{
			return Comparison(gameTime1, gameTime2) < 0;
		}
		public static bool operator >(GameTime gameTime1, GameTime gameTime2)
		{
			return Comparison(gameTime1, gameTime2) > 0;
		}
		public static bool operator ==(GameTime gameTime1, GameTime gameTime2)
		{
			return Comparison(gameTime1, gameTime2) == 0;
		}
		public static bool operator !=(GameTime gameTime1, GameTime gameTime2)
		{
			return Comparison(gameTime1, gameTime2) != 0;
		}
		public override bool Equals(object obj)
		{
			if (!(obj is GameTime)) return false;
			return this == (GameTime)obj;
		}
		public static bool operator <=(GameTime gameTime1, GameTime gameTime2)
		{
			return Comparison(gameTime1, gameTime2) <= 0;
		}
		public static bool operator >=(GameTime gameTime1, GameTime gameTime2)
		{
			return Comparison(gameTime1, gameTime2) >= 0;
		}
		public override int GetHashCode()
		{
			return DayOfWeek.GetHashCode() ^ StartTime.Hour.GetHashCode() ^ StartTime.Minute.GetHashCode();
		}
		public static int Comparison(GameTime gameTime1, GameTime gameTime2)
		{
			// Sunday is greater than all other days
			int dow1 = gameTime1.DayOfWeek == 0 ? 7 : gameTime1.DayOfWeek;
			int dow2 = gameTime2.DayOfWeek == 0 ? 7 : gameTime2.DayOfWeek;

			if ((dow1 < dow2) ||
				(dow1 == dow2 && gameTime1.StartTime.Hour < gameTime2.StartTime.Hour) ||
				(dow1 == dow2 && gameTime1.StartTime.Hour == gameTime2.StartTime.Hour && gameTime1.StartTime.Minute < gameTime2.StartTime.Minute))
			{
				return -1;
			}
			else if (dow1 == dow2 && gameTime1.StartTime.Hour < gameTime2.StartTime.Hour && gameTime1.StartTime.Minute < gameTime2.StartTime.Minute)
			{
				return 0;
			}
			else if ((dow1 > dow2) ||
				(dow1 == dow2 && gameTime1.StartTime.Hour > gameTime2.StartTime.Hour) ||
				(dow1 == dow2 && gameTime1.StartTime.Hour == gameTime2.StartTime.Hour && gameTime1.StartTime.Minute > gameTime2.StartTime.Minute))
			{
				return 1;
			}
			return 0;
		}
	}
}

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
			return DayOfWeek.GetHashCode() ^ StartTime.GetHashCode();
		}
		public static int Comparison(GameTime gameTime1, GameTime gameTime2)
		{
			if ((gameTime1.DayOfWeek < gameTime2.DayOfWeek) ||
				(gameTime1.DayOfWeek == gameTime2.DayOfWeek && gameTime1.StartTime.Hour < gameTime2.StartTime.Hour) ||
				(gameTime1.DayOfWeek == gameTime2.DayOfWeek && gameTime1.StartTime.Hour == gameTime2.StartTime.Hour && gameTime1.StartTime.Minute < gameTime2.StartTime.Minute))
			{
				return -1;
			}
			else if (gameTime1.DayOfWeek == gameTime2.DayOfWeek && gameTime1.StartTime.Hour < gameTime2.StartTime.Hour && gameTime1.StartTime.Minute < gameTime2.StartTime.Minute)
			{
				return 0;
			}
			else if ((gameTime1.DayOfWeek > gameTime2.DayOfWeek) ||
				(gameTime1.DayOfWeek == gameTime2.DayOfWeek && gameTime1.StartTime.Hour > gameTime2.StartTime.Hour) ||
				(gameTime1.DayOfWeek == gameTime2.DayOfWeek && gameTime1.StartTime.Hour == gameTime2.StartTime.Hour && gameTime1.StartTime.Minute > gameTime2.StartTime.Minute))
			{
				return 1;
			}
			return 0;
		}
	}
}

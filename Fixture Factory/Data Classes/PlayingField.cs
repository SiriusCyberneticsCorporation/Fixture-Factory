using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixture_Factory.Data_Classes
{
	public class PlayingField
	{
		public Guid ID { get; set; }
		public string FieldName { get; set; }
		public int Priority { get; set; }
	}
}

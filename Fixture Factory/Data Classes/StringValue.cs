using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixture_Factory.Data_Classes
{
	public class StringValue
	{
		private string m_value = "";
		public StringValue() { }
		public StringValue(string value) { m_value = value; }
		public static implicit operator StringValue(string value)
		{
			return new StringValue(value);
		}
		public string Value { get { return m_value; } set { m_value = value; } }

		public override string ToString()
		{
			return m_value;
		}
	}
}

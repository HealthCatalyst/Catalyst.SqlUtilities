using System.Collections.Generic;

namespace ColumnExtractor.Models
{
		public class ParsedData
		{
				public List<string> databaselist = new List<string>();
				public List<string> schemalist = new List<string>();
				public List<string> tablelist = new List<string>();
				public List<string> columnlist = new List<string>();
				public List<string> errors = new List<string>();
		}
}
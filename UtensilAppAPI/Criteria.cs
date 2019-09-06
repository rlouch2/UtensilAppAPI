using System;
using System.Collections.Generic;
using System.Text;

namespace UtensilAppAPI
{
	public class Criteria : ICriteria
	{
		public string filter { get; set; }
		public Criteria(string field, string value)
		{
			filter = field + "=" + value;
		}

		public string ToQueryString()
		{
			//CriteriaGroup group = new CriteriaGroup(this);
			//return group.ToQueryString(); 
			return filter;
		}
	}

	public class CriteriaGroup : ICriteria
	{
		public List<string> filters { get; private set; }

		public CriteriaGroup(params ICriteria[] criterias)
		{
			this.filters = new List<string>();
			foreach(Criteria crit in criterias)
			{
				this.filters.Add(crit.filter);
			}		
		}

		public string ToQueryString()
		{
			return String.Join("&", filters);
		}
	}



	public interface ICriteria
	{
		string ToQueryString();
	}
}

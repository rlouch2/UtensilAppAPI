using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTestProject1
{
	[TestClass]
	public class UnitTest1
	{
		private string UtensilURL = "http://app.utensilapp.com/api/v1/";
		private string UtensilToken = "8f0c3b1da46a3f8034348db966ed78af6ab38b49";

		[TestMethod]
		public void TestSelectAllFacilities()
		{
			UtensilAppAPI.Utensil_ws ws = new UtensilAppAPI.Utensil_ws(UtensilURL, UtensilToken, "");

			dynamic utenFacilities = ws.SelectAll("facilities");


			utenFacilities[0].id = "1q234";
		}

		[TestMethod]
		public void TestSelectMealsForFacility()
		{
			UtensilAppAPI.Utensil_ws ws = new UtensilAppAPI.Utensil_ws(UtensilURL, UtensilToken, "");

			UtensilAppAPI.Criteria critDate = new UtensilAppAPI.Criteria("date", DateTime.Now.ToString("yyyy-MM-dd"));
			UtensilAppAPI.Criteria critMealName = new UtensilAppAPI.Criteria("name", "lunch");

			dynamic Meals = ws.Select("facilities", "671e3760-ddc0-47d0-b424-a817882cbbd0", "meals", new UtensilAppAPI.CriteriaGroup(critDate, critMealName));
		}
	}
}

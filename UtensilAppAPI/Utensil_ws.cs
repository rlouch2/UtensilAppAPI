using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UtensilAppAPI
{
	public class Utensil_ws
	{
		#region Properties
		private string baseURL { get; set; }
		public HttpStatusCode LastStatus { get; set; }
		public string LastResponse { get; set; }
		public string LastRequest { get; set; }
		public string LastURL { get; private set; }
		public string Utensil_URL { get; set; }
		public string Utensil_Token { get; set; }
		public string Utensil_Password { get; set; }
		#endregion

		#region Contsructors
		public Utensil_ws(string in_Utensil_URL)
		{
			Utensil_URL = in_Utensil_URL;

		}

		public Utensil_ws(string in_Utensil_URL, string in_Utensil_Token, string in_Utensil_Password)
			: this(in_Utensil_URL)
		{
			this.Utensil_Token = in_Utensil_Token;
			this.Utensil_Password = in_Utensil_Password;
		}
		#endregion

		#region Select

		public dynamic[] SelectAll(string tableName)
		{
			if (tableName == null)
				throw new ArgumentNullException("tableName");

			string UtensilDataArray = "starting array";
			List<dynamic> results = new List<dynamic>();

			results.AddRange(PerformSelect(tableName, "", "", "", out UtensilDataArray));
			return results.ToArray();
		}


		public dynamic[] Select(string tableName, ICriteria criteria = null)
		{
			if (tableName == null)
				throw new ArgumentNullException("tableName");

			List<dynamic> results = new List<dynamic>();
			string UtensilDataArray = "starting array";


			results.AddRange(PerformSelect(tableName, "", "", criteria.ToQueryString(), out UtensilDataArray));

			return results.ToArray();
		}

		public dynamic[] Select(string tableName, string id, ICriteria criteria = null)
		{
			if (tableName == null)
				throw new ArgumentNullException("tableName");

			List<dynamic> results = new List<dynamic>();
			string UtensilDataArray = "starting array";
			results.AddRange(PerformSelect(tableName, id, "", criteria.ToQueryString(), out UtensilDataArray));

			return results.ToArray();
		}

		public dynamic[] Select(string tableName, string id, string subTable, ICriteria criteria)
		{
			if (tableName == null)
				throw new ArgumentNullException("tableName");

			List<dynamic> results = new List<dynamic>();
			string UtensilDataArray = "starting array";

			results.AddRange(PerformSelect(tableName, id, subTable, criteria.ToQueryString(), out UtensilDataArray));

			return results.ToArray();
		}

		private dynamic PerformSelect(string tableName, string uuid, string subTable, string filters, out string dataReturn)
		{
			List<object> urlBits = new List<object> { tableName };

			if (uuid != "")
			{
				urlBits.Add(uuid);
			}

			if (subTable != "")
			{
				urlBits.Add(subTable);
			}

			string finalUrl = String.Join("/", urlBits);

			if (filters != "")
				finalUrl += "?" + filters;

			HttpStatusCode statusCode = PerformRequest(finalUrl, HttpMethod.Get, "", out string result);

			if (statusCode == HttpStatusCode.OK)
			{
				dataReturn = result;
				return JsonConvert.DeserializeObject<dynamic>(result);
			}
			else
			{
				dataReturn = "";
				return null;
			}
		}

		#endregion

		#region Delete
		public bool Delete(string tableName, string id)
		{
			if (tableName == null)
				throw new ArgumentNullException("tableName");

			string url = tableName + "/" + id;

			string result;
			HttpStatusCode status = PerformRequest(url, HttpMethod.Delete, "", out result);

			if (status == HttpStatusCode.OK)
				return true;
			else
				return false;
		}
		#endregion

		#region Update
		public bool Update(string tableName, string id, string json)
		{
			string url = tableName + "/" + id;
			string result;

			HttpStatusCode status = PerformRequest(url, HttpMethod.Put, json, out result);

			if (status == HttpStatusCode.OK)
				return true;
			else
				return false;
		}
		#endregion


		#region PrivateHelpers
		private void SetBasicAuthHeader(WebRequest request, String userName, String userPassword)
		{
			String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(userName + ":" + userPassword));
			request.Headers.Add("Authorization", "Basic " + encoded);
		}

		private HttpStatusCode PerformRequest(string url, HttpMethod method, string postJSON, out string result)
		{
			result = null;
			HttpStatusCode status = HttpStatusCode.BadRequest;

			Uri requestUri = new Uri(Utensil_URL + url);

			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requestUri);
			SetBasicAuthHeader(req, Utensil_Token, Utensil_Password);

			req.ContentType = "application/json";
			req.Accept = "application/json";
			req.Method = method.ToString();

			if (postJSON != "")
			{
				byte[] byteArray = Encoding.UTF8.GetBytes(postJSON);
				req.ContentLength = byteArray.Length;
				Stream stream = req.GetRequestStream();
				stream.Write(byteArray, 0, byteArray.Length);
				stream.Close();
			}

			try
			{
				HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
				using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
				{
					string RespStream = sr.ReadToEnd();
					result = RespStream;
					//result = JsonConvert.DeserializeObject<eMaintResult>(RespStream);
					status = resp.StatusCode;
				}
			}
			catch (WebException ex)
			{
				using (var exStream = ex.Response.GetResponseStream())
				using (var exReader = new StreamReader(exStream))
				{
					//result = exReader.ReadToEnd();
					result = null;
					status = HttpStatusCode.BadRequest;
				}
			}
			catch (Exception ex)
			{
				LastResponse = "Web service call failed with - " + ex;
			}

			this.LastURL = url;
			this.LastStatus = status;
			this.LastRequest = postJSON;
			this.LastResponse = result;

			return status;

		}


		#endregion
	}
}

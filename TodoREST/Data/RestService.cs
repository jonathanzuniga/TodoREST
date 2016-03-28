using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;

namespace TodoREST
{
	public class RestService : IRestService
	{
		HttpClient client;

		public List<TodoItem> Items { get; private set; }

		public RestService ()
		{
			var authData = string.Format ("{0}:{1}", Constants.Username, Constants.Password);
			var authHeaderValue = Convert.ToBase64String (Encoding.UTF8.GetBytes (authData));
//			var handler = new HttpClientHandler {
//				Credentials = new NetworkCredential(Constants.Username, Constants.Password)
//			};

			// Use username/password credentials.
			client = new HttpClient ();
//			client = new HttpClient (handler);
			client.MaxResponseContentBufferSize = 256000;
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Basic", authHeaderValue);
		}

		public async Task<List<TodoItem>> RefreshDataAsync ()
		{
			Items = new List<TodoItem> ();

			var uri = new Uri (string.Format (Constants.RestUrl, string.Empty));

			try {
				var response = await client.GetAsync (uri);

				// Check status code.
				Debug.WriteLine("Response status code: " + (int)response.StatusCode);

				if (response.IsSuccessStatusCode) {
					var content = await response.Content.ReadAsStringAsync ();
					Items = JsonConvert.DeserializeObject <List<TodoItem>> (content);
				}
			} catch (Exception ex) {
				Debug.WriteLine (@"				ERROR {0}", ex.Message);
			}

			return Items;
		}

		class CustomNamesContractResolver : DefaultContractResolver
		{
			protected override IList<JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization)
			{
				// Let the base class create all the JsonProperties 
				// using the short names.
				IList<JsonProperty> list = base.CreateProperties(type, memberSerialization);

				// Now inspect each property and replace the 
				// short name with the real property name.
				foreach (JsonProperty prop in list) {
					prop.PropertyName.ToLower ();

//					if (prop.UnderlyingName == "ID") // Change this to your implementation!
//						prop.PropertyName = "id";
					
//					switch (prop.UnderlyingName) {
//						case "ID":
//							prop.PropertyName = "id";
//							break;
//						case "Name":
//							prop.PropertyName = "name";
//							break;
//						case "Notes":
//							prop.PropertyName = "notes";
//							break;
//						case "Done":
//							prop.PropertyName = "done";
//							break;
//					}
				}

				return list;
			}
		}

		public async Task SaveTodoItemAsync (TodoItem item, bool isNewItem = false)
		{
			var itemId = isNewItem ? string.Empty : item.ID.ToString();
			var uri = new Uri (string.Format (Constants.RestUrl, itemId));

			try {
				JsonSerializerSettings settings = new JsonSerializerSettings();
				settings.Formatting = Formatting.None;
				settings.ContractResolver = new CustomNamesContractResolver();

				var json = JsonConvert.SerializeObject (item, settings);
				var content = new StringContent (json, Encoding.UTF8, "application/json");

				HttpResponseMessage response = null;
				if (isNewItem) {
					response = await client.PostAsync (uri, content);
				} else {
					response = await client.PutAsync (uri, content);
				}

				if (response.IsSuccessStatusCode) {
					Debug.WriteLine (@"				TodoItem successfully saved.");
				}

			} catch (Exception ex) {
				Debug.WriteLine (@"				ERROR {0}", ex.Message);
			}
		}

		public async Task DeleteTodoItemAsync (int id)
		{
			var uri = new Uri (string.Format (Constants.RestUrl, id));

			try {
				var response = await client.DeleteAsync (uri);

				if (response.IsSuccessStatusCode) {
					Debug.WriteLine (@"				TodoItem successfully deleted.");	
				}

			} catch (Exception ex) {
				Debug.WriteLine (@"				ERROR {0}", ex.Message);
			}
		}
	}
}

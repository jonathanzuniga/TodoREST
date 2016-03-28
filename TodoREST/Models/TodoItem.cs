using System;

using Newtonsoft.Json;

namespace TodoREST
{
	public class TodoItem
	{
		[JsonProperty(PropertyName = "id")]
		public int ID { get; set; }

		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "notes")]
		public string Notes { get; set; }

		[JsonProperty(PropertyName = "done"), JsonConverter(typeof(BoolConverter))]
		public bool Done { get; set; }
	}
}

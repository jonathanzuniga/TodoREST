using System;

namespace TodoREST
{
	public static class Constants
	{
		// URL of REST service.
		public static string RestUrl = "http://192.168.0.10:8000/api/v1/item/{0}";

		// Credentials that are hard coded into the REST service.
		public static string Username = "tester1";
		public static string Password = "tester1";
	}
}

using Newtonsoft.Json;

namespace Shopping_Tutorial.Repository
{
	public static class SessionExtensions
	{
		public static void SetJson(this ISession session, string key, object value)// dang ky session
		{
			session.SetString(key, JsonConvert.SerializeObject(value));
		}

		public static T GetJson<T>(this ISession session, string key)
		{
			var sessionData = session.GetString(key);
			return sessionData == null ? default(T) : JsonConvert.DeserializeObject<T>(sessionData);// change to Json
		}
	}
}

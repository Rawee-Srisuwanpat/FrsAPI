using field_recording_api.Models.HttpModel;
using System.Dynamic;

namespace field_recording_api.Utilities
{
    public static class Util
    { 
        public static bool HasAttr(ExpandoObject expando, string key)
        {
            return ((IDictionary<string, Object>)expando).ContainsKey(key);
        }

        public static Func<string, string, ResponseContext> responseError = (statusCode, message) => new ResponseContext() { statusCode = statusCode, message = message };

    }
}

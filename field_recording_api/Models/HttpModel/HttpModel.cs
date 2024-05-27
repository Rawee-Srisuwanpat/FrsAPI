namespace field_recording_api.Models.HttpModel
{
    public class ResponseContext {
       
        public string statusCode { get; set; } = "200";
        public object data { get; set; }
        public string message { get; set; } = string.Empty;
    }

    public class HttpOption { 
        public string? token { get; set; }
        public string? timeouts { get; set; }
    }
}

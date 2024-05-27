using field_recording_api.Helpers.JWT;
using field_recording_api.Utilities;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace field_recording_api.Middleware
{/// <summary>
///   ไม่ได้ใช้
/// </summary>
    //
    public class TraceIdMiddleware
    {
        private readonly RequestDelegate _next;

        public TraceIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            
            try
            {
                //context.TraceIdentifier = Guid.NewGuid().ToString();
                //string id = context.TraceIdentifier;
                //context.Response.Headers["TraceId"] = id;
                //var aaa =log4net.ThreadContext.Properties["TraceId"];
                //log4net.ThreadContext.Properties["TraceId"] = Guid.NewGuid();
                //aaa = log4net.ThreadContext.Properties["TraceId"];
                //var token = context.Request.Headers[HeaderNames.Authorization].ToString();
                ////Log4netCustomLayout.traceid = Guid.NewGuid();
                ////Log4netCustomLayout.user = "";
                //if (!string.IsNullOrEmpty(token))
                //{
                //    var userData = JWT.DecryptionToken(token.Replace("Bearer ", ""));
                //    log4net.ThreadContext.Properties["usrname"] = userData.usrname;
                //    //Log4netCustomLayout.user = userData.usrname;
                //}

                await _next(context);
            } 
            catch (Exception ex) 
            {
                await _next(context);
            } 
        }
    }
}

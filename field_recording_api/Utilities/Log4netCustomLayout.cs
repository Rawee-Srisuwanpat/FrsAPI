using field_recording_api.Services.File;
using log4net.Core;
using log4net.Layout;
using log4net.Layout.Pattern;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace field_recording_api.Utilities
{
    public class Log4netCustomLayout : PatternLayoutConverter
    {
        public Guid traceid;
        public static string user;
        #region Methods
        public Log4netCustomLayout() {
            traceid = Guid.NewGuid();
        }

        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            //if (loggingEvent.MessageObject is string stringMessage)
            //{
            //    writer.WriteLine(new { 
            //        traceId = traceid,//transactionid,
            //        date = DateTime.Now.ToString("dd/MM/yyyy: HH:mm:ss"), 
            //        message = stringMessage, 
            //        user = loggingEvent.UserName
            //    });
            //}
            //else
            //{
            //    writer.Write(loggingEvent.RenderedMessage);
            //}

            writer.WriteLine((loggingEvent.MessageObject is string stringMessage) ? stringMessage : loggingEvent.RenderedMessage);
            //writer.WriteLine(new
            //{
            //    //traceId = log4net.ThreadContext.Properties["TraceId"],
            //    //date = DateTime.Now.ToString("dd/MM/yyyy: HH:mm:ss"),
            //    message = (loggingEvent.MessageObject is string stringMessage) ? stringMessage : loggingEvent.RenderedMessage,
            //    //user = log4net.ThreadContext.Properties["usrname"]//loggingEvent.UserName
            //});
        }

        public static class http
        {
            private static IConfiguration config;
            public static IConfiguration Configuration
            {
                get
                {
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");
                    config = builder.Build();
                    return config;
                }
            }
        }

        #endregion
    }

}

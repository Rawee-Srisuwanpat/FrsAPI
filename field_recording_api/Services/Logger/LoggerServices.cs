using Azure.Core;
using field_recording_api.DataAccess.FieldRecording;
using field_recording_api.Helpers.JWT;
using field_recording_api.Models.AuthenticationModel;
using field_recording_api.Models.FileModel;
using field_recording_api.Models.HttpModel;
using field_recording_api.Services.File;
using field_recording_api.Utilities;
using log4net;
using log4net.Core;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;

namespace field_recording_api.Services.Logger
{
    public class LoggerServices : ILoggerServices
    {
        private readonly ILog _logger;
        private readonly IRepository<TLog> _tLog;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Guid tracId;
        public LoggerServices(
            ILog logger,
            IRepository<TLog> tLog,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _tLog = tLog;
            _httpContextAccessor = httpContextAccessor;
            tracId = Guid.NewGuid();
        }
        public async Task dblog(string MessageCode, object InputData, object OutputData, object? remak = null, [CallerMemberName] string methodName = "")
        {
            try
            {
                var user = getbodyToken();
                var logging = new TLog();
                logging.Id = Guid.NewGuid();
                logging.ControllerName = _httpContextAccessor.HttpContext.Request.RouteValues["controller"].ToString() ?? "";
                logging.ActionName = _httpContextAccessor.HttpContext.Request.RouteValues["action"].ToString() ?? "";
                logging.MethodName = methodName;
                logging.InputData = JsonConvert.SerializeObject(InputData);
                logging.OutputData = JsonConvert.SerializeObject(OutputData);
                logging.MessageCode = MessageCode;
                //logging.MessageThaDesc = data.MessageThaDesc;
                //logging.MessageEngDesc = data.MessageEngDesc;
                logging.CreatedBy = user.usrname;
                logging.CreatedDate = DateTime.Now;
                logging.UpdatedBy = user.usrname;
                logging.UpdatedDate = DateTime.Now;
                logging.IsActive = true;
                logging.TransId = tracId;//Guid.NewGuid();
                logging.Remark = (remak != null) ? JsonConvert.SerializeObject(OutputData) : "";
                //logging.ContractNo = "";// data.ContractNo;
                logging.UserName = user.usrname;
                await _tLog.AddAsync(logging);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat("Service setCollection Error: {0}", ex.Message);
                throw new Exception(string.Format("LoggerServices Error: {0}", ex.Message));
            }
        }

        public void Info(string message)
        {
            var user = getbodyToken();          
            _logger.InfoFormat("{0}", JsonConvert.SerializeObject(new
            {
                traceId = tracId,//log4net.ThreadContext.Properties["TraceId"],
                date = DateTime.Now.ToString("dd/MM/yyyy: HH:mm:ss"),
                message,
                user = user.usrname
            }));
        }

        private AuthenticationModel getbodyToken() {
            var token = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization].ToString();
            var user = new AuthenticationModel();
            if (!string.IsNullOrEmpty(token))
                user = JWT.DecryptionToken(_httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", ""));
            return user;
        }
    }
}

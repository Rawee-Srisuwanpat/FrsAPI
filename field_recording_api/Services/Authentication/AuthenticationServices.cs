using field_recording_api.Helpers.JWT;
using field_recording_api.Models.AuthenticationModel;
using field_recording_api.Models.HttpModel;
using field_recording_api.Services.Logger;
using field_recording_api.Utilities;
using log4net;
using Newtonsoft.Json;
using System.Dynamic;

namespace field_recording_api.Services.Authentication
{
    public class AuthenticationServices: IAuthentication
    {
        private readonly ILoggerServices _logService;
        private readonly IConfiguration _config;
        public AuthenticationServices(
            ILoggerServices logService,
            IConfiguration config
        ) {
            _logService = logService;
            _config = config;
        }

        public async Task<ResponseContext> authen(AuthenticationModel Req) {
            var _resview = new ResponseContext();
            try
            {
                _logService.Info(string.Format("Service authen: Start: {0}", JsonConvert.SerializeObject(Req)));
                var getTokenUrl = _config.GetSection("ApiSetting:FieldRecordingToken").Value;
                dynamic body = new ExpandoObject();
                body.JWTUsername = _config.GetSection("FieldRecordingAccount:user").Value;
                body.JWTPassword = _config.GetSection("FieldRecordingAccount:password").Value;

                _logService.Info("Url : " + getTokenUrl);
                _logService.Info("body : " + body);

                var getTokenResponse = await CallApi.post(getTokenUrl, body);
                if (getTokenResponse.statusCode != "200")
                {

                    await _logService.dblog(_resview.statusCode, body, getTokenResponse);
                    return getTokenResponse;
                }
                _logService.Info("statusCode : " + getTokenResponse.statusCode);


                _logService.Info("End call : " + getTokenUrl);


                dynamic resultObj = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(getTokenResponse.data);
                if (resultObj == null || !Util.HasAttr(resultObj, "token_val"))
                {
                    _resview.data = resultObj;
                    throw new Exception("Cannot get Token from FieldRecordig");
                }

                var verifyTokenUrl = _config.GetSection("ApiSetting:FieldRecordingVerify").Value;
                body = new ExpandoObject();
                body.loginid = Req.loginid;
                body.username = _config.GetSection("FieldRecordingAccount:projectname").Value;
                var verifyTokenResponse = await CallApi.post(verifyTokenUrl, body, new HttpOption { token = resultObj.token_val });
                if (verifyTokenResponse.statusCode != "200") {


                    await _logService.dblog(_resview.statusCode, body, verifyTokenResponse);
                    return verifyTokenResponse;
                }

                resultObj = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(verifyTokenResponse.data);

                if (!string.IsNullOrEmpty(ValidatesResponse(resultObj)))
                {
                    _resview.data = resultObj;
                    throw new Exception(ValidatesResponse(resultObj));
                }

                _resview.data = JWT.GenerateToken(Req);
                _logService.Info("Service authen: End");

                _logService.Info("Before out :"+ _resview.data.ToString());
            }
            catch (Exception ex)
            {
                _resview.statusCode = "201";
                _resview.message = string.Format("authen Error => {0}", ex.Message);
                _logService.Info(string.Format("Service authen Error: {0}", ex.Message));
                await _logService.dblog("201", Req, _resview);
            }
            return _resview;
        }

        private string ValidatesResponse(dynamic obj)
        {
            var resultCode = (obj == null || !Util.HasAttr(obj, "msgcode")) ? "" : obj.msgcode;
            var resultMessage = (obj == null || !Util.HasAttr(obj, "msgth")) ? "" : obj.msgth;
            resultMessage = resultMessage != "" ? resultMessage : "Access denied: Your not have permisstion";
            return (resultCode != "200") ? resultMessage : "";
        }
    }
}

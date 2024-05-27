using Azure.Core;
using field_recording_api.Helpers.JWT;
using field_recording_api.Models.AccountDetailModel;
using field_recording_api.Models.ActionCodeModel;
using field_recording_api.Models.FollowUp;
using field_recording_api.Models.GetDropDownListMaster;
using field_recording_api.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;

namespace field_recording_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowUpController : ControllerBase
    {
        IFollowUpService followUpService;
        private static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public FollowUpController(IFollowUpService followUpService)
        {
            this.followUpService = followUpService;
        }


        [HttpPost("GetDropDownListMaster")]
        [AuthorizeAttribute]
        public async Task<ActionResult> GetDropDownListMaster()
        {

            var res = new GetDropDownListMasterResModel();

            try
            {
                if (!_cache.TryGetValue("GetDropDownListMaster", out res))
                {
                    GetDropDownListMasterReqModel req = new GetDropDownListMasterReqModel();
                    res = followUpService.GetDropdownList(req);

                    _cache.Set("GetDropDownListMaster", res, TimeSpan.FromMinutes(30));
                }



            }
            catch (Exception ex)
            {
                res.status_code = "500";
                res.status_text = ex.Message.ToString();
                //res.payload_action_code = null;
            }
            return Ok(res);
        }


        [HttpPost("SaveFollowUp")]
        [AuthorizeAttribute]
        public async Task<ActionResult> SaveFollowUp(FollowUpReqModel req)
        {

            var res = new FollowUpResModel();

            try
            {
                string? ip = getIP(this.Request);
                

                req.IPAddress = ip;

                res = followUpService.SaveFollowUp(req);

            }
            catch (Exception ex)
            {
                res.status_code = "500";
                res.status_text = ex.Message.ToString();
                res.payload = null;
            }
            return Ok(res);
        }

        private string getIP(HttpRequest request)
        {
            var ipAddress = request?.Headers?["X-Real-IP"].ToString();

            if (!string.IsNullOrEmpty(ipAddress))
            {
                return ipAddress;
            }

            ipAddress = request?.Headers?["X-Forwarded-For"].ToString();

            if (!string.IsNullOrEmpty(ipAddress))
            {
                var parts = ipAddress.Split(',');

                if (parts.Count() > 0)
                {
                    ipAddress = parts[0];
                }

                return ipAddress;
            }

            ipAddress = request?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            if (!string.IsNullOrEmpty(ipAddress))
            {
                return ipAddress;
            }

            return string.Empty;
        }


        [HttpPost("GetFilePdf")]
        public async Task<ActionResult> GetFilePdf()
        {

            var res = new FollowUpResModel();
            var dataStream = new MemoryStream();

            try
            {
                string? ip = getIP(this.Request);


                var dataBytes = System.IO.File.ReadAllBytes(@".\Pdf\dummy.pdf");
                dataStream = new MemoryStream(dataBytes);

               

            }
            catch (Exception ex)
            {
                res.status_code = "500";
                res.status_text = ex.Message.ToString();
                res.payload = null;
            }
            //return Ok(res);


            FileStream fs2 = new FileStream(@".\Pdf\dummy.pdf", FileMode.Open, FileAccess.Read);
            return File(dataStream, "application/pdf", "test.pdf");
            //return File(dataStream, "application/octet-stream", "test.pdf");
        }

        //[HttpPost("GetFilePdf2")]
        //public async Task<ActionResult> GetFilePdf2()
        //{
        //    var jsonString = JsonConvert.SerializeObject(new { test = "123" });
        //    return new MemoryStream(Encoding.Default.GetBytes(jsonString));
        //}
    }
}

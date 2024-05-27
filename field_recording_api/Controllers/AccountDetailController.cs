using Azure;
using field_recording_api.Helpers.JWT;
using field_recording_api.Models.AccountDetailModel;
using field_recording_api.Models.HttpModel;
using field_recording_api.Services.AccountDetail;
using field_recording_api.Services.Logger;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace field_recording_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountDetailController : ControllerBase
    {
        private readonly IAccountDetailServices _accountDetailServices;
        private readonly ILoggerServices _logService;
        public AccountDetailController(IAccountDetailServices accountDetailServices , ILoggerServices logService)
        {
            _accountDetailServices = accountDetailServices;
            _logService = logService;
    }
        
        [HttpGet("SyncDataFile")]
        [AuthorizeAttribute]
        public async Task<ActionResult> SyncDataFile([FromQuery] CollecctionModel Dto)
        {
            var data = await _accountDetailServices.syncDataFile(Dto).ConfigureAwait(false);
            return Ok(data);
        }

        


        [HttpGet("GetCollections")]
        [AuthorizeAttribute]
        public async Task<ActionResult> GetCollections([FromQuery] CollecctionModel Dto)
        {
            var data = await _accountDetailServices.getCollections(Dto).ConfigureAwait(false);
            return Ok(data);
        }

        [HttpGet("GetCollectionsNew")]
        [AuthorizeAttribute]
        public async Task<ActionResult> GetCollectionsNew([FromQuery] CollecctionModel Dto)
        {
            var data = await _accountDetailServices.getCollectionsNew(Dto).ConfigureAwait(false);
            return Ok(data);
        }

        [HttpGet("SearchCollections")]
        [AuthorizeAttribute]
        public async Task<ActionResult> SearchCollections([FromQuery] SearchCollectionsModel Dto)
        {
            var data = await _accountDetailServices.searchCollections(Dto).ConfigureAwait(false);
            return Ok(data);
        }

        [HttpGet("SearchCollectionsNew")]
        [AuthorizeAttribute]
        public async Task<ActionResult> SearchCollectionsNew([FromQuery] SearchCollectionsModel Dto)
        {
            var data = await _accountDetailServices.searchCollectionsNew(Dto).ConfigureAwait(false);
            return Ok(data);
        }

        [HttpGet("GetAccountDetailData")]
        [AuthorizeAttribute]
        public async Task<ActionResult> GetAccountDetailData([FromQuery] AccountDetailModel Dto)
        {
            if (!ModelState.IsValid)
            {
                return new ObjectResult(new { Message = "Some Parameter Requried" })
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            var data = await _accountDetailServices.getAccountDetailData(Dto).ConfigureAwait(false);
            return Ok(data);
        }

        [HttpGet("GetAccountDetailDataNew")]
        [AuthorizeAttribute]
        public async Task<ActionResult> GetAccountDetailDataNew([FromQuery] AccountDetailModel Dto)
        {
            if (!ModelState.IsValid) {
                return new ObjectResult(new { Message = "Some Parameter Requried" })
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            var data = await _accountDetailServices.getAccountDetailDataNew(Dto).ConfigureAwait(false);
            return Ok(data);
        }

        [HttpPost("saveAccountDetailData")]
        [AuthorizeAttribute]
        public async Task<ActionResult> saveAccountDetailData([FromForm] SaveAccountDetailModel Dto)
        {
           //  throw new NotImplementedException("ddd");
            //return BadRequest("aaaa");
            //var data = await _accountDetailServices.saveAccountDetailData(Dto).ConfigureAwait(false);

            

            try
            {
                //throw new Exception("รูปภาพมีขนาด 0 byte กรุณาอัปโหลดรูปภาพใหม่");
                foreach (IFormFile postedFile in Dto.inputFile)
                {
                    if (postedFile.Length == 0) throw new Exception("รูปภาพมีขนาด 0 byte กรุณาอัปโหลดรูปภาพใหม่");
                }

                if (Dto.latlong == null) {
                    throw new Exception("lat long is null");
                }

                if (Dto.contract_no == null )
                {
                    throw new Exception("contract_no is null");
                }

                if (Dto.action == null)
                {
                    throw new Exception("action is null");
                }

                if (Dto.result == null)
                {
                    throw new Exception("result is null");
                }


                var data = await _accountDetailServices.saveAccountDetailData(Dto).ConfigureAwait(false);
                return Ok(data);
            }
            catch (Exception ex) 
            {
                var _resview = new ResponseContext();

                _resview.statusCode = "201";
               // _resview.message = "หากระบบเกิดข้อผิดพลาด รบกวนติดต่อ IT Helpdesk ที่เบอร์ 02-107-2222 ต่อ 3111, 3222 ทุกวัน เวลา 8.30 - 17.30 น. (ยกเว้นวันหยุดนักขัตฤกษ์)";

                _resview.message = ex.Message;

                 await _logService.dblog("201", Dto, string.Format("saveAccountDetailData Error => {0}", ex.ToString()) );

                return Ok(_resview);
            }
            
        }


        [HttpPost("getLocationTracking")]
        [AuthorizeAttribute]
        public async Task<ActionResult> getLocationTracking(LocationTrackingReq req)
        {
            LocationTracking Dto = new LocationTracking() { CreatedBy = req.CreatedBy.ToUpper()} ;
            var data = await _accountDetailServices.getLocationTracking(Dto).ConfigureAwait(false);
            return Ok(data);
        }

        [HttpGet("AddLocationTracking")]
        //[AuthorizeAttribute] ต้องไม่เปิด 
        public async Task<ActionResult> AddLocationTracking([FromQuery]AddLocationTrackingReq Dto)
        {
            var data = await _accountDetailServices.AddLocationTracking(Dto).ConfigureAwait(false);
            return Ok(data);
        }


    }
}

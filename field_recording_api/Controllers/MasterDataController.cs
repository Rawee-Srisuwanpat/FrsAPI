using field_recording_api.Helpers.JWT;
using field_recording_api.Models.ConfigCloseSystem;
using field_recording_api.Models.GetDropDownListMaster;
using field_recording_api.Models.HttpModel;
using field_recording_api.Services.Implement;
using field_recording_api.Services.MasterDataServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace field_recording_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasterDataController : ControllerBase
    {
        private static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        private readonly IMasterDataServices _masterDataServices;
        public MasterDataController(IMasterDataServices masterDataServices)
        {
            _masterDataServices = masterDataServices;
        }

        [HttpGet("getDropdownMaster")]
        [ResponseCache(Duration = 60)]
        [AuthorizeAttribute]
        public async Task<ActionResult> GetDropdownMaster()
        {
            ResponseContext data = new ResponseContext();

            if (!_cache.TryGetValue("getDropdownMaster", out data))
            {
                //var data = await _masterDataServices.getDropdownMaster().ConfigureAwait(false);

                 data = await _masterDataServices.getDropdownMaster().ConfigureAwait(false);
                _cache.Set("getDropdownMaster", data, TimeSpan.FromMinutes(30));

            }
            return Ok(data);
        }

        [HttpPost("getConfigCloseSystem")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult> GetConfigCloseSystem(ConfigCloseSystemReqModel req)
        {
            var res = new Models.ConfigCloseSystem.ConfigCloseSystemResModel();

            try
            {
                //if (!_cache.TryGetValue("getConfigCloseSystem", out res))
                {
                    res =  _masterDataServices.GetConfigCloseSystem(req);

                   // _cache.Set("getConfigCloseSystem", res, TimeSpan.FromMinutes(30));
                }

            }
            catch (Exception ex)
            {
                res.status_code = "500";
                res.status_text = ex.Message.ToString();
                res.payload = null;
            }
            return Ok(res);

        }

    }
}

using field_recording_api.Models.AuthenticationModel;
using field_recording_api.Services.Authentication;
using field_recording_api.Services.Logger;
using Microsoft.AspNetCore.Mvc;

namespace field_recording_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthentication _authenticationServices;
        private readonly ILoggerServices _logService;

        public AuthenticationController(IAuthentication authenticationServices,  ILoggerServices _logService)
        {
            _authenticationServices = authenticationServices;
            this._logService = _logService;
    }

        [HttpPost("VertifyToken")]
        public async Task<ActionResult> VertifyToken([FromBody] AuthenticationModel Dto)
        {
            var data = await _authenticationServices.authen(Dto).ConfigureAwait(false);

            _logService.Info("message : " +data.message);
            return Ok(data);
        }
    }
}

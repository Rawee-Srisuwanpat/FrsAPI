using field_recording_api.Models.FileModel;
using field_recording_api.Services.File;
using Microsoft.AspNetCore.Mvc;
using field_recording_api.Helpers.JWT;

namespace field_recording_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileServices _fileServices;
        public FileController(IFileServices fileServices)
        {
            _fileServices = fileServices;
        }

        [HttpPost("Upload")]
        [AuthorizeAttribute]
        public async Task<ActionResult> Upload([FromForm] FileModel Dto)
        {
            var data = await _fileServices.Upload(Dto).ConfigureAwait(false);
            return Ok(data);
        }
    }
}
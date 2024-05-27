using System.ComponentModel.DataAnnotations;

namespace field_recording_api.Models.ConfigCloseSystem
{
    public class ConfigCloseSystemReqModel
    {
        [Required(ErrorMessage = "Please add StartDate to the request.")]
        public string dateFromMobile { get; set; }
    }
}

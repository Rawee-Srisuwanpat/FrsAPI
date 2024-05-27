using field_recording_api.Models.ActionCodeModel;

namespace field_recording_api.Models.ConfigCloseSystem
{
    public class ConfigCloseSystemResModel
    {
        public string status_code { get; set; }
        public string status_text { get; set; }
        public IReadOnlyList<ConfigCloseSystemDtoModel>? payload { get; set; }
    }
}

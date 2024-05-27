using System.ComponentModel.DataAnnotations;

namespace field_recording_api.Models.AccountDetailModel
{
    public class AddLocationTrackingReq
    {
        [Required]
        public string username  { get; set; }
        
        [Required]
        public string lat       { get; set; }
       
        [Required]
        public string lng       { get; set; }
        
        [Required]
        public string token { get; set; }
    }
}

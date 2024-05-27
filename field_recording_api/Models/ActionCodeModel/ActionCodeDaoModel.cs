using System.ComponentModel.DataAnnotations;

namespace field_recording_api.Models.ActionCodeModel
{
    public class ActionCodeDaoModel
    {
        [Key]
        public int ID { get ; set ; }
        public string? Action_Code { get ; set ; }
        public string? DescriptionTH { get ; set ; }
        public string? DescriptionEN { get ; set ; }
       
        
        public string? IsActive { get ; set ; }
        public string? CreateBy { get ; set ; }
        public DateTime? CreateDate { get ; set ; }
        public string? UpdateBy { get ; set ; }
        public DateTime? UpdateDate { get ; set ; }

    }
}

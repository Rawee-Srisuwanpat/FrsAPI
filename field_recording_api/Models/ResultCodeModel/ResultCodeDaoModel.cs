using System.ComponentModel.DataAnnotations;

namespace field_recording_api.Models.ResultCodeModel
{
    public class ResultCodeDaoModel
    {
        [Key]
         public int ID { get ; set ;}
         public string? result_code { get ; set ;}
         public string? DescriptionTH { get ; set ;}
         public string? DescriptionEN { get ; set ;}
         public int? CountContract { get ; set ;}

        public string? PTPOptionYN { get ; set ;}
         public string? IsActive { get ; set ;}
         public string? CreateBy { get ; set ;}
         public DateTime? CreateDate { get ; set ;}
         public string? UpdateBy { get ; set ;}
         public DateTime? UpdateDate { get ; set ;}
    }
}

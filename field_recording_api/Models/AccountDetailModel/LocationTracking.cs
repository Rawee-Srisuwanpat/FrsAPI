namespace field_recording_api.Models.AccountDetailModel
{
    public class LocationTracking
    {
      public string? id { get ; set ; }
      public string? lat { get ; set ; }
      public string? lng { get ; set ; }
      public string? CreatedBy { get ; set ; }
      public DateTime? CreatedDate { get ; set ; }
      public string? UpdatedBy { get ; set ; }
      public DateTime? UpdatedDate { get ; set ; }
      public string? IsActive { get ; set ; }
    }

    public class LocationTrackingReq
    {
        
        public string? CreatedBy { get; set; }
    
    }


}

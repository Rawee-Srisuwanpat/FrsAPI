namespace field_recording_api.Models.ConfigCloseSystem
{
    public class tbt_special_day
    {
        public long? id { get ; set ; }
        public string? day_type_code { get ; set ; }
        public DateTime? date { get ; set ; }
        public string? desc { get ; set ; }
        public DateTime? create_date { get ; set ; }
        public DateTime? update_date { get ; set ; }
        public string? create_by { get ; set ; }
        public string? update_by { get ; set ; }
        public string? is_deleted { get ; set ; }
    }
}

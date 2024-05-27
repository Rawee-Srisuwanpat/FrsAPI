namespace field_recording_api.Models.ConfigCloseSystem
{
    public class ConfigCloseSystemDtoModel
    {
        public long id { get; set; }
        public string day_type_code { get; set; }
        public string message_close_system { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }
        public DateTime effecitive_date_from { get; set; }
        public DateTime? effecitive_date_to { get; set; }
        public string remind_time_mins { get; set; }
        public string remind_message { get; set; }
        public string message_save_after_closed { get; set; }
        public string is_active { get; set; }
        public string is_deleted { get; set; }
        
    }
}

namespace field_recording_api.Models.MasterData
{
    public class MasterData
    {
        public string statusCode { get; set; }
        public Data data { get; set; }
        public string message { get; set; }
    }


    public class Data
    {
        public List<ActionCode> ActionCode { get; set; }
        public List<ResutCode> ResutCode { get; set; }
        public List<ModelOfContact> ModelOfContact { get; set; }
        public List<PlaceOfContract> PlaceOfContract { get; set; }
        public List<PartyContracted> PartyContracted { get; set; }
    }

    public class ActionCode
    {
        public string text { get; set; }
        public string value { get; set; }
    }

    public class ModelOfContact
    {
        public string text { get; set; }
        public string value { get; set; }
    }

    public class PartyContracted
    {
        public string text { get; set; }
        public string value { get; set; }
    }

    public class PlaceOfContract
    {
        public string text { get; set; }
        public string value { get; set; }
    }

    public class ResutCode
    {
        public string text { get; set; }
        public string value { get; set; }
    }


}

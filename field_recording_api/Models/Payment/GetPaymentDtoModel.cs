namespace field_recording_api.Models.Payment
{
    public class GetPaymentDtoModel
    {
        public string contract_no          { get; set; }
        public string payment_barcode      { get; set; }
        public string payment_qr_code      { get; set; }
        public string statement_channel    { get; set; }

    }
}

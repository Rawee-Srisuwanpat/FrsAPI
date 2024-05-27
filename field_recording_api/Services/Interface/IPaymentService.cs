using field_recording_api.Models.Payment;

namespace field_recording_api.Services.Interface
{
    public interface IPaymentService
    {
        public GetPaymentResModel GetPaymentCode(GetPaymentReqModel req);
    }
}

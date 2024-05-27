using field_recording_api.Models.FollowUp;
using field_recording_api.Services.Implement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using field_recording_api.Helpers.JWT;
using field_recording_api.Models.Payment;
using field_recording_api.Services.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace field_recording_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        IPaymentService paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }



        [HttpPost("GetPaymentCode")]
        //[AuthorizeAttribute]
        public async Task<ActionResult> GetPaymentCode(GetPaymentReqModel req)
        {

            var res = new GetPaymentResModel();

            try
            {
                

                res = paymentService.GetPaymentCode(req);

            }
            catch (Exception ex)
            {
                res.status_code = "500";
                res.status_text = ex.Message.ToString();
                res.payload = null;
            }
            return Ok(res);
        }
    }
}

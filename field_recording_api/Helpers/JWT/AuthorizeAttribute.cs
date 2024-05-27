using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Net;

namespace field_recording_api.Helpers.JWT
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                JWT.ValidateToken(context.HttpContext);
                //throw new Exception();    
            }
            catch (Exception e)
            {
                context.Result = new UnauthorizedResult();
                //var problemDetails = new ProblemDetails
                //{
                //    Type = "https://example.com/probs/out-of-credit",
                //    Title = "อิอิอิอิอิอิ",
                //    Extensions = { { "traceId", Activity.Current?.Id } }
                //};
                //context.Result = new UnauthorizedObjectResult(problemDetails);//new BadRequestObjectResult(problemDetails);//ObjectResult(problemDetails);
                //context.Result = Problem("msg", statusCode: (int)HttpStatusCode.BadRequest);//new UnauthorizedResult();

                //throw new AttributeException(e.Message, HttpStatusCode.BadRequest)
                //{
                //    actionResult = new BadRequestResult()
                //};

                //context.Result = new BadRequestResult();// new BadRequestObjectResult("Reference number not supplied.");
                //new ObjectResult($"Permission 'อิอิอิอิ' is required.")
                //{
                //    StatusCode = (int)HttpStatusCode.BadRequest
                //};
            }
        }
    }

    public class AttributeException : Exception
    {
        public IActionResult actionResult = new OkResult();

        public HttpStatusCode StatusCode;

        public AttributeException(string message, HttpStatusCode statusCode) : base(message)
        {
            this.StatusCode = statusCode;
        }
    }
}

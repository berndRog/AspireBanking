using Asp.Versioning;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
namespace BankingApi.Controllers;

// "http://localhost:5010/banking/owners" = Endpoint

[ApiVersionNeutral] // <-- This is the key!
[ApiExplorerSettings(IgnoreApi = true)]
[ApiController]
public class ErrorsController : ControllerBase {

   #region methods
   // RFC 7807 Probelm Details
   [Route("/error")]
   [HttpGet]
   [ProducesDefaultResponseType]
   public IActionResult HandleError()
      => Problem();

   
   [Route("/error-development")]
   [HttpGet]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   [ProducesDefaultResponseType]
   public IActionResult HandleErrorDevelopment(
      [FromServices] IHostEnvironment hostEnvironment  
   ) {
      if (!hostEnvironment.IsDevelopment()) return NotFound();
      
      var exceptionHandlerFeature =
         HttpContext.Features.Get<IExceptionHandlerFeature>()!;
      
      // var values = exceptionHandlerFeature.RouteValues?.Values;
      // var text = string.Empty;
      // if(values != null) {
      //    foreach(var r in values)
      //       if(r != null) text += r + " ";
      // }
      // text += $"{exceptionHandlerFeature.Path} ... ";
      // text += $"{exceptionHandlerFeature?.Error.Message}";

      return Problem(
//       title: text,
         title: exceptionHandlerFeature?.Error.Message,
         instance: exceptionHandlerFeature?.Endpoint?.DisplayName,
         detail: exceptionHandlerFeature?.Error.StackTrace
      );
   }

   #endregion
}

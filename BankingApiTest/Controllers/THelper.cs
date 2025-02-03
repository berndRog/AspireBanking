using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace BankingApiTest.Controllers;
public static class THelper {
   
   private static (bool, T, S) EvalActionResult<T, S>(
      ActionResult<S?> actionResult
   ) where T : ObjectResult   // OkObjectResult 
     where S : class? {       // OwnerDto
      
      // Check if actionResult is of type ObjectResult
      actionResult.Result.Should().NotBeNull().And.BeOfType<T>();
      // and cast it to ObjectResult
      var result = (actionResult.Result as T)!; 
      
      // Check if value is not null
      result.Value.Should().NotBeNull();
      // and result.Value is of Type s, then cast it to S
      if (result.Value is S resultValue) {
         // return true and result:T and resultValue:S
         return (true, result, resultValue); 
      }
      // return false and result:T and default(S)
      return (false, result, default!);
   }
   
   // HttpStatusCode.Ok (200)
   public static void IsOk<T>(
      ActionResult<T?>? actionResult, 
      T expected
   ) where T : class? {
      // Check if actionResult is not null
      actionResult.Should().NotBeNull();
      
      // Check if actionResult! is of type OkObjectResult
      // and evaluate the result
      var(success, result, value) =  
         EvalActionResult<OkObjectResult, T?>(actionResult!);
      // Check if success is true
      success.Should().BeTrue();
      // Check if result.StatusCode is 200
      result.StatusCode.Should().Be(200);
      value.Should().NotBeNull().And.BeEquivalentTo(expected);
   }
   
   // HttpStatusCode.Ok (200)
   public static void IsEnumerableOk<T>(
      ActionResult<T>? actionResult, 
      T expected
   ) where T : class {
      // Check if actionResult is not null
      actionResult.Should().NotBeNull();
      
      // Check if actionResult! is of type OkObjectResult
      // and evaluate the result
      var(success, result, value) =  
         EvalActionResult<OkObjectResult, T>(actionResult!);
      // Check if success is true
      success.Should().BeTrue();
      // Check if result.StatusCode is 200
      result.StatusCode.Should().Be(200);
      value.Should().NotBeNull().And.BeEquivalentTo(expected);
   }

   
   // HttpStatusCode.Created (201)
   public static void IsCreated<T>(
      ActionResult<T>? actionResult, 
      T? expected
   )  where T : class {
      // Check if actionResult is not null
      actionResult.Should().NotBeNull();
      
      // Check if actionResult! is of type CreatedResult
      // and evaluate the result
      var(success, result, value) = 
         EvalActionResult<CreatedResult, T?>(actionResult!);
      
      // Check if success is true
      success.Should().BeTrue();
      
      // Check if result.StatusCode is 201
      result.StatusCode.Should().Be(201);
      
      // Check if value is not null and is equivalent to expected
      value.Should().NotBeNull().And.BeEquivalentTo(expected);
   }
   
   // HttpStatusCode.NoContent (204)
   public static void IsNoContent(
      IActionResult actionResult
   ) {
      actionResult.Should().NotBeNull();
      actionResult.Should().BeOfType<NoContentResult>();
   }
   
   // // HttpStatusCode.NotFound (404)
   // public static void IsNotFound(
   //    IActionResult actionResult
   // ) {
   //    actionResult.Should().NotBeNull();
   //    actionResult.Should().BeOfType<NotFoundResult>();
   // }
   
   // HttpStatusCode.NotFound (404)
   public static void IsNotFound<T>(
      ActionResult<T> actionResult
   ) {
      actionResult.Should().NotBeNull();
      actionResult.Result.Should().BeOfType<NotFoundObjectResult>();
   }
   
   // HttpStatusCode.Conflict (409)
   public static void IsConflict<T>(
      ActionResult<T> actionResult
   ) where T : class {
      
      // Check if actionResult is not null
      actionResult.Should().NotBeNull();
      
      // Check if actionResult! is of type ConflictObjectResult
      // and evaluate the result
      var(success,result,value) = 
         EvalActionResult<ConflictObjectResult, T?>(actionResult!);
      
      // Check if success is false
      success.Should().BeFalse();
      
      // Check if result.StatusCode is 409
      result.StatusCode.Should().Be(409);
   }
   
   // HttpStatusCode.Conflict (409)
   public static void IsConflict(
      IActionResult actionResult
   ) {
      actionResult.Should().NotBeNull();
      actionResult.Should().BeOfType<ConflictObjectResult>();
   }
}

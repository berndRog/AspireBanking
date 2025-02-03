namespace BankingClient.Core;

// public abstract class ResultData<T>(
//    T? data = null,
//    HttpStatusCode? status  = null,
//    string message = ""
// ) where T : class? {
// }
//
// public class Success<T>(T? data) : ResultData<T>(data: data)
//    where T : class?;
//
// public class Error<T>(HttpStatusCode? status, string error) : ResultData<T>(status: status, message: error)
//    where T : class?;
//


public abstract record ResultData<T> where T : class? {
   public sealed record Success(T Data) : ResultData<T>;
   public sealed record Error(Exception Exception) : ResultData<T>;
   public sealed record Loading() : ResultData<T>;
}


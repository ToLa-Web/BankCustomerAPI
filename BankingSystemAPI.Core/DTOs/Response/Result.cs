namespace BankingSystemAPI.Core.DTOs.Response;

public class Result<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static Result<T> SuccessResult(T data, string? message = null)
        => new Result<T> { Success = true, Data = data, Message = message };

    public static Result<T> Fail(string message)
        => new Result<T> { Success = false, Message = message };
}
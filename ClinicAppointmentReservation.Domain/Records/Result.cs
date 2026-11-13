using ClinicAppointmentReservation.Domain.Enums;

namespace ClinicAppointmentReservation.Domain.Records;

public record Result(Error? Error = null)
{
    public bool IsSuccess => Error == null;
}

public record Result<T>(ResponseContent<T>? Content, Error? Error = null)
{
    public bool IsSuccess => Error == null;
}

public class ResultBuilder
{
    public Result<T> OkResult<T>(T value) => new Result<T>(new ResponseContent<T>(SuccessType.Ok, value));
    public Result<T> CreatedResult<T>(T value) => new Result<T>(new ResponseContent<T>(SuccessType.Created, value));
    public Result<T> NoContentResult<T>(T value) => new Result<T>(new ResponseContent<T>(SuccessType.NoContent, value));
    public Result<T> ForbiddenResult<T>(string message) => new Result<T>(null, new Error(ErrorType.Forbidden, message));
    public Result<T> ConflictResult<T>(string message) => new Result<T>(null, new Error(ErrorType.Conflict, message));
    public Result<T> NotFoundResult<T>(string message) => new Result<T>(null, new Error(ErrorType.NotFound, message));
    public Result<T> BadRequestResult<T>(string message) => new Result<T>(null, new Error(ErrorType.BadRequest, message));
}

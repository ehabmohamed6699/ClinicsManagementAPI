namespace ClinicAppointmentReservation.Domain.Records;

public record Result(Error? Error = null)
{
    public bool IsSuccess => Error == null;
}

public record Result<T>(ResponseContent<T>? Content, Error? Error = null)
{
    public bool IsSuccess => Error == null;
}

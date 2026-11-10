using ClinicAppointmentReservation.Domain.Enums;

namespace ClinicAppointmentReservation.Domain.Records;

public record class ResponseContent<T>(SuccessType Type, T? Value);

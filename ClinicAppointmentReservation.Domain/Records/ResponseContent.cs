using ClinicAppointmentReservation.Domain.Enums;

namespace ClinicAppointmentReservation.Domain.Records;

public record ResponseContent<T>(SuccessType Type, T? Value);

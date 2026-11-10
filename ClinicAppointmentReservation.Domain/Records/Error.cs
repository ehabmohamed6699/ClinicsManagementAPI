using ClinicAppointmentReservation.Domain.Enums;

namespace ClinicAppointmentReservation.Domain.Records;

public record Error(ErrorType Type, String Message);

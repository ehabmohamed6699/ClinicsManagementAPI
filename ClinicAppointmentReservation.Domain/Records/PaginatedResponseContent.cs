namespace ClinicAppointmentReservation.Domain.Records;

public record class PaginatedResponseContent<T>(IEnumerable<T> Items, int TotalItems);

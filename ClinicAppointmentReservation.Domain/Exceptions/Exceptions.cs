using System;

namespace ClinicAppointmentReservation.Domain.Exceptions;

public class DoctorHasNoApprovedProfileException: Exception
{
    public DoctorHasNoApprovedProfileException(): base("This user does not have an approved doctor profile."){}
}
public class DoctorNotFoundException: Exception
{
    public DoctorNotFoundException(): base("This user does not have a doctor profile."){}
}
public class PatientNotFoundException: Exception
{
    public PatientNotFoundException() : base("This user does not have a patient profile."){}
}

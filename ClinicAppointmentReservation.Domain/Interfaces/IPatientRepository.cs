using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.Domain.Records;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAppointmentReservation.Domain.Interfaces
{
    public interface IPatientRepository
    {
        Result<Patient> Add(Patient entity);
    }
}

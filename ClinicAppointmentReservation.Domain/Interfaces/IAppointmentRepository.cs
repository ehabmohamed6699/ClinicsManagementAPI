using ClinicAppointmentReservation.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAppointmentReservation.Domain.Interfaces
{
    public interface IAppointmentRepository
    {
        Task BookAppointment(FormAppointment appointment, string userId);
    }
}

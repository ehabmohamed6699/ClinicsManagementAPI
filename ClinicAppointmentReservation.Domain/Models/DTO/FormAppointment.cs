using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAppointmentReservation.Domain.Models.DTO
{
    public class FormAppointment
    {
        public DateTime AppointmentDate { get; set; }

        public int DoctorId { get; set; }

        public int ClinicId { get; set; }
    }
}

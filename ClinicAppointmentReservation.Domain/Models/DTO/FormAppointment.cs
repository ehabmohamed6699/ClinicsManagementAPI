using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClinicAppointmentReservation.Domain.Models.DTO
{
    public class FormAppointment
    {
        public DateTime AppointmentDate { get; set; }

        [MaxLength(200)]
        public string? PatientNotes { get; set; }

        public int DoctorId { get; set; }

        public int ClinicId { get; set; }
    }
}

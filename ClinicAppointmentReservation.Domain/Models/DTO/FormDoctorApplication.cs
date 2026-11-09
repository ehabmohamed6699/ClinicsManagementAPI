using ClinicAppointmentReservation.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClinicAppointmentReservation.Domain.Models.DTO
{
    public class FormDoctorApplication
    {
        [Required]
        public string Bio { get; set; }
        [Required]
        [Range(0, 10000)]
        public int ConsultationFee {get; set;}
        [Required]
        public int SpecializationId { get; set; }
    }
}

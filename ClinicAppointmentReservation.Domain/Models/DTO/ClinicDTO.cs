using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClinicAppointmentReservation.Domain.Models.DTO
{
    public class ClinicDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public List<DoctorSummary> Doctors { get; set; }

    }

    public class DoctorSummary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public byte[]? Image { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public bool IsRejected { get; set; }
        public int SpecializationId { get; set; }

        public DoctorSummary(){}
        public DoctorSummary(Doctor d)
        {
            Id = d.Id;
            Bio = d.Bio;
            Name = d.User.Name;
            Image = d.Image;
            IsApproved = d.IsApproved;
            IsRejected = d.IsRejected;
            SpecializationId = d.SpecializationId;
            ApprovalDate = d.ApprovalDate;
        }
    }
}

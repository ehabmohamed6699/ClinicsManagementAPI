using ClinicAppointmentReservation.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAppointmentReservation.Infrastructure.Data.Config
{
    internal class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
    {
        public void Configure(EntityTypeBuilder<Clinic> builder)
        {
            builder.HasData(
                new { Id = 1, Name = "Al Safa", Address = "Faisal", PhoneNumber = "+201100101011"},
                new { Id = 2, Name = "Egy Medica", Address = "Nasr City", PhoneNumber = "+201000101011" },
                new { Id = 3, Name = "Cairo Care", Address = "Dokki", PhoneNumber = "+201200101011" },
                new { Id = 4, Name = "Optica", Address = "Kit Kat", PhoneNumber = "+201500101011" }
            );
        }
    }
}

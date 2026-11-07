using ClinicAppointmentReservation.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicAppointmentReservation.Infrastructure.Data.Config
{
    internal class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
    {
        public void Configure(EntityTypeBuilder<Specialization> builder)
        {
            builder.HasData(
                new { Id = 1, Name = "Cardiology" },
                new { Id = 2, Name = "Dermatology" },
                new { Id = 3, Name = "Neurology" },
                new { Id = 4, Name = "Pediatrics" },
                new { Id = 5, Name = "Orthopedics" }
            );
        }
    }
}

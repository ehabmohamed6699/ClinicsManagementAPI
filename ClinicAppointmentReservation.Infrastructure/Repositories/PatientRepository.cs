using ClinicAppointmentReservation.Domain.Interfaces;
using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.Domain.Records;
using ClinicAppointmentReservation.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAppointmentReservation.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly AppDbContext _context;
        private readonly ResultBuilder _builder;
        public PatientRepository(AppDbContext context, ResultBuilder builder)
        {
            _context = context;
            _builder = builder;
        }
        public Result<Patient> Add(Patient entity)
        {
            _context.Patients.Add(entity);
            return _builder.CreatedResult<Patient>(entity);
        }
    }
}

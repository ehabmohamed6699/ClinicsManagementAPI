using ClinicAppointmentReservation.Domain.Interfaces;
using ClinicAppointmentReservation.Domain.Records;
using ClinicAppointmentReservation.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAppointmentReservation.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly ResultBuilder _builder;
        public IClinicRepository Clinics { get; private set; }
        public ISpecializationRepository Specializations { get; private set; }
        public IPatientRepository Patients { get; private set; }
        public IDoctorRepository Doctors { get; private set; }

        public IAppointmentRepository Appointments { get; private set; }
        public UnitOfWork(AppDbContext context, ResultBuilder builder) {
            _context = context;
            _builder = builder;
            this.Clinics = new ClinicRepository(_context, _builder);
            this.Specializations = new SpecializationRepository(_context, _builder);
            this.Patients = new PatientRepository(_context, _builder);
            this.Doctors = new DoctorRepository(_context, _builder);
            this.Appointments = new AppointmentRepository(_context, _builder);
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}

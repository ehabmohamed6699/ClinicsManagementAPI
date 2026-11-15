using ClinicAppointmentReservation.Domain.Interfaces;
using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.Domain.Models.DTO;
using ClinicAppointmentReservation.Domain.Records;
using ClinicAppointmentReservation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAppointmentReservation.Infrastructure.Repositories
{
    
    public class DoctorRepository : IDoctorRepository
    {
        private readonly AppDbContext _context;
        private readonly ResultBuilder _builder;
        public DoctorRepository(AppDbContext context, ResultBuilder builder)
        {
            _context = context;
            _builder = builder;
        }
        public Result<Doctor> Add(Doctor entity)
        {
            _context.Doctors.Add(entity);
            return _builder.CreatedResult<Doctor>(entity);
        }

        public Result<object> Delete(Doctor entity)
        {
            _context.Doctors.Remove(entity);
            return _builder.NoContentResult<object>(null!);
        }

        public async Task<Result<PaginatedResponseContent<Doctor>>> GetAllAsync(DoctorSearchParameters parameters)
        {
            var query = _context.Doctors.AsNoTracking().AsQueryable().Where(d => d.IsApproved == true);
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                query = query.Where(d => d.User.Name.Contains(parameters.Search));
            }
            if(parameters.SpecializationId.HasValue)
            {
                query = query.Where(d => d.SpecializationId == parameters.SpecializationId.Value);
            }
            if(parameters.ClinicId.HasValue)
            {
                query = query.Where(d => d.Clinics.Any(c => c.Id == parameters.ClinicId.Value));
            }
            int page = Math.Max(parameters.Page, 1);
            var totalCount = await query.CountAsync();
            var doctors = await query.Skip((page - 1) * parameters.PageSize)
                                      .Take(parameters.PageSize)
                                      .ToListAsync();
            return _builder.OkResult<PaginatedResponseContent<Doctor>>(new PaginatedResponseContent<Doctor>(doctors, totalCount));
        }

        public async Task<Result<PaginatedResponseContent<Doctor>>> GetByClinicIdAsync(int clinicId, DoctorSearchParameters parameters)
        {
            var query = _context.Doctors.Where(d => d.Clinics.Any(c => c.Id == clinicId))
                                        .AsNoTracking()
                                        .AsQueryable();
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                query = query.Where(d => d.User.Name.Contains(parameters.Search));
            }
            if(parameters.SpecializationId.HasValue)
            {
                query = query.Where(d => d.SpecializationId == parameters.SpecializationId.Value);
            }
            int page = Math.Max(parameters.Page, 1);
            var totalCount = await query.CountAsync();
            var doctors = await query.Skip((page - 1) * parameters.PageSize)
                                      .Take(parameters.PageSize)
                                      .ToListAsync();
            return _builder.OkResult<PaginatedResponseContent<Doctor>>(new PaginatedResponseContent<Doctor>(doctors, totalCount));
        }

        public async Task<Result<Doctor>> GetByIdAsync(int id)
        {   var result = await _context.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            return result == null ? _builder.NotFoundResult<Doctor>($"Doctor {id} was not found.") : _builder.OkResult<Doctor>(result);
        }

        public async Task<Result<Doctor>> GetByUserIdAsync(string userId)
        {
            var doctor = await _context.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.UserId == userId);
            return doctor == null ? _builder.NotFoundResult<Doctor>($"User {userId} has no doctor profile") : _builder.OkResult<Doctor>(doctor);
        }

        public async Task<Result<Doctor>> GetDetailsAsync(int id)
        {
            var result = await _context.Doctors.AsNoTracking().Include(x => x.Specialization)
                                            .Include(d => d.User)
                                            .Include(d => d.Clinics)
                                            .FirstOrDefaultAsync(d => d.Id == id);
            return result == null ? _builder.NotFoundResult<Doctor>($"Doctor {id} was not found.") : _builder.OkResult<Doctor>(result);
        }

        public async Task<Result<IEnumerable<AppointmentDTO>>> GetMyAppointments(string userId)
        {
            var doctor = await _context.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.UserId == userId);
            if(doctor == null || !doctor.IsApproved || doctor.IsRejected) return _builder.NotFoundResult<IEnumerable<AppointmentDTO>>($"User {userId} does not have an approved doctor profile.");
            var appointments = await _context.Appointments.Where(a => a.DoctorId == doctor.Id).Select(a => new AppointmentDTO { 
                Id = a.Id,
                AppointmentDate = a.AppointmentDate,
                Status = a.Status,
                Fee = a.Fee,
                PatientNotes = a.PatientNotes,
                CreatedAt = a.CreatedAt,
                Patient = new AppointmentPatientDTO
                {
                    Id = a.Patient.Id,
                    Email = a.Patient.User.Email,
                    Name = a.Patient.User.Name
                },
                Clinic = new AppointmentClinicDTO
                {
                    Id = a.Clinic.Id,
                    Name = a.Clinic.Name,
                    PhoneNumber = a.Clinic.PhoneNumber
                }
            }).ToListAsync();
            return _builder.OkResult<IEnumerable<AppointmentDTO>>(appointments);
        }

        public async Task<Result<PaginatedResponseContent<Doctor>>> GetPendingDoctors(DoctorSearchParameters parameters)
        {
            var query = _context.Doctors.AsNoTracking().Where(d => !d.IsApproved && !d.IsRejected);
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                query = query.Where(d => d.User.Name.Contains(parameters.Search));
            }
            if (parameters.SpecializationId.HasValue)
            {
                query = query.Where(d => d.SpecializationId == parameters.SpecializationId.Value);
            }
            int page = Math.Max(parameters.Page, 1);
            var totalCount = await query.CountAsync();
            var doctors = await query
                                     .Include(d => d.User)
                                     .Skip((page - 1) * parameters.PageSize)
                                     .Take(parameters.PageSize)
                                     .ToListAsync();
            return _builder.OkResult<PaginatedResponseContent<Doctor>>(new PaginatedResponseContent<Doctor>(doctors, totalCount));
        }

        public Result<object> Update(Doctor entity)
        {
            _context.Doctors.Update(entity);
            return _builder.NoContentResult<object>(null!);
        }
    }
}

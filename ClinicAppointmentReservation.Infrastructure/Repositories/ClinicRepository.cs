using ClinicAppointmentReservation.Domain.Enums;
using ClinicAppointmentReservation.Domain.Interfaces;
using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.Domain.Models.DTO;
using ClinicAppointmentReservation.Domain.Records;
using ClinicAppointmentReservation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentReservation.Infrastructure.Repositories
{
    public class ClinicRepository : IClinicRepository
    {
        private readonly AppDbContext _context;
        private readonly ResultBuilder _builder;
        public ClinicRepository(AppDbContext context, ResultBuilder builder)
        {
            _context = context;
            _builder = builder;
        }
        public Result<Clinic> Add(Clinic entity)
        {
            _context.Clinics.Add(entity);
            
            return _builder.CreatedResult<Clinic>(entity);
        }

        public async Task<Result<string>> AssignDoctorToClinic(int doctorId, int clinicId)
        {
            var assigned = await _context.Clinics
                .Where(c => c.Id == clinicId)
                .SelectMany(c => c.Doctors)
                .AnyAsync(d => d.Id == doctorId);
            if(assigned)
                return _builder.ConflictResult<string>("Doctor is already assigned to the clinic.");
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if(doctor == null) return _builder.NotFoundResult<string>($"Doctor {doctorId} does not exist.");
            if(!doctor.IsApproved) return _builder.ConflictResult<string>($"Doctor {doctorId} is not approved.");
            var clinic = await _context.Clinics.Include(c => c.Doctors).FirstOrDefaultAsync(c => c.Id == clinicId);
            if(clinic == null) return _builder.NotFoundResult<string>($"Clinic {clinicId} does not exist.");
            clinic.Doctors.Add(doctor);
            return _builder.OkResult<string>("Doctor assigned to clinic successfullt.");
        }

        public Result<object> Delete(Clinic entity)
        {
            _context.Clinics.Remove(entity);
            return _builder.NoContentResult<object>(null!);
        }

        public async Task<Result<PaginatedResponseContent<Clinic>>> GetAllAsync(ClinicSearchParameters parameters)
        {
            var query = _context.Clinics.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                query = query.Where(c => c.Name.Contains(parameters.Search) || c.Address.Contains(parameters.Search));
            }
            int page = Math.Max(parameters.Page, 1);
            var totalCount = await query.CountAsync();
            var clinics = await query.Skip((page - 1) * parameters.PageSize)
                                     .Take(parameters.PageSize)
                                     .ToListAsync();
            return _builder.OkResult<PaginatedResponseContent<Clinic>>(new PaginatedResponseContent<Clinic>(clinics, totalCount));
        }

        public async Task<Result<ClinicDTO>> GetByIdAsync(int id)
        {
            var content = await _context.Clinics
                .Where(c => c.Id == id)
                .Select(c => new ClinicDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Address = c.Address,
                    PhoneNumber = c.PhoneNumber,
                    Doctors = c.Doctors.Select(d => new DoctorSummary
                    {
                        Id = d.Id,
                        Name = d.User.Name,
                        Bio = d.Bio,
                        Image = d.Image,
                        ApprovalDate = d.ApprovalDate,
                        IsApproved = d.IsApproved,
                        IsRejected = d.IsRejected,
                        SpecializationId = d.SpecializationId

                    }).ToList()
                })
                .FirstOrDefaultAsync();
            return content == null ? _builder.NotFoundResult<ClinicDTO>($"Clinic {id} was not found.") : _builder.OkResult<ClinicDTO>(content);
        }

        public async Task<Result<string>> UnassignDoctorFromClinic(int doctorId, int clinicId)
        {
            var assigned = await _context.Clinics
                    .Where(c => c.Id == clinicId)
                    .SelectMany(c => c.Doctors)
                    .AnyAsync(d => d.Id == doctorId);
            if (!assigned)
                return _builder.ConflictResult<string>("Doctor is not assigned to the clinic.");
            var clinic = await _context.Clinics.Include(c => c.Doctors).FirstOrDefaultAsync(c => c.Id == clinicId);
            var doctor = await _context.Doctors.FindAsync(doctorId);
            clinic!.Doctors.Remove(doctor!);
            return _builder.OkResult<string>("Doctor removed from clinic successfullt.");
        }

        public Result<object> Update(Clinic entity)
        {
            _context.Clinics.Update(entity);
            return _builder.NoContentResult<object>(null!);
        }
    }
}

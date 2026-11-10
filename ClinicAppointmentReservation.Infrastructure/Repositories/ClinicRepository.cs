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
        public ClinicRepository(AppDbContext context)
        {
            _context = context;
        }
        public Result<ClinicDTO> Add(Clinic entity)
        {
            _context.Clinics.Add(entity);
            var clinicDto = new ClinicDTO{
                Id = entity.Id, 
                Name = entity.Name, 
                Address = entity.Address, 
                PhoneNumber = entity.PhoneNumber
            };
            return new Result<ClinicDTO>(new ResponseContent<ClinicDTO>(SuccessType.Created, clinicDto));
        }

        public async Task<Result<string>> AssignDoctorToClinic(int doctorId, int clinicId)
        {
            var assigned = await _context.Clinics
                .Where(c => c.Id == clinicId)
                .SelectMany(c => c.Doctors)
                .AnyAsync(d => d.Id == doctorId);
            if(assigned)
                return new Result<string>(Content: null,Error: new Error(ErrorType.Conflict, "Doctor is already assigned to the clinic."));
            var doctor = await _context.Doctors.FindAsync(doctorId);
            var clinic = await _context.Clinics.Include(c => c.Doctors).FirstOrDefaultAsync(c => c.Id == clinicId);
            clinic!.Doctors.Add(doctor!);
            return new Result<string>(new ResponseContent<string>(SuccessType.Ok, "Doctor assigned to clinic successfullt."));
        }

        public Result<object> Delete(Clinic entity)
        {
            _context.Clinics.Remove(entity);
            return new Result<object>(new ResponseContent<object>(SuccessType.NoContent, null));
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
            return new Result<PaginatedResponseContent<Clinic>>(new ResponseContent<PaginatedResponseContent<Clinic>>(SuccessType.Ok, new PaginatedResponseContent<Clinic>(clinics, totalCount)));
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
            return content == null ? new Result<ClinicDTO>(Content: null, Error: new Error(ErrorType.NotFound, $"Clinic {id} was not found.")) : new Result<ClinicDTO>(new ResponseContent<ClinicDTO>(SuccessType.Ok, content));
        }

        public async Task<Result<string>> UnassignDoctorFromClinic(int doctorId, int clinicId)
        {
            var assigned = await _context.Clinics
                    .Where(c => c.Id == clinicId)
                    .SelectMany(c => c.Doctors)
                    .AnyAsync(d => d.Id == doctorId);
            if (!assigned)
                return new Result<string>(Content: null,Error: new Error(ErrorType.Conflict, "Doctor is not assigned to the clinic."));
            var clinic = await _context.Clinics.Include(c => c.Doctors).FirstOrDefaultAsync(c => c.Id == clinicId);
            var doctor = await _context.Doctors.FindAsync(doctorId);
            clinic!.Doctors.Remove(doctor!);
            return new Result<string>(new ResponseContent<string>(SuccessType.Ok, "Doctor removed from clinic successfullt."));
        }

        public Result<object> Update(Clinic entity)
        {
            _context.Clinics.Update(entity);
            return new Result<object>(new ResponseContent<object>(SuccessType.NoContent, null));
        }
    }
}

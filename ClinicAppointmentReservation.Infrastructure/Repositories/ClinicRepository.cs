using ClinicAppointmentReservation.Domain.Interfaces;
using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.Domain.Models.DTO;
using ClinicAppointmentReservation.Domain.Records;
using ClinicAppointmentReservation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ClinicAppointmentReservation.Infrastructure.Repositories
{
    public class ClinicRepository : IClinicRepository
    {
        private readonly AppDbContext _context;
        public ClinicRepository(AppDbContext context)
        {
            _context = context;
        }
        public void Add(Clinic entity)
        {
            _context.Clinics.Add(entity);
        }

        public async Task AssignDoctorToClinic(int doctorId, int clinicId)
        {
            var assigned = await _context.Clinics
                .Where(c => c.Id == clinicId)
                .SelectMany(c => c.Doctors)
                .AnyAsync(d => d.Id == doctorId);
            if(assigned)
                throw new InvalidOperationException("Doctor is already assigned to the clinic.");
            var doctor = await _context.Doctors.FindAsync(doctorId);
            var clinic = await _context.Clinics.Include(c => c.Doctors).FirstOrDefaultAsync(c => c.Id == clinicId);
            clinic!.Doctors.Add(doctor!);
        }

        public void Delete(Clinic entity)
        {
            _context.Clinics.Remove(entity);
        }

        public async Task<(IEnumerable<Clinic> Clinics, int TotalCount)> GetAllAsync(ClinicSearchParameters parameters)
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

            return (clinics, totalCount);
        }

        public async Task<ClinicDTO?> GetByIdAsync(int id)
        {
            return await _context.Clinics
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
        }

        public async Task UnassignDoctorFromClinic(int doctorId, int clinicId)
        {
            var assigned = await _context.Clinics
                    .Where(c => c.Id == clinicId)
                    .SelectMany(c => c.Doctors)
                    .AnyAsync(d => d.Id == doctorId);
            if (!assigned)
                throw new InvalidOperationException("Doctor is not assigned to the clinic.");
            var clinic = await _context.Clinics.Include(c => c.Doctors).FirstOrDefaultAsync(c => c.Id == clinicId);
            var doctor = await _context.Doctors.FindAsync(doctorId);
            clinic!.Doctors.Remove(doctor!);
        }

        public void Update(Clinic entity)
        {
            _context.Clinics.Update(entity);
        }
    }
}

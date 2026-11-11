using ClinicAppointmentReservation.Domain.Enums;
using ClinicAppointmentReservation.Domain.Interfaces;
using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.Domain.Records;
using ClinicAppointmentReservation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace ClinicAppointmentReservation.Infrastructure.Repositories
{
    public class SpecializationRepository : ISpecializationRepository
    {
        private readonly AppDbContext _context;
        private readonly ResultBuilder _builder;
        public SpecializationRepository(AppDbContext context, ResultBuilder builder)
        {
            _context = context;
            _builder = builder;
        }
        public async Task<Result<Specialization>> Create(Specialization specialization)
        {
            var exists = await _context.Specializations.AnyAsync(s => s.Name == specialization.Name);
            if(exists) return _builder.ConflictResult<Specialization>($"Specialization {specialization.Name} already exists.");
            _context.Specializations.Add(specialization);
            return _builder.CreatedResult<Specialization>(specialization);
        }

        public Result<object> Delete(Specialization specialization)
        {
            _context.Specializations.Remove(specialization);
            return _builder.NoContentResult<object>(null!);
        }

        public async Task<Result<IEnumerable<Specialization>>> GetAllAsync()
        {
            return _builder.OkResult<IEnumerable<Specialization>>(await _context.Specializations.AsNoTracking().ToListAsync());
        }

        public async Task<Result<Specialization>> GetByIdAsync(int id)
        {
            var result = await _context.Specializations.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            return result == null ? _builder.NotFoundResult<Specialization>($"Specialization {id} was not found.") : _builder.OkResult<Specialization>(result);
        }

        public Result<object> Update(Specialization specialization)
        {
            _context.Specializations.Update(specialization);
            return _builder.NoContentResult<object>(null!);
        }
    }
}

using ClinicAppointmentReservation.Domain.Enums;
using ClinicAppointmentReservation.Domain.Interfaces;
using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.Domain.Records;
using ClinicAppointmentReservation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentReservation.Infrastructure.Repositories
{
    public class SpecializationRepository : ISpecializationRepository
    {
        private readonly AppDbContext _context;
        public SpecializationRepository(AppDbContext context)
        {
            _context = context;
        }
        public Result<Specialization> Create(Specialization specialization)
        {
            _context.Specializations.Add(specialization);
            return new Result<Specialization>(new ResponseContent<Specialization>(Value: specialization, Type: SuccessType.Created));
        }

        public Result<object> Delete(Specialization specialization)
        {
            _context.Specializations.Remove(specialization);
            return new Result<object>(new ResponseContent<object>(Type: SuccessType.NoContent, Value: null));
        }

        public async Task<Result<IEnumerable<Specialization>>> GetAllAsync()
        {
            return new Result<IEnumerable<Specialization>>(new ResponseContent<IEnumerable<Specialization>>(Type: SuccessType.Ok, Value: await _context.Specializations.AsNoTracking().ToListAsync()));
        }

        public async Task<Result<Specialization>> GetByIdAsync(int id)
        {
            var result = await _context.Specializations.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            return result == null ? new Result<Specialization>(Content: null, Error: new Error(ErrorType.NotFound, $"Specialization {id} was not found.")) : new Result<Specialization>(new ResponseContent<Specialization>(Type: SuccessType.Ok, Value: result));
        }

        public Result<object> Update(Specialization specialization)
        {
            _context.Specializations.Update(specialization);
            return new Result<object>(new ResponseContent<object>(Type: SuccessType.NoContent, Value: null));
        }
    }
}

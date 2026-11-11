using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.Domain.Records;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAppointmentReservation.Domain.Interfaces
{
    public interface ISpecializationRepository
    {
        Task<Result<IEnumerable<Specialization>>> GetAllAsync();
        Task<Result<Specialization>> GetByIdAsync(int id);
        Task<Result<Specialization>> Create(Specialization specialization);
        Result<object> Update(Specialization specialization);
        Result<object> Delete(Specialization specialization);
    }
}

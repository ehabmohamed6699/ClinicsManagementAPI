using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.Domain.Models.DTO;
using ClinicAppointmentReservation.Domain.Records;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAppointmentReservation.Domain.Interfaces
{
    public interface IDoctorRepository
    {
        Task<Result<PaginatedResponseContent<Doctor>>> GetAllAsync(DoctorSearchParameters parameters);
        Task<Result<Doctor>> GetByIdAsync(int id);
        Task<Result<Doctor>> GetByUserIdAsync(string userId);
        Result<Doctor> Add(Doctor entity);
        Result<object> Update(Doctor entity);
        Result<object> Delete(Doctor entity);

        Task<Result<Doctor>> GetDetailsAsync(int id);
        Task<Result<PaginatedResponseContent<Doctor>>> GetByClinicIdAsync(int clinicId, DoctorSearchParameters parameters);
        Task<Result<PaginatedResponseContent<Doctor>>> GetPendingDoctors(DoctorSearchParameters parameters);

        Task<Result<IEnumerable<AppointmentDTO>>> GetMyAppointments(string userId);

    }
}

using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.Domain.Models.DTO;
using ClinicAppointmentReservation.Domain.Records;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAppointmentReservation.Domain.Interfaces
{
    public interface IClinicRepository
    {
        Task<Result<PaginatedResponseContent<Clinic>>> GetAllAsync(ClinicSearchParameters parameters);
        Task<Result<ClinicDTO>> GetByIdAsync(int id);
        Result<ClinicDTO> Add(Clinic entity);
        Result<object> Update(Clinic entity);
        Result<object> Delete(Clinic entity);
        Task<Result<string>> AssignDoctorToClinic(int doctorId, int clinicId);
        Task<Result<string>> UnassignDoctorFromClinic(int doctorId, int clinicId);
    }
}

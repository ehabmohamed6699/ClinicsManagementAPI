using ClinicAppointmentReservation.Domain.Interfaces;
using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.Domain.Models.DTO;
using ClinicAppointmentReservation.Domain.Records;
using ClinicAppointmentReservation.Infrastructure.Data;
using ClinicAppointmentReservation.WebAPI.Extentions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentReservation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClinicsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllClinics(string? search, int? page)
        {
            var parameters = new ClinicSearchParameters(search, page ?? 1);
            var result = await _unitOfWork.Clinics.GetAllAsync(parameters);
            return this.ToActionResult<PaginatedResponseContent<Clinic>>(result);
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetClinicById(int id)
        {
            var result = await _unitOfWork.Clinics.GetByIdAsync(id);
            return this.ToActionResult<ClinicDTO>(result);
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> CreateClinic(FormClinic clinic)
        {
            var newClinic = new Clinic
            {
                Name = clinic.Name,
                Address = clinic.Address,
                PhoneNumber = clinic.PhoneNumber
            };
            var result = _unitOfWork.Clinics.Add(newClinic);
            await _unitOfWork.SaveChangesAsync();
            return this.ToCreatedAtActionResult<Clinic>(result, nameof(GetClinicById), new {Id = result.Content!.Value!.Id});
        }
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> UpdateClinic(int id, FormClinic clinic)
        {
            var searchResult = await _unitOfWork.Clinics.GetByIdAsync(id);
            if (!searchResult.IsSuccess)
            {
                return this.ToActionResult<ClinicDTO>(searchResult);
            }
            var updatedClinic = new Clinic
            {
                Id = searchResult.Content!.Value!.Id,
                Name = clinic.Name,
                Address = clinic.Address,
                PhoneNumber = clinic.PhoneNumber
            };
            var result = _unitOfWork.Clinics.Update(updatedClinic);
            await _unitOfWork.SaveChangesAsync();
            return this.ToActionResult<object>(result);
        }
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> DeleteClinic(int id)
        {
            var searchResult = await _unitOfWork.Clinics.GetByIdAsync(id);
            if (!searchResult.IsSuccess)
            {
                return this.ToActionResult<ClinicDTO>(searchResult);
            }
            var result =  _unitOfWork.Clinics.Delete(new Clinic
            {
                Id = searchResult.Content!.Value!.Id
            });
            await _unitOfWork.SaveChangesAsync();
            return this.ToActionResult<object>(result);
        }
        [HttpPost("{clinicId}/assign-doctor/{doctorId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AssignDoctorToClinic(int clinicId, int doctorId)
        {
            var clinicSeachResult = await _unitOfWork.Clinics.GetByIdAsync(clinicId);
            if (!clinicSeachResult.IsSuccess)
            {
                return this.ToActionResult<ClinicDTO>(clinicSeachResult);
            }
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(doctorId);
            if (doctor == null)
            {
                return NotFound("Doctor not found.");
            }
            var result = await _unitOfWork.Clinics.AssignDoctorToClinic(doctorId, clinicId);
            await _unitOfWork.SaveChangesAsync();
            return this.ToActionResult<string>(result);
        }
        [HttpPost("{clinicId}/unassign-doctor/{doctorId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> UnassignDoctorFromClinic(int clinicId, int doctorId)
        {
            var clinicSeachResult = await _unitOfWork.Clinics.GetByIdAsync(clinicId);
            if (!clinicSeachResult.IsSuccess)
            {
                return this.ToActionResult<ClinicDTO>(clinicSeachResult);
            }
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(doctorId);
            if (doctor == null)
            {
                return NotFound("Doctor not found.");
            }
            var result = await _unitOfWork.Clinics.UnassignDoctorFromClinic(doctorId, clinicId);
            await _unitOfWork.SaveChangesAsync();
            return this.ToActionResult<string>(result);
        }
    }
}
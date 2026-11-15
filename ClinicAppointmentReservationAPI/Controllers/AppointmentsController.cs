using ClinicAppointmentReservation.Domain.Interfaces;
using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.Domain.Models.DTO;
using ClinicAppointmentReservation.WebAPI.Extentions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointmentReservation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        public AppointmentsController(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "patient,doctor")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Problem(statusCode: StatusCodes.Status404NotFound, title: $"User profile not found.");
            var patient = await _unitOfWork.Patients.GetByUserIdAsync(user.Id);
            var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(user.Id);
            if(!patient.IsSuccess && !doctor.IsSuccess) return this.ToActionResult<Patient>(patient);
            int patientId = patient.IsSuccess ? patient.Content!.Value!.Id : 0;
            int doctorId = doctor.IsSuccess ? doctor.Content!.Value!.Id : 0;
            var result = await _unitOfWork.Appointments.GetById(id, patientId, doctorId);
            return this.ToActionResult<AppointmentDTO>(result);
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "patient")]
        public async Task<IActionResult> MakeAppointment([FromBody] FormAppointment appointment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Problem(statusCode: StatusCodes.Status404NotFound, title: $"User profile not found.");
            var result = await _unitOfWork.Appointments.BookAppointment(appointment, user.Id);

            await _unitOfWork.SaveChangesAsync();
            return this.ToCreatedAtActionResult<AppointmentDTO>(result.Map<AppointmentDTO>(a => new AppointmentDTO(a)), nameof(GetById), v => new {Id = v.Id});
        }
    }
}

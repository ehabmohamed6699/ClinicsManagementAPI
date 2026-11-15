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
        public async Task<IActionResult> GetById(int id)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "patient")]
        public async Task<IActionResult> MakeAppointment([FromBody] FormAppointment appointment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Problem(statusCode: StatusCodes.Status404NotFound, title: $"User profile not found.");
            var result = await _unitOfWork.Appointments.BookAppointment(appointment, user.Id);
            if(!result.IsSuccess)
            await _unitOfWork.SaveChangesAsync();
            return this.ToCreatedAtActionResult<Appointment>(result, nameof(GetById), new {Id = result.Content.Value.Id});
        }
    }
}

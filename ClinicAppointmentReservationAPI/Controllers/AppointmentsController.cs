using ClinicAppointmentReservation.Domain.Exceptions;
using ClinicAppointmentReservation.Domain.Interfaces;
using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.Domain.Models.DTO;
using ClinicAppointmentReservation.Infrastructure.Data.Mapper;
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
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "patient")]
        public async Task<IActionResult> MakeAppointment([FromBody] FormAppointment appointment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return BadRequest(ModelState);
            try
            {
                await _unitOfWork.Appointments.BookAppointment(appointment, user.Id);
            }
            catch(PatientNotFoundException e)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, title: e.Message);
            }
            catch(DoctorNotFoundException e)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, title: e.Message);
            }
            catch(DoctorHasNoApprovedProfileException e)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, title: e.Message);
            }
            await _unitOfWork.SaveChangesAsync();
            return Created();
        }
    }
}

using ClinicAppointmentReservation.Domain.Interfaces;
using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.Domain.Models.DTO;
using ClinicAppointmentReservation.Domain.Records;
using ClinicAppointmentReservation.Infrastructure.Data.Mapper;
using ClinicAppointmentReservation.WebAPI.Extentions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointmentReservation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DoctorMapper _mapper;
        private readonly UserManager<User> _userManager;
        public DoctorsController(IUnitOfWork unitOfWork, DoctorMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DoctorSearchParameters parameters)
        {
            var result = await _unitOfWork.Doctors.GetAllAsync(parameters);
            return this.ToActionResult<PaginatedResponseContent<Doctor>>(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _unitOfWork.Doctors.GetDetailsAsync(id);
            if (!result.IsSuccess)
            {
                return this.ToActionResult<Doctor>(result);
            }
            DoctorDTO doctorDTO = _mapper.MapToDoctorDTO(result.Content!.Value!);
            return Ok(doctorDTO);
        }
        [HttpPost("become-a-doctor")]
        [Authorize]
        public async Task<IActionResult> BecomeADoctor(FormDoctorApplication model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest(ModelState);
            }
            var searchResult = await _unitOfWork.Doctors.GetByUserIdAsync(user.Id);
            if (searchResult.IsSuccess)
            {
                return Problem(statusCode: StatusCodes.Status409Conflict, title: "You already have a doctor profile.");
            }
            var newDoctor = new Doctor
            {
                Bio = model.Bio,
                SpecializationId = model.SpecializationId,
                UserId = user.Id,
                ConsultationFee = model.ConsultationFee
            };
            var result = _unitOfWork.Doctors.Add(newDoctor);
            await _unitOfWork.SaveChangesAsync();
            return this.ToCreatedAtActionResult<Doctor>(result, nameof(GetById), new {Id = result.Content!.Value!.Id});
        }
        [HttpPatch("approve-doctor/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> ApproveDoctor(int id)
        {
            var searchResult = await _unitOfWork.Doctors.GetByIdAsync(id);
            if (!searchResult.IsSuccess)
            {
                return this.ToActionResult<Doctor>(searchResult);
            }
            var user = await _userManager.FindByIdAsync(searchResult.Content!.Value!.UserId);
            if(user == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, title: "This doctor profile does not belong to a user.");
            }
            if(searchResult.Content.Value.IsRejected)
            {
                return Problem(statusCode:StatusCodes.Status409Conflict,title:"You cannot approve an already rejected doctor.");
            }
            searchResult.Content.Value.IsApproved = true;
            searchResult.Content.Value.ApprovalDate = DateTime.Now;
            await _userManager.AddToRoleAsync(user, "doctor");
            var result = _unitOfWork.Doctors.Update(searchResult.Content.Value);
            await _unitOfWork.SaveChangesAsync();
            return this.ToActionResult<object>(result);
        }
        [HttpPatch("reject-doctor/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> RejectDoctor(int id)
        {
            var searchResult = await _unitOfWork.Doctors.GetByIdAsync(id);
            if (!searchResult.IsSuccess)
            {
                return this.ToActionResult<Doctor>(searchResult);
            }
            var user = await _userManager.FindByIdAsync(searchResult.Content!.Value!.UserId);
            if(user == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, title: "This doctor profile does not belong to a user.");
            }
            if(searchResult.Content.Value.IsApproved)
            {
                return Problem(statusCode:StatusCodes.Status409Conflict,title:"You cannot reject an already approved doctor.");
            }
            searchResult.Content.Value.IsRejected = true;
            var result  = _unitOfWork.Doctors.Update(searchResult.Content.Value);
            await _unitOfWork.SaveChangesAsync();
            return this.ToActionResult<object>(result);
        }
        [HttpDelete("delete-doctor/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var searchResult = await _unitOfWork.Doctors.GetByIdAsync(id);
            if (!searchResult.IsSuccess)
            {
                return this.ToActionResult<Doctor>(searchResult);
            }
            var user = await _userManager.FindByIdAsync(searchResult.Content!.Value!.UserId);
            if(user == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, title: "This doctor profile does not belong to a user.");
            }
            var result = _unitOfWork.Doctors.Delete(searchResult.Content.Value);
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("doctor"))
            {
                await _userManager.RemoveFromRoleAsync(user, "doctor");
            }
            await _unitOfWork.SaveChangesAsync();
            return this.ToActionResult<object>(result);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        [HttpGet("pending-doctors")]
        public async Task<IActionResult> GetPendingDoctors([FromQuery] DoctorSearchParameters parameters)
        {
            var result = await _unitOfWork.Doctors.GetPendingDoctors(parameters);
            return this.ToActionResult<PaginatedResponseContent<Doctor>>(result);
        }
        [HttpGet("appointments")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "doctor")]
        public async Task<IActionResult> GetDoctorAppointments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest(ModelState);
            }
            var result = await _unitOfWork.Doctors.GetMyAppointments(user.Id);
            return this.ToActionResult<IEnumerable<AppointmentDTO>>(result);
        }
    }
}

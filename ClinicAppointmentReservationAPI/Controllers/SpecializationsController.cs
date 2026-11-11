using ClinicAppointmentReservation.Domain.Interfaces;
using ClinicAppointmentReservation.Domain.Models;
using ClinicAppointmentReservation.WebAPI.Extentions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointmentReservation.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecializationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public SpecializationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllSpecializations()
        {
            var result = await _unitOfWork.Specializations.GetAllAsync();
            return this.ToActionResult<IEnumerable<Specialization>>(result);
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSpecializationById(int id)
        {
            var result = await _unitOfWork.Specializations.GetByIdAsync(id);
            return this.ToActionResult<Specialization>(result);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> CreateSpecialization([FromBody] string name)
        {
            var newSpecialization = new Specialization
            {
                Name = name
            };
            var result = await _unitOfWork.Specializations.Create(newSpecialization);
            await _unitOfWork.SaveChangesAsync();
            if(!result.IsSuccess) return this.ToActionResult<Specialization>(result);
            return this.ToCreatedAtActionResult<Specialization>(result, nameof(GetSpecializationById), new {Id = result.Content!.Value!.Id});
        }
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> UpdateSpecialization(int id, [FromBody] string name)
        {
            var searchResult = await _unitOfWork.Specializations.GetByIdAsync(id);
            if (!searchResult.IsSuccess)
            {
                return this.ToActionResult<Specialization>(searchResult);
            }
            searchResult.Content!.Value!.Name = name;
            var result = _unitOfWork.Specializations.Update(searchResult.Content.Value);
            await _unitOfWork.SaveChangesAsync();
            return this.ToActionResult<object>(result);
        }
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> DeleteSpecialization(int id)
        {
            var searchResult = await _unitOfWork.Specializations.GetByIdAsync(id);
            if (!searchResult.IsSuccess)
            {
                return this.ToActionResult<Specialization>(searchResult);
            }
            var result = _unitOfWork.Specializations.Delete(searchResult.Content!.Value!);
            await _unitOfWork.SaveChangesAsync();
            return this.ToActionResult<object>(result);
        }
    }
}
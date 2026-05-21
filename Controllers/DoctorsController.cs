using HospitalManagementApi.Models;
using HospitalManagementApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DoctorsController : ControllerBase
	{
		private readonly IDoctorRepository _repo;

		public DoctorsController(IDoctorRepository repo)
		{
			_repo = repo;
		}

		// GET: api/doctors
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var doctors = await _repo.GetAllAsync();
				return Ok(doctors);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Server error: {ex.Message}");
			}
		}

		// GET: api/doctors/available
		[HttpGet("available")]
		public async Task<IActionResult> GetAvailable()
		{
			try
			{
				var doctors = await _repo.GetAvailableAsync();
				return Ok(doctors);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Server error: {ex.Message}");
			}
		}

		// POST: api/doctors
		// DoctorId nahi aayega — DB auto generate karega
		[HttpPost]
		public async Task<IActionResult> Add([FromBody] CreateDoctorDto dto)
		{
			try
			{
				var doctor = new Doctor
				{
					FullName = dto.FullName,
					Specialization = dto.Specialization,
					IsAvailable = dto.IsAvailable,
					AvailableFrom = dto.AvailableFrom,
					AvailableTo = dto.AvailableTo
				};
				await _repo.AddAsync(doctor);
				return Ok("Doctor added successfully!");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error while adding doctor: {ex.Message}");
			}
		}

		// PUT: api/doctors
		[HttpPut]
		public async Task<IActionResult> Update([FromBody] Doctor doctor)
		{
			try
			{
				await _repo.UpdateAsync(doctor);
				return Ok("Doctor updated successfully!");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error while updating doctor: {ex.Message}");
			}
		}

		// DELETE: api/doctors/1
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var doctor = await _repo.GetByIdAsync(id);
				if (doctor == null)
					return NotFound("Doctor not found!");

				await _repo.DeleteAsync(id);
				return Ok("Doctor deleted successfully!");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error while deleting doctor: {ex.Message}");
			}
		}
	}
}
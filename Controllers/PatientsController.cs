using HospitalManagementApi.Models;
using HospitalManagementApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PatientsController : ControllerBase
	{
		private readonly IPatientRepository _repo;

		public PatientsController(IPatientRepository repo)
		{
			_repo = repo;
		}

		// GET: api/patients
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var patients = await _repo.GetAllAsync();
				return Ok(patients);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Server error: {ex.Message}");
			}
		}

		// GET: api/patients/1
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			try
			{
				var patient = await _repo.GetByIdAsync(id);
				if (patient == null)
					return NotFound("Patient not found!");
				return Ok(patient);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Server error: {ex.Message}");
			}
		}

		// POST: api/patients
		[HttpPost]
		public async Task<IActionResult> Add([FromBody] CreatePatientDto dto)
		{
			try
			{
				var patient = new Patient
				{
					FullName = dto.FullName,
					Email = dto.Email,
					Phone = dto.Phone,
					DateOfBirth = dto.DateOfBirth
				};
				await _repo.AddAsync(patient);
				return Ok("Patient added successfully!");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error while adding patient: {ex.Message}");
			}
		}

		// DELETE: api/patients/1
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				await _repo.DeleteAsync(id);
				return Ok("Patient deleted successfully!");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error while deleting patient: {ex.Message}");
			}
		}
	}
}
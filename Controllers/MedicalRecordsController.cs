using Azure.Storage.Blobs;
using HospitalManagementApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class MedicalRecordsController : ControllerBase
	{
		private readonly IMedicalRecordRepository _repo;
		private readonly IConfiguration _config;

		public MedicalRecordsController(
			IMedicalRecordRepository repo,
			IConfiguration config)
		{
			_repo = repo;
			_config = config;
		}

		// GET: api/medicalrecords/patient/1
		[HttpGet("patient/{patientId}")]
		public async Task<IActionResult> GetByPatient(int patientId)
		{
			try
			{
				var records = await _repo.GetByPatientIdAsync(patientId);
				return Ok(records);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error while fetching records: {ex.Message}");
			}
		}

		// POST: api/medicalrecords/upload
		[HttpPost("upload")]
		public async Task<IActionResult> UploadReport(
			IFormFile file, [FromQuery] int patientId)
		{
			try
			{
				if (file == null || file.Length == 0)
					return BadRequest("File is empty or not selected!");

				var connectionString = _config.GetConnectionString("StorageConnection");
				var blobServiceClient = new BlobServiceClient(connectionString);
				var container = blobServiceClient
					.GetBlobContainerClient("medical-reports");

				var blobName = $"{patientId}-{file.FileName}";
				await container.UploadBlobAsync(blobName, file.OpenReadStream());

				return Ok($"Report uploaded successfully! File: {blobName}");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error while uploading file: {ex.Message}");
			}
		}
	}
}
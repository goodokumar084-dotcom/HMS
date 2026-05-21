using Azure.Messaging.ServiceBus;
using HospitalManagementApi.Models;
using HospitalManagementApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HospitalManagementApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AppointmentsController : ControllerBase
	{
		private readonly IAppointmentRepository _repo;
		private readonly IDoctorRepository _doctorRepo;
		private readonly IPatientRepository _patientRepo;
		private readonly IConfiguration _config;

		public AppointmentsController(
			IAppointmentRepository repo,
			IDoctorRepository doctorRepo,
			IPatientRepository patientRepo,
			IConfiguration config)
		{
			_repo = repo;
			_doctorRepo = doctorRepo;
			_patientRepo = patientRepo;
			_config = config;
		}

		// GET: api/appointments
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var appointments = await _repo.GetAllAsync();
				return Ok(appointments);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Server error: {ex.Message}");
			}
		}

		// GET: api/appointments/1
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			try
			{
				var appointment = await _repo.GetByIdAsync(id);
				if (appointment == null)
					return NotFound("Appointment not found!");
				return Ok(appointment);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Server error: {ex.Message}");
			}
		}

		// POST: api/appointments
		// Swagger mein sirf PatientId, DoctorId, AppointmentDate aayega
		[HttpPost]
		public async Task<IActionResult> Book([FromBody] CreateAppointmentDto dto)
		{
			try
			{
				// 1. Patient exist karta hai?
				var patient = await _patientRepo.GetByIdAsync(dto.PatientId);
				if (patient == null)
					return NotFound("Patient not found!");

				// 2. Doctor fetch karo
				var doctor = await _doctorRepo.GetByIdAsync(dto.DoctorId);
				if (doctor == null)
					return NotFound("Doctor not found!");

				// 3. IsAvailable check karo
				if (!doctor.IsAvailable)
					return BadRequest("Doctor is not available!");

				// 4. Time range check karo
				if (doctor.AvailableFrom.HasValue && doctor.AvailableTo.HasValue)
				{
					var appointmentTime = dto.AppointmentDate.TimeOfDay;
					if (appointmentTime < doctor.AvailableFrom.Value ||
						appointmentTime > doctor.AvailableTo.Value)
					{
						return BadRequest(
							$"Doctor is not available at this time! " +
							$"Available from {doctor.AvailableFrom} to {doctor.AvailableTo}.");
					}
				}

				// 5. Patient ki email automatically fetch karo
				var patientEmail = await _patientRepo.GetEmailByIdAsync(dto.PatientId);

				// 6. Appointment object banao
				var appointment = new Appointment
				{
					PatientId = dto.PatientId,
					DoctorId = dto.DoctorId,
					AppointmentDate = dto.AppointmentDate,
					Status = "Confirmed",
					PatientEmail = patientEmail
				};

				// 7. DB mein save karo
				await _repo.AddAsync(appointment);

				// 8. Service Bus mein message bhejo
				try
				{
					var connectionString = _config.GetConnectionString("ServiceBus");
					var queueName = _config["ServiceBus:QueueName"];

					await using var client = new ServiceBusClient(connectionString);
					var sender = client.CreateSender(queueName);

					var messageBody = JsonSerializer.Serialize(new
					{
						PatientId = appointment.PatientId,
						DoctorId = appointment.DoctorId,
						AppointmentDate = appointment.AppointmentDate,
						Status = "Confirmed",
						PatientEmail = patientEmail
					});

					await sender.SendMessageAsync(new ServiceBusMessage(messageBody));
				}
				catch (Exception sbEx)
				{
					return Ok($"Appointment booked successfully! " +
							  $"But email notification failed: {sbEx.Message}");
				}

				return Ok("Appointment booked successfully! Confirmation email will be sent.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error while booking appointment: {ex.Message}");
			}
		}
	}
}
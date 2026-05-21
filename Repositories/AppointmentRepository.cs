using HospitalManagementApi.Models;
using Microsoft.Data.SqlClient;

namespace HospitalManagementApi.Repositories
{
	public class AppointmentRepository : IAppointmentRepository
	{
		private readonly string? _connectionString;

		public AppointmentRepository(IConfiguration config)
		{
			_connectionString = config.GetConnectionString("HospitalDb");
		}

		public async Task<IEnumerable<Appointment>> GetAllAsync()
		{
			var list = new List<Appointment>();
			try
			{
				using var conn = new SqlConnection(_connectionString);
				await conn.OpenAsync();
				var cmd = new SqlCommand("SELECT * FROM Appointments", conn);
				var reader = await cmd.ExecuteReaderAsync();
				while (await reader.ReadAsync())
				{
					list.Add(new Appointment
					{
						AppointmentId = (int)reader["AppointmentId"],
						PatientId = (int)reader["PatientId"],
						DoctorId = (int)reader["DoctorId"],
						AppointmentDate = (DateTime)reader["AppointmentDate"],
						Status = reader["Status"].ToString()
					});
				}
			}
			catch (SqlException ex)
			{
				throw new Exception($"Database error while fetching appointments: {ex.Message}");
			}
			return list;
		}

		public async Task<Appointment?> GetByIdAsync(int id)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				await conn.OpenAsync();
				var cmd = new SqlCommand(
					"SELECT * FROM Appointments WHERE AppointmentId = @Id", conn);
				cmd.Parameters.AddWithValue("@Id", id);
				var reader = await cmd.ExecuteReaderAsync();
				if (await reader.ReadAsync())
				{
					return new Appointment
					{
						AppointmentId = (int)reader["AppointmentId"],
						PatientId = (int)reader["PatientId"],
						DoctorId = (int)reader["DoctorId"],
						AppointmentDate = (DateTime)reader["AppointmentDate"],
						Status = reader["Status"].ToString()
					};
				}
			}
			catch (SqlException ex)
			{
				throw new Exception($"Database error while fetching appointment by ID: {ex.Message}");
			}
			return null;
		}

		public async Task AddAsync(Appointment appointment)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				await conn.OpenAsync();
				var cmd = new SqlCommand(
					@"INSERT INTO Appointments (PatientId, DoctorId, AppointmentDate, Status)
                      VALUES (@PatientId, @DoctorId, @AppointmentDate, @Status)", conn);
				cmd.Parameters.AddWithValue("@PatientId", appointment.PatientId);
				cmd.Parameters.AddWithValue("@DoctorId", appointment.DoctorId);
				cmd.Parameters.AddWithValue("@AppointmentDate", appointment.AppointmentDate);
				cmd.Parameters.AddWithValue("@Status", appointment.Status ?? "Confirmed");
				await cmd.ExecuteNonQueryAsync();
			}
			catch (SqlException ex)
			{
				throw new Exception($"Database error while adding appointment: {ex.Message}");
			}
		}
	}
}
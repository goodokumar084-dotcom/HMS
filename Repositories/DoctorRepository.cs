using Microsoft.Data.SqlClient;
using HospitalManagementApi.Models;

namespace HospitalManagementApi.Repositories
{
	public class DoctorRepository : IDoctorRepository
	{
		private readonly string? _connectionString;

		public DoctorRepository(IConfiguration config)
		{
			_connectionString = config.GetConnectionString("HospitalDb");
		}

		public async Task<IEnumerable<Doctor>> GetAllAsync()
		{
			var doctors = new List<Doctor>();
			try
			{
				using var conn = new SqlConnection(_connectionString);
				await conn.OpenAsync();
				var cmd = new SqlCommand("SELECT * FROM Doctors", conn);
				var reader = await cmd.ExecuteReaderAsync();
				while (await reader.ReadAsync())
				{
					doctors.Add(new Doctor
					{
						DoctorId = (int)reader["DoctorId"],
						FullName = reader["FullName"].ToString(),
						Specialization = reader["Specialization"].ToString(),
						IsAvailable = (bool)reader["IsAvailable"],
						AvailableFrom = reader["AvailableFrom"] == DBNull.Value
							? null : (TimeSpan?)reader["AvailableFrom"],
						AvailableTo = reader["AvailableTo"] == DBNull.Value
							? null : (TimeSpan?)reader["AvailableTo"]
					});
				}
			}
			catch (SqlException ex)
			{
				throw new Exception($"Database error while fetching doctors: {ex.Message}");
			}
			return doctors;
		}

		public async Task<IEnumerable<Doctor>> GetAvailableAsync()
		{
			var doctors = new List<Doctor>();
			try
			{
				using var conn = new SqlConnection(_connectionString);
				await conn.OpenAsync();
				var cmd = new SqlCommand(
					"SELECT * FROM Doctors WHERE IsAvailable = 1", conn);
				var reader = await cmd.ExecuteReaderAsync();
				while (await reader.ReadAsync())
				{
					doctors.Add(new Doctor
					{
						DoctorId = (int)reader["DoctorId"],
						FullName = reader["FullName"].ToString(),
						Specialization = reader["Specialization"].ToString(),
						IsAvailable = (bool)reader["IsAvailable"],
						AvailableFrom = reader["AvailableFrom"] == DBNull.Value
							? null : (TimeSpan?)reader["AvailableFrom"],
						AvailableTo = reader["AvailableTo"] == DBNull.Value
							? null : (TimeSpan?)reader["AvailableTo"]
					});
				}
			}
			catch (SqlException ex)
			{
				throw new Exception($"Database error while fetching available doctors: {ex.Message}");
			}
			return doctors;
		}

		public async Task<Doctor?> GetByIdAsync(int id)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				await conn.OpenAsync();
				var cmd = new SqlCommand(
					"SELECT * FROM Doctors WHERE DoctorId = @Id", conn);
				cmd.Parameters.AddWithValue("@Id", id);
				var reader = await cmd.ExecuteReaderAsync();
				if (await reader.ReadAsync())
				{
					return new Doctor
					{
						DoctorId = (int)reader["DoctorId"],
						FullName = reader["FullName"].ToString(),
						Specialization = reader["Specialization"].ToString(),
						IsAvailable = (bool)reader["IsAvailable"],
						AvailableFrom = reader["AvailableFrom"] == DBNull.Value
							? null : (TimeSpan?)reader["AvailableFrom"],
						AvailableTo = reader["AvailableTo"] == DBNull.Value
							? null : (TimeSpan?)reader["AvailableTo"]
					};
				}
			}
			catch (SqlException ex)
			{
				throw new Exception($"Database error while fetching doctor by ID: {ex.Message}");
			}
			return null;
		}

		public async Task AddAsync(Doctor doctor)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				await conn.OpenAsync();
				var cmd = new SqlCommand(
					@"INSERT INTO Doctors (FullName, Specialization, IsAvailable, AvailableFrom, AvailableTo)
                      VALUES (@FullName, @Specialization, @IsAvailable, @AvailableFrom, @AvailableTo)", conn);
				cmd.Parameters.AddWithValue("@FullName", doctor.FullName ?? "");
				cmd.Parameters.AddWithValue("@Specialization", doctor.Specialization ?? "");
				cmd.Parameters.AddWithValue("@IsAvailable", doctor.IsAvailable);
				cmd.Parameters.AddWithValue("@AvailableFrom", (object?)doctor.AvailableFrom ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@AvailableTo", (object?)doctor.AvailableTo ?? DBNull.Value);
				await cmd.ExecuteNonQueryAsync();
			}
			catch (SqlException ex)
			{
				throw new Exception($"Database error while adding doctor: {ex.Message}");
			}
		}

		public async Task UpdateAsync(Doctor doctor)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				await conn.OpenAsync();
				var cmd = new SqlCommand(
					@"UPDATE Doctors SET
                        FullName = @FullName,
                        Specialization = @Specialization,
                        IsAvailable = @IsAvailable,
                        AvailableFrom = @AvailableFrom,
                        AvailableTo = @AvailableTo
                      WHERE DoctorId = @DoctorId", conn);
				cmd.Parameters.AddWithValue("@FullName", doctor.FullName ?? "");
				cmd.Parameters.AddWithValue("@Specialization", doctor.Specialization ?? "");
				cmd.Parameters.AddWithValue("@IsAvailable", doctor.IsAvailable);
				cmd.Parameters.AddWithValue("@AvailableFrom", (object?)doctor.AvailableFrom ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@AvailableTo", (object?)doctor.AvailableTo ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@DoctorId", doctor.DoctorId);
				await cmd.ExecuteNonQueryAsync();
			}
			catch (SqlException ex)
			{
				throw new Exception($"Database error while updating doctor: {ex.Message}");
			}
		}

		public async Task DeleteAsync(int id)
		{
			try
			{
				using var conn = new SqlConnection(_connectionString);
				await conn.OpenAsync();
				var cmd = new SqlCommand(
					"DELETE FROM Doctors WHERE DoctorId = @Id", conn);
				cmd.Parameters.AddWithValue("@Id", id);
				await cmd.ExecuteNonQueryAsync();
			}
			catch (SqlException ex)
			{
				throw new Exception($"Database error while deleting doctor: {ex.Message}");
			}
		}
	}
}
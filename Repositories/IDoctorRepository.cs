using HospitalManagementApi.Models;

namespace HospitalManagementApi.Repositories
{
	public interface IDoctorRepository
	{
		Task<IEnumerable<Doctor>> GetAllAsync();
		Task<IEnumerable<Doctor>> GetAvailableAsync();
		Task<Doctor?> GetByIdAsync(int id);
		Task AddAsync(Doctor doctor);
		Task UpdateAsync(Doctor doctor);
		Task DeleteAsync(int id);
	}
}
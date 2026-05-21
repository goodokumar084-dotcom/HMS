using HospitalManagementApi.Models;

namespace HospitalManagementApi.Repositories
{
	public interface IAppointmentRepository
	{
		Task<IEnumerable<Appointment>> GetAllAsync();
		Task<Appointment?> GetByIdAsync(int id);
		Task AddAsync(Appointment appointment);
	}
}
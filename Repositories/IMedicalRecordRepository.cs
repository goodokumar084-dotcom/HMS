using HospitalManagementApi.Models;

namespace HospitalManagementApi.Repositories
{
	public interface IMedicalRecordRepository
	{
		Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId);
		Task AddAsync(MedicalRecord record);
	}
}
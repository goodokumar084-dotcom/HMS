namespace HospitalManagementApi.Models
{
	public class MedicalRecord
	{
		public int RecordId { get; set; }
		public int PatientId { get; set; }
		public int DoctorId { get; set; }
		public string? Diagnosis { get; set; }
		public string? Prescription { get; set; }
		public DateTime RecordDate { get; set; }
	}
}
namespace HospitalManagementApi.Models
{
	public class Doctor
	{
		public int DoctorId { get; set; }
		public string? FullName { get; set; }
		public string? Specialization { get; set; }
		public bool IsAvailable { get; set; }
		public TimeSpan? AvailableFrom { get; set; }
		public TimeSpan? AvailableTo { get; set; }
	}
}
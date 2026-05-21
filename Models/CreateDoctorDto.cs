namespace HospitalManagementApi.Models
{
	public class CreateDoctorDto
	{
		public string? FullName { get; set; }
		public string? Specialization { get; set; }
		public bool IsAvailable { get; set; }
		public TimeSpan? AvailableFrom { get; set; }
		public TimeSpan? AvailableTo { get; set; }
	}
}
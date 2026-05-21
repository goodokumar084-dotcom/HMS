namespace HospitalManagementApi.Models
{
	public class Patient
	{
		public int PatientId { get; set; }
		public string? FullName { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public DateOnly DateOfBirth { get; set; }
	}
}
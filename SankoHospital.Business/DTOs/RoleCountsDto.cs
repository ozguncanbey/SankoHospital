namespace SankoHospital.Business.DTOs;

public class RoleCountsDto
{
    public int TotalUsers { get; set; }
    public int AdminCount { get; set; }
    public int UserCount { get; set; }
    public int ReceptionistCount { get; set; }
    public int NurseCount { get; set; }
    public int CleanerCount { get; set; }
}
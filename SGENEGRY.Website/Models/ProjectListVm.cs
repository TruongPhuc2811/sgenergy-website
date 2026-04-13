namespace SGENERGY.Website.Models;

public class ProjectListVm
{
    public string? HeadingSmall { get; set; }
    public string HeadingLarge { get; set; } = "DỰ ÁN ĐÃ TRIỂN KHAI";

    public List<ProjectCardVm> Projects { get; set; } = [];

    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
}

public class ProjectCardVm
{
    public string Title { get; set; } = default!;
    public string ImageUrl { get; set; } = default!;
    public string DetailUrl { get; set; } = "#";
}
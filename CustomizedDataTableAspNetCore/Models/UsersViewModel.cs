using CustomizedDataTableAspNetCore.Database.Entities;

namespace CustomizedDataTableAspNetCore.Models
{
    public class UsersViewModel
    {
        public int PageNo { get; set; }
        public int RowsPerPage { get; set; }

        public int TotalCount { get; set; }
        public int StartFrom { get; set; }
        public string? Search { get; set; }
        public string? Role { get; set; }

        public string? OrderBy { get; set; }
        public string? Direction { get; set; }

        public int ExportAll { get; set; }
        public List<User> _Items { get; set; } = new();
    }
}

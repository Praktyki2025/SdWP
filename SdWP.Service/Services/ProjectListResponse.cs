using SdWP.DTO.Responses;

namespace SdWP.Service.Services
{
    public class ProjectListResponse<T>
    {
        public List<T> Projects { get; set; }
        public int TotalCount { get; set; }
        public bool HasMore { get; set; }
    }
}

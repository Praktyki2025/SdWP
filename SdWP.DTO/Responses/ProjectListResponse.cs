using SdWP.DTO.Requests;

namespace SdWP.DTO.Responses
{
    public class ProjectListResponse<T>
    {
        public List<T> Projects { get; set; }
        public int TotalCount { get; set; }
        public bool HasMore { get; set; }
    }
}

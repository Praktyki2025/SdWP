using Microsoft.AspNetCore.Identity;
using SdWP.Data.Models;
using SdWP.DTO.Requests.Datatable;
using SdWP.DTO.Responses.ProjectRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.Data.IData
{
    public enum UserRole
    {
        Unknown,
        Admin,
        User
    }

    public interface IProjectRepository
    {
        Task AddAsync(Project project);
        Task<Project?> GetProjectByIdAsync(Guid id);
        Task UpdateAsync(Project project);
        Task DeleteAsync(Guid id);
        Task<ProjectListResponse<ProjectResponse>> FilterAsync(DataTableRequest request, UserRole userRole, Guid userId);
    }
}
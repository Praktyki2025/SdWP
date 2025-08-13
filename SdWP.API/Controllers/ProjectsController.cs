using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SdWP.Data.Models;
using SdWP.DTO.Requests.Datatable;
using SdWP.DTO.Requests.ProjectRequests;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;

namespace SdWP.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ProjectCreateRequest dto)
        {
            var result = await _projectService.CreateProjectAsync(dto);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result.Data);
            }

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromBody] ProjectEditRequest dto)
        {
            var result = await _projectService.EditProjectAsync(dto);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result.Data);
            }

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var project = await _projectService.GetProjectAsync(id);
            if (project.Success)
            {
                var result = await _projectService.DeleteProjectAsync(project.Data.MapToDeleteRequest());
                if (result.Success)
                {
                    return StatusCode(result.StatusCode, result.Data);
                }
                else
                {

                    return StatusCode(result.StatusCode, new
                    {
                        success = false,
                        message = result.Message,
                        errors = result.Errors
                    });
                }
            }
            else
            {
                return StatusCode(project.StatusCode, new
                {
                    success = false,
                    message = project.Message,
                    errors = project.Errors
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _projectService.GetProjectAsync(id);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result.Data);
            }

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetProjects([FromBody] DataTableRequest request)
        {
            var result = await _projectService.GetProjects(request);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, new
                {
                    recordsTotal = result.Data.TotalCount,
                    recordsFiltered = result.Data.TotalCount,
                    data = result.Data.Projects
                }
                );
            }

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }
    }
}

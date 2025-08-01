using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost("create")] // Create
        public async Task<ActionResult<ProjectUpsertResponseDTO>> Create([FromBody] ProjectUpsertRequestDTO dto)
        {
            var result = await _projectService.CreateProjectAsync(dto);
            return Ok(result);
        }

        [HttpPost("edit")] // Edit
        public async Task<ActionResult<ProjectUpsertResponseDTO>> Edit([FromBody] ProjectUpsertRequestDTO dto)
        {
            var result = await _projectService.EditProjectAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ProjectDeleteResponseDTO>> Delete(Guid id)
        {
            var result = await _projectService.DeleteProjectAsync(id);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectUpsertResponseDTO>> GetById(Guid id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null)
                return NotFound();
            return Ok(project);
        }

        [HttpPost("all")]
        public async Task<IActionResult> GetProjects([FromBody] DataTableRequest request)
        {
            var (projects, totalRecords) = await _projectService.GetProjects(request);

            return Ok(new
            {
                draw = request.draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = projects
            });
        }
    }
}

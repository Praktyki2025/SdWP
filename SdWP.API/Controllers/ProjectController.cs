using Microsoft.AspNetCore.Mvc;
using SdWP.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;

namespace SdWP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectInteractionsService _projectService;

        public ProjectController(IProjectInteractionsService projectService)
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

        [HttpGet("all")]
        public async Task<ActionResult<List<ProjectUpsertRequestDTO>>> GetAll()
        {
            var projects = await _projectService.GetAllAsync();
            return Ok(projects);
        }
    }
}

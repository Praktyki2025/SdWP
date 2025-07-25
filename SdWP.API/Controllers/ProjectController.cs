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

        [HttpPost("save")] // Create/Edit
        public async Task<ActionResult<ProjectResponseDTO>> Save([FromBody] ProjectUpsertRequestDTO dto)
        {
            var result = await _projectService.SaveProjectAsync(dto);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<ActionResult<DeleteResponseDTO>> Delete([FromBody] ProjectDeleteDTO dto)
        {
            var result = await _projectService.DeleteProjectAsync(dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectResponseDTO>> GetById(Guid id)
        {
            var project = await _projectService.GetByIdAsync(id);
            if (project == null)
                return NotFound();
            return Ok(project);
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectResponseDTO>>> GetAll()
        {
            var projects = await _projectService.GetAllAsync();
            return Ok(projects);
        }
    }
}

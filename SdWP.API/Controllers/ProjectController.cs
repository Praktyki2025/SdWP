using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SdWP.Data.Models;
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
        public async Task<IActionResult> Create([FromBody] ProjectUpsertRequestDTO dto)
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

        [HttpPost("edit")] // Edit
        public async Task<IActionResult> Edit([FromBody] ProjectUpsertRequestDTO dto)
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
            var result = await _projectService.DeleteProjectAsync(id);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _projectService.GetByIdAsync(id);
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

        [HttpPost("all")]
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

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
        public IActionResult GetProjects([FromBody] DataTableRequest request)
        {
            var projects = _projectService.GetProjects();

            //search
            if (!string.IsNullOrWhiteSpace(request.search?.value))
            {
                var searchLower = request.search.value.ToLower();
                projects = projects.Where(p =>
                    (!string.IsNullOrEmpty(p.Title) && p.Title.ToLower().Contains(searchLower)) ||
                    (!string.IsNullOrEmpty(p.Description) && p.Description.ToLower().Contains(searchLower)));
            }

            //sorting
            if (request.order != null && request.order.Count > 0)
            {
                var order = request.order[0];
                bool ascending = order.dir == "asc";
                string? sortColumn = null;
                if (request.columns != null && request.columns.Count > order.column)
                {
                    sortColumn = request.columns[order.column].data;
                }

                if (!string.IsNullOrEmpty(sortColumn))
                {
                    projects = ApplyOrdering(projects, sortColumn, ascending);
                }
            }

            var totalRecords = projects.Count();

            var data = projects
                .Skip(request.start)
                .Take(request.length)
                .ToList();

            return Ok(new
            {
                draw = request.draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            });
        }
        
        //sorting fn
        private IQueryable<ProjectUpsertResponseDTO> ApplyOrdering(IQueryable<ProjectUpsertResponseDTO> source, string propertyName, bool ascending)
        {
            if (string.IsNullOrEmpty(propertyName))
                return source;

            //normalize prop
            propertyName = FirstCharToUpper(propertyName);

            var param = Expression.Parameter(typeof(ProjectUpsertResponseDTO), "p");
            var property = Expression.PropertyOrField(param, propertyName);
            var sortLambda = Expression.Lambda(property, param);

            string methodName = ascending ? "OrderByDescending" : "OrderBy";

            var result = typeof(Queryable).GetMethods()
                .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                .Single()
                .MakeGenericMethod(typeof(ProjectUpsertResponseDTO), property.Type)
                .Invoke(null, new object[] { source, sortLambda });

            return (IQueryable<ProjectUpsertResponseDTO>)result;
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}

﻿using Domain.DTOs.Department;
using Domain.Interfaces.Services;
using Domain.ResourceParameters;
using Domain.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Domain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<ActionResult<GetBaseResponse<DepartmentDto>>> GetDepartmentsAsync(
            [FromQuery] DepartmentResourceParameters departmentResource)
        {
            var departments = await _departmentService.GetDepartmentsAsync(departmentResource);

            return Ok(departments);
        }

        [HttpGet("{id}", Name = "GetDepartmentById")]
        public async Task<ActionResult<DepartmentDto>> GetDepartmentByIdAsync(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);

            return Ok(department);
        }

        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> PostAsync([FromBody] DepartmentCreateDto departmentCreateDto)
        {
            var createdDepartment = await _departmentService.CreateDepartmentAsync(departmentCreateDto);

            return CreatedAtAction("GetDepartmentById", new { id = createdDepartment.Id }, createdDepartment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(int id, [FromBody] DepartmentUpdateDto departmentUpdateDto)
        {
            if (id != departmentUpdateDto.Id)
            {
                return BadRequest($"Route id: {id} does not match with parameter id: {departmentUpdateDto.Id}.");
            }

            var updatedDepartment = await _departmentService.UpdateDepartmentAsync(departmentUpdateDto);

            return Ok(updatedDepartment);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await _departmentService.DeleteDepartmentAsync(id);

            return NoContent();
        }
    }
}

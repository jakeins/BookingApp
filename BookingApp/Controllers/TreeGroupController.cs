using AutoMapper;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Helpers;
using BookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BookingApp.Controllers
{
    /// <remarks>
    /// This class-controller can add, edit, delete and getting TreeGroup.
    /// </remarks>
    public class TreeGroupController : EntityControllerBase
    {
        TreeGroupService service;
        readonly IMapper mapper;

        public TreeGroupController(TreeGroupService s)
        {
            service = s;
            mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TreeGroup, TreeGroupBaseDto>();
                cfg.CreateMap<TreeGroup, TreeGroupMinimalDto>().ReverseMap();
            }));
        }

        /// <summary>
        /// Creating TreeGroup
        /// <summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Route("api/tree-group")]
        public async Task<IActionResult> Index()
        {
            var models = (IsAdmin) ? await service.GetTreeGroups() : await service.GetTreeGroupsActive();
            IEnumerable <TreeGroupBaseDto> dtos = mapper.Map<IEnumerable<TreeGroupBaseDto>>(models);
            return Ok(dtos);
        }

        /// <summary>
        /// Getting TreeGroup on id
        /// <summary>
        /// <param name="id">Id TreeGroup.</param>
        /// <response code="200">Success</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = RoleTypes.Admin)]
        [Route("api/tree-group/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            TreeGroupBaseDto treeGroupDto = mapper.Map<TreeGroupBaseDto>(await service.GetDetail(id));
            return Ok(treeGroupDto);
        }

        /// <summary>
        /// Creating TreeGroup
        /// <summary>
        /// <param name="treeGroupDto">Tdo model TreeGroupCrUpDto.</param>
        /// <response code="201">Success created</response>
        /// <response code="400">Invalid argument</response>
        /// <response code="404">Resources or rule not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Route("api/tree-group")]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Create([FromBody]TreeGroupMinimalDto treeGroupDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var itemModel = mapper.Map<TreeGroup>(treeGroupDto);

            await service.Create(UserId, itemModel);
            return Created(
                this.BaseApiUrl + "/" + itemModel.Id,
                new { ResourceId = itemModel.Id }
            );
        }

        /// <summary>
        /// Updating TreeGroup
        /// <summary>
        /// <param name="treeGroupDto">Tdo model TreeGroupCrUpDto.</param>
        /// <response code="200">Success update</response>
        /// <response code="401">Error. Only admin and owner can update booking data</response>
        /// <respomse code="404">Error. Non exist booking id passed</respomse>
        /// <response code="500">Internal server error</response>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Route("api/tree-group/{id}")]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody]TreeGroupMinimalDto treeGroupDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            } 
            await service.Update(id, UserId, mapper.Map<TreeGroup>(treeGroupDto));
            return Ok("TreeGroup updated successfully.");  
        }

        /// <summary>
        /// Deleting TreeGroup
        /// <summary>
        /// <param name="tree">Tdo model TreeGroupCrUpDto.</param>
        /// <response code="200">Success deleted</response>
        /// <response code="401">Error. Only admin and owner can update booking data</response>
        /// <respomse code="404">Error. Non exist booking id passed</respomse>
        /// <response code="500">Error. Internal server</response>
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Route("api/tree-group/{id}")]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await service.Delete(id);
            return Ok("TreeGroup deleted.");
        }

    }
}

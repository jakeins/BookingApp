using AutoMapper;
using BookingApp.Controllers.Bases;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.DTOs.Folder;
using BookingApp.Helpers;
using BookingApp.Services;
using BookingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Controllers
{
    /// <remarks>
    /// This class-controller can add, edit, delete and getting Folder.
    /// </remarks>
    public class FolderController : EntityControllerBase
    {
        IFolderService service;
        readonly IMapper mapper;

        public FolderController(IFolderService s)
        {
            service = s;
            mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Folder, FolderBaseDto>();
                cfg.CreateMap<Folder, FolderMinimalDto>().ReverseMap();
            }));
        }

        /// <summary>
        /// Creating Folder
        /// <summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Route("api/folder")]
        public async Task<IActionResult> Index()
        {
            //IsAdmin
            var models = (IsAdmin) ? await service.GetFolders() : await service.GetFoldersActive();
            IEnumerable <FolderBaseDto> dtos = mapper.Map<IEnumerable<FolderBaseDto>>(models);
            return Ok(dtos);
        }

        /// <summary>
        /// Getting Folder on id
        /// <summary>
        /// <param name="id">Id Folder.</param>
        /// <response code="200">Success</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Route("api/folder/{id}")]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Detail(int id)
        {
            FolderBaseDto FolderDto = mapper.Map<FolderBaseDto>(await service.GetDetail(id));
            return Ok(FolderDto);
        }

        /// <summary>
        /// Creating Folder
        /// <summary>
        /// <param name="FolderDto">Tdo model FolderCrUpDto.</param>
        /// <response code="201">Success created</response>
        /// <response code="400">Invalid argument</response>
        /// <response code="404">Resources or rule not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Route("api/folder")]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Create([FromBody]FolderMinimalDto FolderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var itemModel = mapper.Map<Folder>(FolderDto);

            await service.Create(UserId, itemModel);
            return Created(
                this.BaseApiUrl + "/" + itemModel.Id,
                new { FolderId = itemModel.Id }
            );
        }

        /// <summary>
        /// Updating Folder
        /// <summary>
        /// <param name="FolderDto">Tdo model FolderCrUpDto.</param>
        /// <response code="200">Success update</response>
        /// <response code="401">Error. Only admin and owner can update booking data</response>
        /// <respomse code="404">Error. Non exist booking id passed</respomse>
        /// <response code="500">Internal server error</response>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Route("api/folder/{id}")]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody]FolderMinimalDto FolderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            } 
            await service.Update(id, UserId, mapper.Map<Folder>(FolderDto));
            return Ok("Folder updated successfully.");  
        }

        /// <summary>
        /// Deleting Folder
        /// <summary>
        /// <param name="tree">Tdo model FolderCrUpDto.</param>
        /// <response code="200">Success deleted</response>
        /// <response code="401">Error. Only admin and owner can update booking data</response>
        /// <respomse code="404">Error. Non exist booking id passed</respomse>
        /// <response code="500">Error. Internal server</response>
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Route("api/folder/{id}")]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await service.Delete(id);
            return Ok("Folder deleted.");
        }
    }
}
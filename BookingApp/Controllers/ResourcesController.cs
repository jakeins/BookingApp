using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookingApp.Data.Models;
using BookingApp.Services.Interfaces;
using AutoMapper;
using BookingApp.DTOs;
using BookingApp.Exceptions;
using BookingApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using BookingApp.Services;
using BookingApp.Controllers.Bases;
using BookingApp.DTOs.Resource;

namespace BookingApp.Controllers
{
    [Route("api/resources")]
    [ApiController]
    public partial class ResourcesController : EntityControllerBase
    {
        readonly IResourcesService resService;
        readonly IBookingsService bookService;
        readonly IMapperService mappService;

        public ResourcesController(IResourcesService resService, IBookingsService bookService, IMapperService mappService)
        {
            this.resService = resService;
            this.bookService = bookService;
            this.mappService = mappService;
        }

        #region GETs
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        // Filtered access: Guest/Admin. 
        public async Task<IActionResult> List()
        {
            var models = IsAdmin ? await resService.GetList() : await resService.ListActive();
            var dtos = mappService.Map<IEnumerable<ResourceBriefDto>>(models);
            return Ok(dtos);
        }

        [HttpGet("occupancy")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        // Filtered access: Guest/Admin.
        public async Task<IActionResult> ListOccupancy()
        {
            var idsList = await (IsAdmin ? resService.ListKeys() : resService.ListActiveKeys());

            var map = new Dictionary<int, double?>();

            foreach (int resourceId in idsList)
            {
                map.Add(resourceId, null);

                try
                {
                    map[resourceId] = await bookService.OccupancyByResource(resourceId);
                }
                catch (Exception ex) when (ex is KeyNotFoundException || ex is FieldValueAbsurdException)
                {
                    //silently swallowing disjoint values
                }
            }

            return Ok(map);
        }

        [HttpGet("{resourceId}/bookings")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        // Filtered access: Guest/Owner/Admin.
        public async Task<IActionResult> ListRelatedBookings([FromRoute] int resourceId)
        {
            await AuthorizeForSingleResource(resourceId);

            var models = await bookService.ListBookingOfResource(resourceId, IsAdmin);

            IEnumerable<BookingMinimalDTO> dtos;

            if (IsAdmin)
            {
                dtos = mappService.Map<IEnumerable<BookingAdminDTO>>(models);
            }
            else
            {
                if (IsUser && models.Any(b => b.CreatedUserId == UserId))
                {
                    var diffList = new List<BookingMinimalDTO>();
                    var currentUserId = UserId;
                    foreach (var model in models)
                    {
                        BookingMinimalDTO suitableDto;

                        if (model.CreatedUserId == currentUserId)
                            suitableDto = mappService.Map<BookingOwnerDTO>(model);
                        else
                            suitableDto = mappService.Map<BookingMinimalDTO>(model);

                        diffList.Add(suitableDto);
                    }
                    dtos = diffList;
                }
                else
                {
                    dtos = mappService.Map<IEnumerable<BookingMinimalDTO>>(models);
                }
            }
            return Ok(dtos);
        }

        [HttpGet("{resourceId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        // Filtered access: Guest/Admin.
        public async Task<IActionResult> Single([FromRoute] int resourceId)
        {
            await AuthorizeForSingleResource(resourceId);
            var resourceModel = await resService.Get(resourceId);
            var resourceDTO = mappService.Map<ResourceMaxDto>(resourceModel);
            return Ok(resourceDTO);
        }

        [HttpGet("{resourceId}/occupancy")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        // Filtered access: Guest/Admin.
        public async Task<IActionResult> SingleOccupancy([FromRoute] int resourceId)
        {
            await AuthorizeForSingleResource(resourceId);
            return Ok(await bookService.OccupancyByResource(resourceId));
        }
        #endregion

        #region POST / PUT
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Create([FromBody] ResourceDetailedDto item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #region Mapping
            var itemModel = mappService.Map<Resource>(item);
            itemModel.UpdatedUserId = itemModel.CreatedUserId = UserId;
            itemModel.UpdatedTime = itemModel.CreatedTime = DateTime.Now;
            #endregion

            await resService.Create(itemModel);

            return Created(
                this.BaseApiUrl + "/" + itemModel.Id,
                new { ResourceId = itemModel.Id, itemModel.CreatedTime }
            );
        }

        [HttpPut("{resourceId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Update([FromRoute] int resourceId, [FromBody] ResourceDetailedDto item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #region Mapping
            var itemModel = mappService.Map<Resource>(item);
            itemModel.UpdatedUserId = UserId;
            itemModel.UpdatedTime = DateTime.Now;
            itemModel.Id = resourceId;
            #endregion

            await resService.Update(itemModel);
            return Ok(new { itemModel.UpdatedTime });
        }
        #endregion

        #region DELETE
        [HttpDelete("{resourceId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Delete([FromRoute] int resourceId)
        {
            await resService.Delete(resourceId);
            return Ok(new { DeletedTime = DateTime.Now });
        }
        #endregion

        #region Helpers

        /// <summary>
        /// Not found exception factory
        /// </summary>
        CurrentEntryNotFoundException NewNotFoundException => new CurrentEntryNotFoundException("Specified resource not found");

        /// <summary>
        /// Gateway for the single resource. 
        /// Throws Not Found if current user hasn't enough rights for viewing the specified resource.
        /// Throws Not Found if current resource not found.
        /// </summary>
        [NonAction]
        public async Task AuthorizeForSingleResource(int resourceId)
        {
            bool isAuthorized = IsAdmin || await resService.IsActive(resourceId);

            if (!isAuthorized)
                throw NewNotFoundException;// Excuse
        }
        #endregion
    }
}
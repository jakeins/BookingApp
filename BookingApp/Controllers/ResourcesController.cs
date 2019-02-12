using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookingApp.Data.Models;
using BookingApp.Services;
using AutoMapper;
using BookingApp.DTOs;
using BookingApp.Exceptions;
using Microsoft.AspNetCore.Identity;
using BookingApp.Helpers;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System;

namespace BookingApp.Controllers
{
    [Route("api/resources")]
    [ApiController]
    public partial class ResourcesController : ControllerBase
    {
        readonly ResourcesService resService;
        readonly BookingsService bookService;
        readonly UserManager<ApplicationUser> userManager;
        readonly RoleManager<IdentityRole> roleManager;
        readonly IMapper dtoMapper;

        public ResourcesController(ResourcesService resService, BookingsService bookService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.resService = resService;
            this.bookService = bookService;
            this.userManager = userManager;
            this.roleManager = roleManager;

            dtoMapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Resource, ResourceBriefDto>();
                cfg.CreateMap<Resource, ResourceMaxDto>();
                cfg.CreateMap<ResourceDetailedDto,Resource>();
            }));
        }

        #region GETs
        // GET: api/Resources
        // Filtered access: Guest/Admin. 
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var models = await resService.List(includeInactiveResources: AdminAccess == true);
            var dtos = dtoMapper.Map<IEnumerable<ResourceBriefDto>>(models);
            return Ok(dtos);
        }

        // GET: api/Resources/Occupancy
        // Filtered access: Guest/Admin.
        [HttpGet("occupancy")]
        public async Task<IActionResult> ListOccupancy()
        {
            var idsList = await resService.ListIDs(includeIncativeResources: AdminAccess == true);

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

        // GET: api/Resources/5
        // Filtered access: Guest/Admin. 
        [HttpGet("{resourceId}")]
        public async Task<IActionResult> Single([FromRoute] int resourceId)
        {
            await AuthorizeForSingleResource(resourceId);

            var resourceModel = await resService.Single(resourceId);
            var resourceDTO = dtoMapper.Map<ResourceMaxDto>(resourceModel);
            return Ok(resourceDTO);
        }

        // GET: api/Resources/5/Occupancy
        // Filtered access: Guest/Admin.
        [HttpGet("{resourceId}/occupancy")]
        public async Task<IActionResult> SingleOccupancy([FromRoute] int resourceId)
        {
            await AuthorizeForSingleResource(resourceId);
            return Ok(await bookService.OccupancyByResource(resourceId));
        }
        #endregion

        #region POST / PUT
        // POST: api/Resources
        [HttpPost]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Create([FromBody] ResourceDetailedDto item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #region Mapping
            var itemModel = dtoMapper.Map<Resource>(item);
            itemModel.UpdatedUserId = itemModel.CreatedUserId = UserId;
            itemModel.UpdatedTime = itemModel.CreatedTime = DateTime.Now;
            #endregion

            await resService.Create(itemModel);

            return Ok(itemModel.ResourceId);
        }

        // PUT: api/Resources/5
        [HttpPut("{resourceId}")]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Update([FromRoute] int resourceId, [FromBody] ResourceDetailedDto item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #region Mapping
            var itemModel = dtoMapper.Map<Resource>(item);
            itemModel.UpdatedUserId = UserId;
            itemModel.UpdatedTime = DateTime.Now;
            itemModel.ResourceId = resourceId;
            #endregion

            await resService.Update(itemModel);
            return Ok("Resource updated successfully");
        }
        #endregion

        #region DELETE
        // DELETE: api/Resources/5
        [HttpDelete("{resourceId}")]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Delete([FromRoute] int resourceId)
        {
            await resService.Delete(resourceId);
            return Ok("Resource deleted");
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Current user identifier
        /// </summary>
        string UserId => User.Claims.Single(c => c.Type == "uid").Value;

        /// <summary>
        /// Not found exception factory
        /// </summary>
        CurrentEntryNotFoundException NewNotFoundException => new CurrentEntryNotFoundException("Specified resource not found");

        /// <summary>
        /// Shorthand for checking if current user has admin access level
        /// </summary>
        bool AdminAccess => User.IsInRole(RoleTypes.Admin);

        /// <summary>
        /// Gateway for the single resource. Throws Not Found if current user hasn't enough rights for viewing the specified resource.
        /// </summary>
        async Task AuthorizeForSingleResource(int resourceId)
        {
            bool isAuthorized = AdminAccess || await resService.IsActive(resourceId);

            if (!isAuthorized)
                throw NewNotFoundException;// Excuse
        }
        #endregion
    }
}
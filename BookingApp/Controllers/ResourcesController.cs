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

namespace BookingApp.Controllers
{
    [Route("api/resources")]
    [ApiController]
    public class ResourcesController : ControllerBase
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
                cfg.CreateMap<Resource, ResourceMinimalDto>();
                cfg.CreateMap<Resource, ResourceDetailedDto>().ReverseMap();
            }));
        }

        #region GETs
        // GET: api/Resources
        // Filtered access: Guest/Admin. 
        [HttpGet]
        public async Task<IActionResult> List()
        {
            bool adminAccess = await CurrentUserHasRole(RoleTypes.Admin);

            var models = await resService.ListResources(includeInactives: adminAccess == true);

            var dtos = dtoMapper.Map<IEnumerable<ResourceMinimalDto>>(models);

            return Ok(dtos);
        }

        // GET: api/Resources/Occupancy
        // Filtered access: Guest/Admin.
        [HttpGet("occupancy")]
        public async Task<IActionResult> ListOccupancy()
        {
            bool adminAccess = await CurrentUserHasRole(RoleTypes.Admin);

            var idsList = await resService.ListIDsAsync(includeIncatives: adminAccess == true);

            var map = new Dictionary<int, double?>();

            foreach (int resourceId in idsList)
            {
                map.Add(resourceId, null);

                try
                {
                    map[resourceId] = await bookService.OccupancyByResource(resourceId);
                }
                catch (KeyNotFoundException)
                {
                }
                catch (FieldValueAbsurdException)
                {
                }
            }

            return Ok(map);
        }

        // GET: api/Resources/5
        // Filtered access: Guest/Admin. 
        [HttpGet("{resourceId}")]
        public async Task<IActionResult> Single([FromRoute] int resourceId)
        {
            bool isAuthorized = await CurrentUserHasRole(RoleTypes.Admin) || await resService.IsActive(resourceId);

            if (isAuthorized && await resService.SingleResource(resourceId) is Resource resourceModel)
            {
                var resourceDTO = dtoMapper.Map<ResourceDetailedDto>(resourceModel);
                return Ok(resourceDTO);
            }
            else
                return NotFound("Requested resource not found."); 
        }

        // GET: api/Resources/5/Occupancy
        // Filtered access: Guest/Admin.
        [HttpGet("{resourceId}/occupancy")]
        public async Task<IActionResult> SingleOccupancy([FromRoute] int resourceId)
        {
            try
            {
                bool isAuthorized = await CurrentUserHasRole(RoleTypes.Admin) || await resService.IsActive(resourceId);

                if (!isAuthorized)
                    throw new KeyNotFoundException();// fake excuse for keeping security of inactive resources

                return Ok(await bookService.OccupancyByResource(resourceId));
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Requested resource not found.");
            }
            catch (FieldValueAbsurdException)
            {
                return BadRequest("There is a problem with the resource booking policy.");
            }
        }
        #endregion

        #region POST / PUT / DELETE
        // POST: api/Resources
        [HttpPost]
        //[Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Create([FromBody] ResourceDetailedDto item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var itemModel = dtoMapper.Map<Resource>(item);
            itemModel.ResourceId = 0;
            itemModel.UpdatedUserId = itemModel.CreatedUserId = await GetCurrentUserId();

            await resService.Create(itemModel);

            return Ok("Resource created successfully.");
        }

        // PUT: api/Resources/5
        [HttpPut("{resourceId}")]
        //[Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Update([FromRoute] int resourceId, [FromBody] ResourceDetailedDto item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (resourceId != item.ResourceId)
                return BadRequest("Resource identifiers inconsistent.");

            try
            {
                var model = dtoMapper.Map<Resource>(item);
                model.UpdatedUserId = await GetCurrentUserId();

                await resService.Update(model);
                return Ok("Resource updated successfully.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Requested resource not found.");
            }
            catch (OperationFailedException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Resources/5
        [HttpDelete("{resourceId}")]
        //[Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Delete([FromRoute] int resourceId)
        {
            try
            {
                await resService.Delete(resourceId);
                return Ok("Resource deleted.");
            }
            catch (EntryNotFoundException)
            {
                return NotFound("Requested resource not found.");
            }
            catch (OperationRestrictedException)
            {
                return BadRequest("Cannot delete this resource. Some bookings rely on it.");
            }
        }
        #endregion

        #region Utilities
        async Task<ApplicationUser> GetCurrentUserMOCK() => await userManager.FindByNameAsync("SuperAdmin");

        async Task<string> GetCurrentUserId() => (await GetCurrentUserMOCK()).Id;
        async Task<IEnumerable<string>> GetCurrentUserRoles()
        {
            var result = new List<string>();
            var currentUser = await GetCurrentUserMOCK();

            foreach (var roleName in new[] { RoleTypes.User, RoleTypes.Admin })
                if (await userManager.IsInRoleAsync(currentUser, roleName))
                    result.Add(roleName);

            return result;
        }
        async Task<bool> CurrentUserHasRole(string role) => (await GetCurrentUserRoles()).Contains(role);
        #endregion
    }
}
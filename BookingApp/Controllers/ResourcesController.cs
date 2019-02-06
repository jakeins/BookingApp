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

namespace BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
        readonly ResourcesService resourcesService;
        readonly UserManager<ApplicationUser> userManager;
        readonly RoleManager<IdentityRole> roleManager;
        readonly IMapper dtoMapper;

        public ResourcesController(ResourcesService resourcesService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.resourcesService = resourcesService;
            this.userManager = userManager;
            this.roleManager = roleManager;

            dtoMapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Resource, ResourceMinimalDto>();
                cfg.CreateMap<Resource, ResourceDetailedDto>().ReverseMap();
            }));
        }

        #region CRUD actions
        // GET: api/Resources
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var modelsList = await resourcesService.GetList(includeInactives: await IsCurrentUserAdmin());

            var dtosList = dtoMapper.Map<IEnumerable<ResourceMinimalDto>>(modelsList);

            return Ok(dtosList);
        }

        // GET: api/Resources/5
        [HttpGet("{resourceId}")]
        public async Task<IActionResult> Details([FromRoute] int resourceId)
        {
            if (await AreActivesAllowed(resourceId) && await resourcesService.Get(resourceId) is Resource resourceModel)
            {
                var resourceDTO = dtoMapper.Map<ResourceDetailedDto>(resourceModel);
                return Ok(resourceDTO);
            }
            else
                return NotFound("Requested resource not found."); 
        }

        // POST: api/Resources
        [HttpPost]
        //[Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Create([FromBody] ResourceDetailedDto item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var itemModel = dtoMapper.Map<Resource>(item);
            itemModel.ResourceId = 0;

            await resourcesService.Create(itemModel, await GetCurrentUserMOCK());

            return Ok("Resource created successfully.");
        }

        // PUT: api/Resources/5
        [HttpPut("{id}")]
        //[Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ResourceDetailedDto item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != item.ResourceId)
                return BadRequest("Resource identifiers inconsistent.");

            try
            {
                await resourcesService.Update(dtoMapper.Map<Resource>(item), await GetCurrentUserMOCK());
            }
            catch (UpdateFailedException)
            {
                return BadRequest("Cannot update this resource. Specified resource identifier could be improper.");
            }

            return Ok("Resource updated successfully.");
        }

        // DELETE: api/Resources/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                await resourcesService.Delete(id);
            }
            catch (DeleteResctrictedException)
            {
                return BadRequest("Cannot delete this resource. Some bookings rely on it.");
            }

            return Ok("Resource deleted.");
        }
        #endregion

        #region Extended actions
        // GET: api/Resources/Occupancy
        [HttpGet("Occupancy")]
        public async Task<IActionResult> IndexOccupancies()
        {
            return Ok(await resourcesService.GetOccupancies(includeIncatives: await IsCurrentUserAdmin()));
        }


        // GET: api/Resources/5/Occupancy
        [HttpGet("{id}/Occupancy")]
        public async Task<IActionResult> SingleOccupancy([FromRoute] int id)
        {
            try
            {
                if (!await AreActivesAllowed(id))
                    throw new KeyNotFoundException();

                return Ok(await resourcesService.GetOccupancy(id));
            }
            catch(KeyNotFoundException)
            {
                return NotFound("Requested resource not found.");
            }
            catch(FieldValueAbsurdException)
            {
                return BadRequest("There is a problem with the resource booking policy.");
            }
        }
        #endregion

        #region Private utilities
        /// <summary>
        /// User mock.
        /// </summary>
        async Task<ApplicationUser> GetCurrentUserMOCK() => await userManager.FindByNameAsync("SuperAdmin");//stub

        async Task<bool> IsCurrentUserAdmin() => await userManager.IsInRoleAsync(await GetCurrentUserMOCK(), RoleTypes.Admin);

        async Task<bool> AreActivesAllowed(int resourceId) => await resourcesService.IsResourceActive(resourceId) || await IsCurrentUserAdmin();


        #endregion
    }
}
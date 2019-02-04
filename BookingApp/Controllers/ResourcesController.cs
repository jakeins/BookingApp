using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookingApp.Models;
using BookingApp.Services;
using AutoMapper;
using BookingApp.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
        readonly ResourcesService service;
        readonly IMapper mapper;

        public ResourcesController(ResourcesService service)
        {
            this.service = service;

            mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Resource, ResourceBriefDTO>();
                cfg.CreateMap<Resource, ResourceDetailedDTO>().ReverseMap();
            }));
        }

        // GET: api/Resources
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = mapper.Map<IEnumerable<ResourceBriefDTO>>(await service.GetList());

            foreach (var item in result)
                item.Occupancy = await service.GetOccupancy(item.ResourceId);

            return Ok(result);
        }

        // GET: api/Resources/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details([FromRoute] int id)
        {
            if(mapper.Map<ResourceDetailedDTO>(await service.Get(id)) is ResourceDetailedDTO item)
                return Ok(item);
            else
                return NotFound(); 
        }
        
        // POST: api/Resources
        [HttpPost]
        //[Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Create([FromBody] ResourceDetailedDTO item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var itemModel = mapper.Map<Resource>(item);
            itemModel.ResourceId = 0;

            await service.Create(itemModel);

            return Ok("Resource created successfully.");
        }
        
        // PUT: api/Resources/5
        [HttpPut("{id}")]
        //[Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ResourceDetailedDTO item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != item.ResourceId)
                return BadRequest();

            try
            {
                await service.Update(mapper.Map<Resource>(item));
            }
            catch(DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
                
            return Ok("Resource updated successfully.");
        }

        // DELETE: api/Resources/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await service.Delete(id);
            return Ok("Resource deleted.");
        }
    }
}
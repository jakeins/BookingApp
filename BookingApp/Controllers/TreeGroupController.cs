using AutoMapper;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Exceptions;
using BookingApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BookingApp.Controllers
{
    public class TreeGroupController : Controller
    {
        TreeGroupService service;
        readonly IMapper mapper;

        public TreeGroupController(TreeGroupService s)
        {
            service = s;
            mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TreeGroup, TreeGroupListDto>();
                cfg.CreateMap<TreeGroup, TreeGroupCrUpDto>().ReverseMap();
            }));
        }

        [HttpGet]
        [Route("api/tree-group")]
        public async Task<IActionResult> Index()
        {
            IEnumerable<TreeGroupListDto> trees = mapper.Map<IEnumerable<TreeGroupListDto>>(await service.GetThree());
            return Ok(trees);
        }

        [HttpGet]
        [Route("api/tree-group-child")]
        public async Task<IActionResult> GetWithChild()
        {
            IEnumerable<TreeGroup> trees = await service.GetWithChild();
            return Ok(trees);
        }

        [HttpGet]
        [Route("api/tree-group/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                TreeGroupListDto tree = mapper.Map<TreeGroupListDto>(await service.GetDetail(id));
                return Ok(tree);
            }
            catch (NotIssetTreeGroupException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("api/tree-group")]
        public async Task<IActionResult> Create([FromBody]TreeGroupCrUpDto tree)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            await service.Create(mapper.Map<TreeGroup>(tree));
            return Ok("TreeGroup created successfully.");
        }

        [HttpPut]
        [Route("api/tree-group/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody]TreeGroupCrUpDto tree)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await service.Update(id, mapper.Map<TreeGroup>(tree));
                return Ok("TreeGroup updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("This TreeGroup does't isset.");
            }
        }

        [HttpDelete]
        [Route("api/tree-group/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await service.Delete(id);
                return Ok("TreeGroup deleted.");
            } catch(NotIssetTreeGroupException e)
            {
                return BadRequest(e.Message);
            } 
        }

    }
}

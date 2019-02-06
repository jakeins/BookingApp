using AutoMapper;
using BookingApp.Data.Models;
using BookingApp.DTOs.TreeGroup;
using BookingApp.Services;
using Microsoft.AspNetCore.Mvc;
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
                cfg.CreateMap<TreeGroup, CreateUpdateDto>().ReverseMap(); ;
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
        [Route("api/tree-group/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                TreeGroupListDto tree = mapper.Map<TreeGroupListDto>(await service.GetDetail(id));
                return Ok(tree);
            }
            catch (IndexOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("api/tree-group")]
        public async Task<IActionResult> Create([FromBody]CreateUpdateDto tree)
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
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody]CreateUpdateDto tree)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await service.Update(id, mapper.Map<TreeGroup>(tree));
            return Ok("TreeGroup updated successfully.");
        }

        [HttpDelete]
        [Route("api/tree-group/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await service.Delete(id);
                return Ok("TreeGroup deleted.");
            } catch(IndexOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
            
        }




    }
}

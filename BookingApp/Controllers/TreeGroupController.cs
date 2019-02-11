using AutoMapper;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Exceptions;
using BookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;


namespace BookingApp.Controllers
{
    /// <remarks>
    /// This class-controller can add, edit, delete and getting TreeGroup.
    /// </remarks>
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

        /// <summary>
        /// Creating TreeGroup
        /// <summary>
        /// <returns>Http response code 200 | 404 | 500</returns>
        [HttpGet]
        [Route("api/tree-group")]
        public async Task<IActionResult> Index()
        {
            bool isAdmin = User.HasClaim(ClaimTypes.Role, "Admin");
            IEnumerable<TreeGroupListDto> trees = mapper.Map<IEnumerable<TreeGroupListDto>>(await service.GetThree(isAdmin));
            return Ok(trees);
        }

        /// <summary>
        /// Getting TreeGroup on id
        /// <summary>
        /// <param name="id">Id TreeGroup.</param>
        /// <returns>Http response code 200 | 404 | 500</returns>
        [HttpGet]
        [Route("api/tree-group/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            TreeGroupListDto tree = mapper.Map<TreeGroupListDto>(await service.GetDetail(id));
            return Ok(tree);
        }

        /// <summary>
        /// Creating TreeGroup
        /// <summary>
        /// <param name="tree">Tdo model TreeGroupCrUpDto.</param>
        /// <returns>Http response code 200 | 201 | 401 | 404 | 500</returns>
        [HttpPost]
        [Route("api/tree-group")]
        //[Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Create([FromBody]TreeGroupCrUpDto tree)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await service.Create(mapper.Map<TreeGroup>(tree));
            return Ok("TreeGroup created successfully.");
        }

        /// <summary>
        /// Updating TreeGroup (DbUpdateConcurrencyException)
        /// <summary>
        /// <param name="tree">Tdo model TreeGroupCrUpDto.</param>
        /// <returns>Http response code 200 | 401 | 404 | 500</returns>
        [HttpPut]
        [Route("api/tree-group/{id}")]
        //[Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody]TreeGroupCrUpDto tree)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await service.Update(id, mapper.Map<TreeGroup>(tree));
            return Ok("TreeGroup updated successfully.");  
        }

        /// <summary>
        /// Deleting TreeGroup (DbUpdateException)
        /// <summary>
        /// <param name="tree">Tdo model TreeGroupCrUpDto.</param>
        /// <returns>Http response code 200 | 401 | 404 | 500</returns>
        [HttpDelete]
        [Route("api/tree-group/{id}")]
        //[Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await service.Delete(id);
            return Ok("TreeGroup deleted.");
        }

    }
}

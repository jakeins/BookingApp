using AutoMapper;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Exceptions;
using BookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
            IEnumerable<TreeGroupListDto> trees = mapper.Map<IEnumerable<TreeGroupListDto>>(await service.GetThree());
            return Ok(trees);
        }

        /// <summary>
        /// Getting TreeGroup with children
        /// <summary>
        /// <returns>Http response code 200 | 404 | 500</returns>
        [HttpGet]
        [Route("api/tree-group-child")]
        public async Task<IActionResult> GetWithChild()
        {
            IEnumerable<TreeGroup> trees = await service.GetWithChild();
            return Ok(trees);
        }

        /// <summary>
        /// Getting TreeGroup on id
        /// <summary>
        /// <param name="id">Id TreeGroup.</param>
        /// <exception cref="BookingApp.Exceptions.CurrentEntryNotFoundException">This TreeGroup does't isset.
        /// <returns>Http response code 200 | 404 | 500</returns>
        [HttpGet]
        [Route("api/tree-group/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                TreeGroupListDto tree = mapper.Map<TreeGroupListDto>(await service.GetDetail(id));
                return Ok(tree);
            }
            catch (CurrentEntryNotFoundException e)
            {
                return BadRequest(e.Message);
            }
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
        /// Updating TreeGroup
        /// <summary>
        /// <param name="tree">Tdo model TreeGroupCrUpDto.</param>
        /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">This TreeGroup does't isset.
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

        /// <summary>
        /// Deleting TreeGroup
        /// <summary>
        /// <param name="tree">Tdo model TreeGroupCrUpDto.</param>
        /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">This category has children.
        /// <exception cref="BookingApp.Exceptions.CurrentEntryNotFoundException">This TreeGroup does't isset.
        /// <returns>Http response code 200 | 401 | 404 | 500</returns>
        [HttpDelete]
        [Route("api/tree-group/{id}")]
        //[Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await service.Delete(id);
                return Ok("TreeGroup deleted.");
            } catch(DbUpdateException)
            {
                return BadRequest("This category has children.");
            } catch(CurrentEntryNotFoundException e)
            {
                return BadRequest(e.Message);
            } 
        }

    }
}

using AutoMapper;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Helpers;
using BookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Controllers
{

    [Route("api/rules")]
    [ApiController]
    public class RuleController : EntityControllerBase
    {
        readonly IRuleService _ruleService;
        readonly IMapper mapper;

        public RuleController(IRuleService ruleService)
        {
            _ruleService = ruleService;

            mapper = new Mapper(new MapperConfiguration(c =>
            {
                c.CreateMap<Rule, RuleBasicDTO>();
                c.CreateMap<Rule, RuleAdminDTO>();
                c.CreateMap<Rule, RuleDetailedDTO>().ReverseMap();
            }));
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Rules()
        {
            if (IsAdmin)
            {
                var rules = await _ruleService.GetList();
                var dtos = mapper.Map<IEnumerable<RuleAdminDTO>>(rules);
                return Ok(dtos);
            }
            else
            {
                var rules = await _ruleService.GetActiveList();
                var dtos = mapper.Map<IEnumerable<RuleBasicDTO>>(rules);
                return Ok(dtos);
            }
        }


        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [Route("{id}")]
        public async Task<IActionResult> GetRule(int id)
        {
            
            if (IsAdmin)
            {
                var rule = await _ruleService.Get(id);
                var dtor = mapper.Map<RuleAdminDTO>(rule);
                return Ok(dtor);
            }
            else
            {
                bool existsActive = await _ruleService.GetActive(id);
                if (!existsActive)
                    return BadRequest();
                var rule = await _ruleService.Get(id);
                var dtor = mapper.Map<RuleBasicDTO>(rule);
                return Ok(dtor);
            }
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> CreateRule([FromBody] RuleDetailedDTO dtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rule = mapper.Map<Rule>(dtos);
            rule.CreatedUserId = rule.UpdatedUserId = UserId;                         //fix time
            rule.CreatedTime = rule.UpdatedTime = DateTime.Now;
            await _ruleService.Create(rule);
            return Ok(rule);

        }

        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [Authorize(Roles = RoleTypes.Admin)]
        [Route("{id}")]
        public async Task<IActionResult> DeleteRule(int id)
        {
            await _ruleService.Delete(id);
            return Ok("Rule's been deleted");
        }

        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [Authorize(Roles = RoleTypes.Admin)]
        [Route("{id}")]
        public async Task<IActionResult> UpdateRule(int id, [FromBody]RuleDetailedDTO dtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rule = mapper.Map<Rule>(dtos);
            rule.UpdatedTime = DateTime.Now;              
            rule.UpdatedUserId = UserId;                 
            rule.Id = id;

            await _ruleService.Update(rule);
            return Ok("Rule's been updated");

        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [Authorize(Roles = RoleTypes.Admin)]
        [Route("{id}/resources")]
        public async Task<IActionResult> GetResourcesByRule(int id, [FromServices]IResourcesService _resourcesService)
        {
            var res = await _resourcesService.ListByRuleKey(id);
            var resD = mapper.Map<IEnumerable<Resource>, IEnumerable<ResourceMaxDto>>(res);
            return Ok(resD);
        }
    }
}

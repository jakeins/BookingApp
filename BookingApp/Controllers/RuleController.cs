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
        RuleService _ruleService;
        IMapper mapper;

        public RuleController(RuleService ruleService)
        {
            _ruleService = ruleService;

            mapper = new Mapper(new MapperConfiguration(c =>
            {
                c.CreateMap<Rule, RuleBasicDTO>().ReverseMap();
                c.CreateMap<Rule, RuleDetailedDTO>();
            }));
        }

        [HttpGet]
        public async Task<IActionResult> Rules()
        {
            var rules = await _ruleService.GetList();

            if (IsAdmin)
            {
                var dtos = mapper.Map<IEnumerable<RuleDetailedDTO>>(rules);
                return Ok(dtos);
            }
            else
            {
                var dtos = mapper.Map<IEnumerable<RuleBasicDTO>>(rules);
                return Ok(dtos);
            }
        }

        [HttpGet]
        [Route("{id}")]
        //[Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> GetRule(int id)
        {
            var rule = await _ruleService.Get(id);
            if (IsAdmin)
            {
                var dtor = mapper.Map<RuleDetailedDTO>(rule);
                return Ok(dtor);
            }
            if (IsUser)
            {
                var dtor = mapper.Map<RuleBasicDTO>(rule);
                return Ok(dtor);
            }
            else return BadRequest();
        }

        [HttpPost]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> CreateRule([FromBody] RuleBasicDTO dtos)
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
        [Authorize(Roles = RoleTypes.Admin)]
        [Route("{id}")]
        public async Task<IActionResult> DeleteRule(int id)
        {
            await _ruleService.Delete(id);
            return Ok("Rule's been deleted");
        }

        [HttpPut]
        [Authorize(Roles = RoleTypes.Admin)]
        [Route("{id}")]
        public async Task<IActionResult> UpdateRule(int id, [FromBody]RuleBasicDTO dtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rule = mapper.Map<Rule>(dtos);
            rule.UpdatedTime = rule.CreatedTime = DateTime.Now;              //need createdtime and createduser to update
            rule.UpdatedUserId = rule.CreatedUserId = UserId;
            rule.Id = id;

            await _ruleService.Update(rule);
            return Ok("Rule's been updated");

        }
    }
}

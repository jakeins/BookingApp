using AutoMapper;
using BookingApp.Controllers.Bases;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.DTOs.Resource;
using BookingApp.Helpers;
using BookingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Controllers
{

    [Route("api/rules")]
    [ApiController]
    public class RuleController : EntityControllerBase
    {
        readonly IRuleService _ruleService;
        readonly IMapper _mapper;             

        public RuleController(IRuleService ruleService)
        {
            _ruleService = ruleService;
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Rule, RuleBasicDTO>();
                cfg.CreateMap<Rule, RuleAdminDTO>();
                cfg.CreateMap<Rule, RuleDetailedDTO>().ReverseMap();
            }));
        }

        /// <summary>
        /// Returns list of rules. GET: api/rules
        /// </summary>
        /// <returns>Http response code</returns>
        /// <response code ="200">Successfull operation</response>
        /// <response code ="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Rules()
        {
            if (IsAdmin)
            {
                var rules = await _ruleService.GetList();
                var dtos = _mapper.Map<IEnumerable<RuleAdminDTO>>(rules);
                return Ok(dtos);
            }
            else
            {
                var rules = await _ruleService.GetActiveList();
                var dtos = _mapper.Map<IEnumerable<RuleBasicDTO>>(rules);
                return Ok(dtos);
            }
        }


        /// <summary>
        /// Return rule. GET: api/rules/{id}
        /// </summary>
        /// <param name="id">Rule id</param>
        /// <returns>Http response code</returns>
        /// <response code ="200">Successfull operation</response>
        /// <response code ="500">Internal server error</response>
        /// <response code ="404">Rule not found</response>
        /// <response code ="400">Incorrect Id</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [Route("{id}")]
        public async Task<IActionResult> GetRule([FromRoute]int id)
        {
            
            if (IsAdmin)
            {
                var rule = await _ruleService.Get(id);
                var dtor = _mapper.Map<RuleAdminDTO>(rule);
                return Ok(dtor);
            }
            else
            {

                var rule = await _ruleService.Get(id);
                var dtor = _mapper.Map<RuleBasicDTO>(rule);
                return Ok(dtor);
            }
        }

        /// <summary>
        /// Return rule. Post: api/rules/{id}
        /// </summary>
        /// <param name="id">Rule id</param>
        /// <param name="dtos">RuleDetailedDTO</param>
        /// <returns>Http response code</returns>
        /// <response code ="200">Successfull operation</response>
        /// <response code ="500">Internal server error</response>
        /// <response code ="401">Unauthorized.Only admin can create rule.</response>
        /// <response code = "400">Invalid dtos</response>
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

            var rule = _mapper.Map<Rule>(dtos);
            rule.CreatedUserId = rule.UpdatedUserId = UserId;                        
            await _ruleService.Create(rule);
            return Ok(rule);

        }

        /// <summary>
        /// Return rule. Delete: api/rules/{id}
        /// </summary>
        /// <param name="id">Rule id</param>
        /// <returns>Http response code</returns>
        /// <response code ="200">Successfull operation</response>
        /// <response code ="500">Internal server error</response>
        /// <response code ="404">Rule not found</response>
        /// <response code ="401">Unauthorized.Only admin can delete rule.</response>
        /// <response code ="403">Error, can't delete rule, relative to resources</response>
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(Roles = RoleTypes.Admin)]
        [Route("{id}")]
        public async Task<IActionResult> DeleteRule([FromRoute]int id)
        {
            await _ruleService.Delete(id);
            return Ok();
        }

        /// <summary>
        /// Return rule. Put: api/rules/{id}
        /// </summary>
        /// <param name="id">Rule id</param>
        /// <param name="dtos">RuleDetailedDTO</param>
        /// <returns>Http response code</returns>
        /// <response code ="200">Successfull operation</response>
        /// <response code ="500">Internal server error</response>
        /// <response code ="401">Unauthorized.Only admin can create rule.</response>
        /// <response code ="400">Incorrect id</response>
        /// <response code ="404">Rule not found</response>
        /// <response code ="403">Error, can't update rule, relative to resources</response>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(Roles = RoleTypes.Admin)]
        [Route("{id}")]
        public async Task<IActionResult> UpdateRule([FromRoute]int id, [FromBody]RuleDetailedDTO dtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rule = _mapper.Map<Rule>(dtos);        
            rule.UpdatedUserId = UserId;                 

            await _ruleService.Update(id, rule);
            return Ok();

        }

        /// <summary>
        /// Return resources. Get: api/rules/{id}/resources
        /// </summary>
        /// <param name="id">Rule id</param>
        /// <returns>Http response code</returns>
        /// <response code ="200">Successfull operation</response>
        /// <response code ="500">Internal server error</response>
        /// <response code ="401">Unauthorized.Only admin can receive list of resources for rule.</response>
        /// <response code ="400">Incorrect id</response>
        /// <response code ="404">Rule not found</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [Authorize(Roles = RoleTypes.Admin)]
        [Route("{id}/resources")]
        public async Task<IActionResult> GetResourcesByRule([FromRoute]int id, [FromServices]IResourcesService _resourcesService)
        {
            var res = await _resourcesService.ListByRuleKey(id);
            var resD = _mapper.Map<IEnumerable<Resource>, IEnumerable<ResourceMaxDto>>(res);
            return Ok(resD);
        }
    }
}

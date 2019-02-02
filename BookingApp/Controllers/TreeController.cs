using BookingApp.Models;
using BookingApp.Models.Dto;
using BookingApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Controllers
{
    public class TreeController : Controller
    {
        TreeService service;

        public TreeController(TreeService s)
        {
            service = s;
        }

        [HttpGet]
        [Route("api/tree/index")]
        public IActionResult Index()
        {
            List<TreeGroup> trees = service.GetThree();
            return new OkObjectResult(trees);
        }

        [HttpGet]
        [Route("api/tree/detail/{id}")]
        public IActionResult Detail(int id)
        {
            TreeGroup tree = service.GetDetail(id);
            return new OkObjectResult(tree);
        }

        [HttpPost]
        [Route("api/tree/create")]
        public IActionResult Create([FromBody]CreateTree tree)
        {
            //if (ModelState.IsValid)
            //{
                //return BadRequest();
            //}
            service.Create(tree);
            return new OkObjectResult("Create ok");
        }

        [HttpPut]
        [Route("api/tree/update")]
        public IActionResult Update([FromBody]UpdateTree tree)
        {
            //if (ModelState.IsValid)
            //{
                //return BadRequest();
            //}
            service.Update(tree);
            return new OkObjectResult("Update ok");
        }

        [HttpDelete]
        [Route("api/tree/delete/{id}")]
        public IActionResult Delete(int id)
        {
            if(service.Delete(id))
            {
                return new OkObjectResult("Delete ok");
            }
            return NotFound();
        }




    }
}

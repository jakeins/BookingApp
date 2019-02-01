using BookingApp.Models;
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
        public async Task<IActionResult> Index()
        {
            List<TreeGroup> trees = service.GetThree();
            return new OkObjectResult(trees);
        }

        [HttpGet]
        [Route("api/tree/detail/{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            TreeGroup tree = service.GetDetail(id);
            return new OkObjectResult(tree);
        }

        [HttpDelete]
        [Route("api/tree/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            service.Delete(id);
            return new OkObjectResult("Delete ok");
        }




    }
}

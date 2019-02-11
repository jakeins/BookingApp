using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookingApp.Controllers
{
    public partial class ResourcesController 
    {
        // Filtered access: Guest/Owner/Admin.
        [HttpGet("{resourceId}/bookings")]
        public async Task<IActionResult> ListRelatedBookings([FromRoute] int resourceId)
        {
            await AuthorizeForSingleResource(resourceId);

            #region Action body

            return Ok("Not implemented");

            #endregion
        }
    }
}
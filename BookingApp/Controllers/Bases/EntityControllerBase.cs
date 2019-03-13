using Microsoft.AspNetCore.Mvc;
using BookingApp.Helpers;
using System.Linq;
using System;

namespace BookingApp.Controllers.Bases
{
    /// <summary>
    /// Base controller class having cross-entity functional.
    /// </summary>
    public abstract class EntityControllerBase : ControllerBase
    {
        /// <summary>
        /// Current user identifier
        /// </summary>
        public virtual string UserId => User.Claims.Single(c => c.Type == "uid").Value;

        /// <summary>
        /// Shorthand for checking if current user has admin access level
        /// </summary>
        public virtual bool IsAdmin => User.IsInRole(RoleTypes.Admin);

        /// <summary>
        /// Shorthand for checking if current user has user access level
        /// </summary>
        public virtual bool IsUser => User.IsInRole(RoleTypes.User);

        /// <summary>
        /// Shorthand for checking if current user doesn't have any specific rights.
        /// Current user is anonymous if he hasn't UserID claim.
        /// </summary>
        public virtual bool IsAnonymous => !User.HasClaim(c => c.Type == "uid");

        public static explicit operator ControllerContext(EntityControllerBase v)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets full base api url of a current controller, if annotated correctly.
        /// </summary>
        public virtual string BaseApiUrl
        {
            get
            {
                var request = ControllerContext.HttpContext.Request;
                return request.Scheme + "://" + request.Host + request.Path;
            }
        }
    }
}
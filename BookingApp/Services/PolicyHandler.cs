using BookingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class NotBannedRequirement : IAuthorizationRequirement
    {
        public bool IsApproved { get; set; }
        public bool IsBlocked { get; set; }

        public NotBannedRequirement()
        { }
    }

    public class NotBannedHandler : AuthorizationHandler<NotBannedRequirement>
    {
        private readonly IUserService userService;

        public NotBannedHandler(IUserService userService)
        {
            this.userService = userService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, NotBannedRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == "uid"))
            {
                return;
            }

            var userId = context.User.FindFirst(c => c.Type == "uid").Value;
            var user = await userService.GetUserById(userId);

            if (user.IsBlocked ?? false || !(user.ApprovalStatus ?? false))
            {
                context.Succeed(requirement);
            }

            return;
        }
    }
}

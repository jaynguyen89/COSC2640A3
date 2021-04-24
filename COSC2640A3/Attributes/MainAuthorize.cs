using System;
using System.Linq;
using COSC2640A3.Services.Interfaces;
using COSC2640A3.ViewModels;
using Helper;
using Helper.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace COSC2640A3.Attributes {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class MainAuthorize : AuthorizeAttribute, IAuthorizationFilter {

        public void OnAuthorization(AuthorizationFilterContext context) {
            var redisCache = context.HttpContext.RequestServices.GetService<IRedisCacheService>();
            if (redisCache is null) {
                context.Result = new NotFoundResult();
                return;
            }

            var (_, accountId) = context.HttpContext.Request.Headers.FirstOrDefault(header => header.Key.Equals(nameof(AuthenticatedUser.AccountId)));
            if (!Helpers.IsProperString(accountId)) {
                context.Result = new ForbidResult();
                return;
            }

            var authenticatedUser = redisCache.GetRedisCacheEntry<AuthenticatedUser>($"{ nameof(AuthenticatedUser) }_{ accountId }").Result;
            if (authenticatedUser is null) {
                context.Result = new ForbidResult();
                return;
            }

            var (_, clientAuthHeader) = context.HttpContext.Request.Headers.FirstOrDefault(header => header.Key.Equals("Authorization"));
            if (!Helpers.IsProperString(clientAuthHeader)) context.Result = new UnauthorizedResult();

            var clientAuthToken = clientAuthHeader.ToString().Split(SharedConstants.MonoSpace).Last();
            if (!authenticatedUser.AuthToken.Equals(clientAuthToken)) context.Result = new UnauthorizedResult();
        }
    }
}
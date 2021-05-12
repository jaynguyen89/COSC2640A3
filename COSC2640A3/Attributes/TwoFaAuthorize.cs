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

    public sealed class TwoFaAuthorize : AuthorizeAttribute, IAuthorizationFilter {

        public void OnAuthorization(AuthorizationFilterContext context) {
            var redisCache = context.HttpContext.RequestServices.GetService<IRedisCacheService>();
            if (redisCache is null) {
                context.Result = new NotFoundResult();
                return;
            }
            
            var (_, accountId) = context.HttpContext.Request.Headers.FirstOrDefault(header => header.Key.ToLower().Equals(nameof(AuthenticatedUser.AccountId).ToLower()));
            if (!Helpers.IsProperString(accountId)) {
                context.Result = new ForbidResult();
                return;
            }

            var isTwoFaVerified = redisCache.GetRedisCacheEntry<bool>($"{ SharedConstants.TwoFaCacheName }_{ accountId }").Result;
            if (!isTwoFaVerified) context.Result = new UnauthorizedResult();
        }
    }
}
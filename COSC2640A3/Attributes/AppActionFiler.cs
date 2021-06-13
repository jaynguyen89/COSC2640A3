using Microsoft.AspNetCore.Mvc.Filters;

namespace COSC2640A3.Attributes {

    public sealed class AppActionFiler : ActionFilterAttribute {

        public override void OnActionExecuted(ActionExecutedContext context) {
            base.OnActionExecuted(context);
            context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}

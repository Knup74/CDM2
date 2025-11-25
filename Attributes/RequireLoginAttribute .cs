using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class RequireLoginAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Si l’action est marquée [AllowAnonymous] => on laisse passer
        var allowAnon = context.ActionDescriptor.EndpointMetadata
            .Any(m => m is AllowAnonymousAttribute);

        if (allowAnon)
            return;

        // Sinon, authentification obligatoire
        var userId = context.HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
        }
    }

}

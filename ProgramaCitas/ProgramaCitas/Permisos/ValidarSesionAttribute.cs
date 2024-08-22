using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProgramaCitas.Permisos
{
    public class ValidarSesionAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            if (session.GetString("Usuario") == null)
            {
                context.Result = new RedirectToActionResult("Login", "Acceso", null);
            }
            base.OnActionExecuting(context);
        }
    }
}

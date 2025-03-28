using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;

namespace MvcOAuthEmpleados.Filters
{
    public class AuthorizeEmpleadosAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            //string controller = context.routedata.values["controller"].tostring();
            //string action = context.routedata.values["action"].tostring();
            //var routevalues = context.routedata.values;

            //// verificar si hay algún parámetro en la url (por ejemplo, un 'id') que debamos guardar
            //var id = routevalues.containskey("id") ? routevalues["id"] : null;

            //// obtener el servicio de tempdata para almacenar la información de la ruta
            //itempdataprovider provider = context.httpcontext.requestservices.getservice<itempdataprovider>();
            //var tempdata = provider.loadtempdata(context.httpcontext);

            //// guardar la ruta, acción y cualquier parámetro (como el 'id') en tempdata
            //tempdata["controller"] = controller;
            //tempdata["action"] = action;
            //tempdata["id"] = id;  // almacenar el parámetro id si está presente

            //// guardar los cambios en tempdata
            //provider.savetempdata(context.httpcontext, tempdata);

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = this.GetRoute("Managed", "Login");
            }
        }

        private RedirectToRouteResult GetRoute(string controller, string action)
        {
            RouteValueDictionary ruta = new RouteValueDictionary(new { controller = controller, action = action });
            RedirectToRouteResult result = new RedirectToRouteResult(ruta);
            return result;
        }
    }
}

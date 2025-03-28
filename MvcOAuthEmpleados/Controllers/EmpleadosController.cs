using Microsoft.AspNetCore.Mvc;
using MvcOAuthEmpleados.Filters;
using MvcOAuthEmpleados.Models;
using MvcOAuthEmpleados.Services;
using NuGet.Common;

namespace MvcOAuthEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private ServiceEmpleados service;

        public EmpleadosController(ServiceEmpleados service)
        {
            this.service = service;
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Index()
        {
            List<Empleado> empleados = await this.service.GetEmpleadosAsync();
            return View(empleados);
        }

        public async Task<IActionResult> Details(int id)
        {
            HttpContext.User.FindFirst(x => x.Type == "Token");
            Empleado empleado = await this.service.FindEmpleadoAsync(id);

            return View(empleado);  
            //string token = HttpContext.Session.GetString("Token");
            //if(token == null)
            //{
            //    ViewBag.Mensaje = "No se ha iniciado sesión";
            //    return View();
            //}
            //else
            //{
            //    Empleado empleado = await this.service.FindEmpleadoAsync(id, token);
            //    return View(empleado);
            //}
        }
    }
}

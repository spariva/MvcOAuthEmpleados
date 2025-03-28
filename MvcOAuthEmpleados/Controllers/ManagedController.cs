using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcOAuthEmpleados.Models;
using MvcOAuthEmpleados.Services;

namespace MvcOAuthEmpleados.Controllers
{
    public class ManagedController : Controller
    {
        private ServiceEmpleados service;

        public ManagedController(ServiceEmpleados service)
        {
            this.service = service;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            string token = await this.service.GetTokenAsync(model.UserName, model.Password);
            if (token == null)
            {
                ViewBag.Mensaje = "User incorrecto";
                return View();
            }
            else
            {
                ViewBag.Mensaje = "User correcto";

                HttpContext.Session.SetString("Token", token);

                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.Name, model.UserName));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, model.Password));
                identity.AddClaim(new Claim("Token", token));
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                });
                return RedirectToAction("Index", "Empleados");

            }
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Empleados");
        }
    }
}

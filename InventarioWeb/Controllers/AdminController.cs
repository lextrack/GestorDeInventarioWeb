using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventarioWeb.Controllers
{
    // Se asegura de que solo los usuarios con el rol de "Admin" puedan acceder a este controlador
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Constructor que inyecta los servicios UserManager y RoleManager
        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Método que devuelve la vista para asignar roles
        public IActionResult AssignRole()
        {
            return View("~/Views/Shared/AssignRole.cshtml");
        }

        // Método HTTP POST para asignar un rol a un usuario
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            // Verifica si los parámetros son válidos
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            {
                ModelState.AddModelError("", "El ID de usuario y el nombre del rol son obligatorios.");
                return View();
            }

            // Busca el usuario por su ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("", "Usuario no encontrado.");
                return View();
            }

            // Verifica si el rol existe
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                ModelState.AddModelError("", "El rol no existe.");
                return View();
            }

            // Asigna el rol al usuario
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            // Si hay errores, los agrega al ModelState para mostrarlos en la vista
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }
    }
}
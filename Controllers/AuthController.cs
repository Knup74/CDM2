using Microsoft.AspNetCore.Mvc;
using CDM.Database;
using CDM.Service;
using CDM.Database.Models;

public class AuthController : Controller
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth)
    {
        _auth = auth;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var user = _auth.Login(email, password);
        if (user == null)
        {
            ViewBag.Error = "Email ou mot de passe incorrect.";
            return View();
        }

        // stocker l'utilisateur en session
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserRole", user.Role);
        HttpContext.Session.SetString("UserName", user.Nom);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}

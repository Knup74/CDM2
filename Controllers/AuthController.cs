using Microsoft.AspNetCore.Mvc;
using CDM.Database;
using CDM.Service;
using CDM.Database.Models;
using CDM.Models;
using System.Security.Cryptography;


[AllowAnonymous]
public class AuthController : Controller
{
    private readonly AuthService _auth;
    private readonly AppDbContext _context;

    public AuthController(AuthService auth, AppDbContext context)
    {
        _auth = auth;
        _context = context;
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

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ForgotPassword(string email)
    {
        var user = _context.Coproprietaires.FirstOrDefault(x => x.Email == email);

        if (user == null)
        {
            ViewBag.Message = "Si un compte existe avec cet email, un lien a été envoyé.";
            return View();
        }

        // Génération du token
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        user.ResetPasswordToken = token;
        user.ResetPasswordTokenExpiration = DateTime.UtcNow.AddHours(1);

        _context.SaveChanges();

        // URL du lien
        var resetUrl = Url.Action("ResetPassword", "Auth", new { token }, Request.Scheme);

        // Envoi de l’email
        EmailService.Send(
            to: user.Email,
            subject: "Réinitialisation du mot de passe",
            body: $"Cliquez ici pour réinitialiser votre mot de passe : {resetUrl}"
        );

        ViewBag.Message = "Si un compte existe avec cet email, un lien a été envoyé.";
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        var user = _context.Coproprietaires.FirstOrDefault(x =>
            x.ResetPasswordToken == token &&
            x.ResetPasswordTokenExpiration > DateTime.UtcNow);

        if (user == null)
            return Content("Token invalide ou expiré.");

        return View(new ResetPasswordViewModel { Token = token });
    }

    [HttpPost]
    public IActionResult ResetPassword(ResetPasswordViewModel model)
    {
        var user = _context.Coproprietaires.FirstOrDefault(x =>
            x.ResetPasswordToken == model.Token &&
            x.ResetPasswordTokenExpiration > DateTime.UtcNow);

        if (user == null)
            return Content("Token invalide ou expiré.");

        _auth.CreatePassword(model.NewPassword, out var hash, out var salt);

        user.PasswordHash = hash;
        user.PasswordSalt = salt;
        user.ResetPasswordToken = null;
        user.ResetPasswordTokenExpiration = null;

        _context.SaveChanges();

        return RedirectToAction("Login");
    }



}

using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using club.soundyard.web.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Web;
using System.Net;
using System.Net.Mail;

public class AccountController : Controller
{
    private readonly ApplicationUserManager _userManager;
    private readonly ApplicationSignInManager _signInManager;
    private readonly ApplicationRoleManager _roleManager;

    // Konstruktor bez parametrů (pro případ, že DI není správně nastaveno)
    public AccountController()
    {
    }

    // Konstruktor se závislostmi injektovanými přes Dependency Injection
    public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    // Getter pro _userManager
    public ApplicationUserManager UserManager
    {
        get
        {
            return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }
    }

    // Getter pro _signInManager
    public ApplicationSignInManager SignInManager
    {
        get
        {
            return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        }
    }

    // Getter pro _roleManager
    public ApplicationRoleManager RoleManager
    {
        get
        {
            return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
        }
    }

    // Zobrazení registračního formuláře
    [AllowAnonymous]
    public ActionResult Register()
    {
        return View();
    }

    // Registrace uživatele
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Kontrola a případné vytvoření role, pokud neexistuje
                if (!await RoleManager.RoleExistsAsync("User"))
                {
                    var roleResult = await RoleManager.CreateAsync(new ApplicationRole { Name = "User" });
                    if (!roleResult.Succeeded)
                    {
                        AddErrors(roleResult);
                        return View(model); // Vrátit formulář v případě chyby při vytváření role
                    }
                }

                // Přiřadit roli uživateli
                await UserManager.AddToRoleAsync(user.Id, "User");

                // Odeslání aktivačního emailu
                var token = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, protocol: Request.Url.Scheme);
                await SendEmailAsync(model.Email, "Ověření emailu", $"Prosím potvrďte svůj email kliknutím na tento odkaz: <a href=\"{confirmationLink}\">Potvrdit email</a>");

                // Uživatel není přihlášen, dokud nepotvrdí email
                return RedirectToAction("Index", "Home");
            }

            // Pokud došlo k chybě při vytvoření uživatele, zobrazit chyby
            AddErrors(result);
        }

        // Pokud je model neplatný, vrátit zpět registrační formulář
        return View(model);
    }

    // Potvrzení emailu
    [AllowAnonymous]
    public async Task<ActionResult> ConfirmEmail(string userId, string token)
    {
        if (userId == null || token == null)
        {
            return View("Error");
        }

        var user = await UserManager.FindByIdAsync(userId);
        if (user == null)
        {
            return View("Error");
        }

        var result = await UserManager.ConfirmEmailAsync(userId, token);
        if (result.Succeeded)
        {
            return RedirectToAction("Login", "Account");
        }

        AddErrors(result);
        return View("Error");
    }

    // Přihlašování uživatele
    [AllowAnonymous]
    public ActionResult Login(string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
    {
        if (ModelState.IsValid)
        {
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("LoginWith2fa", new { ReturnUrl = returnUrl });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Neplatné přihlašovací údaje.");
                    return View(model);
            }
        }

        // Pokud je model neplatný, vrátit zpět přihlašovací formulář
        return View(model);
    }

    // Odesílání emailu
    private async Task SendEmailAsync(string email, string subject, string message)
    {
        using (var smtpClient = new SmtpClient())
        {
            smtpClient.Host = "smtp.centrum.cz";
            smtpClient.Port = 587; // nebo jiný port
            smtpClient.Credentials = new NetworkCredential("soundyard@centrum.cz", "Hudebnik2024+");
            smtpClient.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress("soundyard@centrum.cz"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }

    // Zpracování a přidání chyb do ModelState
    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error);
        }
    }

    private ActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }
}

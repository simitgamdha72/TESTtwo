using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookManagement.Models;
using BookManagement.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace TestProject.Controllers;

public class HomeController : Controller
{
    private readonly IConfiguration _configuration;

    private readonly BookManagementContext _context;

    public HomeController(IConfiguration configuration, BookManagementContext Context)
    {
        _configuration = configuration;
        _context = Context;
    }

    public IActionResult Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            if (User.Claims.Any(c => c.Value == "admin"))
            {
                return RedirectToAction("DashBoard", "Home");
            }
            else
            {
                return RedirectToAction("UserProfile", "Home");
            }

        }

        string? cookie = Convert.ToString(User.FindFirstValue(ClaimTypes.Email));

        if (cookie == null)
        {
            return View();
        }

        User? user = _context.Users.FirstOrDefault(u => u.Email == cookie);

        if (user == null)
        {
            return View();
        }
        else
        {
            if (user.Role == "admin")
            {
                return RedirectToAction("DashBoard", "Home");
            }
            else
            {
                return RedirectToAction("UserProfile", "Home");
            }

        }

    }

    [HttpPost]
    public async Task<IActionResult> Index(UserViewModel userViewModel)
    {
        if (ModelState.ContainsKey("UserName")) ModelState.Remove("UserName");

        if (!ModelState.IsValid)
        {
            return View(userViewModel);
        }

        User? user = _context.Users.FirstOrDefault(u => u.Email == userViewModel.Email);
        if (user == null || user.Password != userViewModel.Password)
        {
            user = null;
        }

        if (user == null)
        {
            TempData["error"] = "User not found or invalid password.";
            return View(userViewModel);
        }

        TempData["success"] = "Successfully logged in!";

        var tokenString = GenerateToken(user.Email, user.Role);
        var cookieOptions = new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.Strict
        };

        Response.Cookies.Append("AuthToken", tokenString, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(
               CookieAuthenticationDefaults.AuthenticationScheme,
               principal,
               new AuthenticationProperties
               {
                   IsPersistent = true,
                   ExpiresUtc = DateTime.UtcNow.AddDays(30)

               });

        if (user.Role == "admin")
        {
            return RedirectToAction("DashBoard", "Home");
        }
        else
        {
            return RedirectToAction("UserProfile", "Home");
        }

    }

    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SignUp(UserViewModel userViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(userViewModel);
        }

        User? user = _context.Users.FirstOrDefault(u => u.Email == userViewModel.Email || u.UserName == userViewModel.UserName);

        if (user != null)
        {
            TempData["AllredayExist"] = "This Email or Username is Allready in use!";
            return View();
        }

        User newUser = new User
        {
            Email = userViewModel.Email,
            UserName = userViewModel.UserName,
            Password = userViewModel.Password,
            Role = "user"
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [Authorize(Roles = "admin")]
    public IActionResult DashBoard()
    {
        return View();
    }

    [Authorize]
    [Authorize(Roles = "user")]
    public IActionResult UserProfile()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        try
        {
            Response.Cookies.Delete("AuthToken");

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            return RedirectToAction("Error", "Home");
        }
    }


    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private string GenerateToken(string email, string? role)
    {
        string Role = role.ToString();
        var claims = new List<Claim>
        {
            new (ClaimTypes.Email, email),
            new (ClaimTypes.Role, Role),
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("hey1234567890ojykjrkr6uluyk"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtConfig:Issuer"],
            audience: _configuration["JwtConfig:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

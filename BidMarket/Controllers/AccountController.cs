using AutoMapper;
using BidMarket.Models;
using BidMarket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace BidMarket.Controllers
{
    [Controller]
    public class AccountController : Controller
    {
        private readonly MailSender _sender;
        private readonly IMapper _mapper;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly Hasher _hasher;
        private readonly ILogger<AccountController> _logger;

        public AccountController(MailSender sender, SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager, IMapper mapper, ILogger<AccountController> logger, Hasher hasher)
        {
            _sender = sender;
            _mapper = mapper;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _hasher = hasher;
        }
        [HttpGet]
        public async Task<ActionResult> LogIn() { await Task.CompletedTask; return View(); }
        [HttpPost]
        public async Task<ActionResult> LogIn(RegisterUser user)
        {
            if (!ModelState.IsValid)
            {
                if(ModelState.ErrorCount > 2)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("not 2", "Заполните все поля");
                    return View(user);
                }
                
            }
                var us = _mapper.Map<AppUser>(user);
            var appUser = await _userManager.FindByEmailAsync(us.Email);

            if (appUser == null)
            {
                ModelState.Clear();
                ModelState.AddModelError("not exist", "Такого пользователя не существует");
                return View(user);
            }

            if(appUser.PasswordHash != _hasher.Compute(us.PasswordHash))
            {
                ModelState.Clear();
                ModelState.AddModelError("login", "Неверный пароль");
                return View(user);
            }
            appUser.PasswordHash = _hasher.Compute(us.PasswordHash);

            if (appUser.IsBlocked.HasValue && appUser.IsBlocked.Value)
            {
                ModelState.Clear();
                ModelState.AddModelError("block", "Ваш аккаунт заблокирован");
                return View(user);
            }

            
            await _signInManager.SignInAsync(appUser, true);


            return RedirectToAction("GetLots", "Lot");
        }
        [HttpGet]
        public async Task<ActionResult> SignUp() { await Task.CompletedTask; return View(); }
        [HttpPost]

        public async Task<ActionResult> SignUp(RegisterUser user)
        {
            if (ModelState.IsValid)
            {
                var appUser = _mapper.Map<AppUser>(user);
                if (await _userManager.FindByEmailAsync(appUser?.Email) != null)
                {
                    ModelState.AddModelError("p1", "Пользователь с таким email уже существует");
                    return View(user);
                }

                appUser.PasswordHash = _hasher.Compute(appUser.PasswordHash);
                appUser.UserName = appUser.Email;
                appUser.VirtualMoney = 0;
                var result = await _userManager.CreateAsync(appUser);

                if (!result.Succeeded) return View(user);

                await _sender.SendCodeAsync(appUser.Email);

                await _userManager.AddToRoleAsync(appUser, "User");

                await _signInManager.SignInAsync(appUser, true);

                return RedirectToAction("EmailConfirm");
            }
            else
            {
                return View(user);
            }
           
        }
        [HttpGet]

        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> SignUpManager() { await Task.CompletedTask; return View(); }
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> SignUpManager(AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(appUser?.Email) != null)
                {
                    ModelState.AddModelError("p1", "Пользователь с таким email уже существует");
                    return View(appUser);
                }

                appUser.PasswordHash = _hasher.Compute(appUser.PasswordHash);
                appUser.UserName = appUser.Email;
                appUser.VirtualMoney = 0;
                var result = await _userManager.CreateAsync(appUser);

                if (!result.Succeeded) return BadRequest();

                await _sender.SendCodeAsync(appUser.Email);

                await _userManager.AddToRoleAsync(appUser, "Manager");

                await _signInManager.SignInAsync(appUser, true);
            }
            else
            {
                return View(appUser);
            }

            return RedirectToAction("EmailConfirm");
        }
        [HttpGet]
        public async Task<ActionResult> EmailConfirm() { await Task.CompletedTask; return View(); }
        [HttpPost]
        public async Task<ActionResult> EmailConfirm(string code)
        {
            var user = await _userManager.GetUserAsync(User);
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("GetSelfUser", "User");
        }
        [HttpGet]

        [Authorize]
        public async Task<ActionResult> ResetPassword() { await Task.CompletedTask; return View(); }
        [HttpPost]

        [Authorize]
        public async Task<ActionResult> ResetPassword(string oldPassword, string newPassword)
        {
            var user = await _userManager.GetUserAsync(User);
            var oldHashed = _hasher.Compute(oldPassword);
            if (user?.PasswordHash != oldHashed) return RedirectToAction("GetSelfUser", "User");

            user.PasswordHash = _hasher.Compute(newPassword);
            await _userManager.UpdateAsync(user);
            return RedirectToAction("GetLots","Lot");
        }
        [HttpGet]
        public async Task<ActionResult> LogOut() { await _signInManager.SignOutAsync(); return RedirectToAction("Login", "Account"); }
        [HttpGet]
        public async Task<ActionResult> ChangePhone() { await Task.CompletedTask; return View(); }
        
        [HttpPost]
        public async Task<ActionResult> ChangePhone(string phone)
        {
            if(phone.Length < 7)
            {
                return RedirectToAction("GetSelfUser", "User");
            }
            var user = await _userManager.GetUserAsync(User);
            user.PhoneNumber = phone;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("GetSelfUser", "User");
        }
        
    }
}
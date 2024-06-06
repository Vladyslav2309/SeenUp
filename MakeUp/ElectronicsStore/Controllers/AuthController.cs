using AutoMapper;
using Core.Constants;
using Core.Interfaces;
using Core.Models.Account;
using Core.Models.Auth;
using Infastructure.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ElectronicsStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMapper _mapper;
        private readonly ISmtpEmailService _smtpEmailService;

        public AuthController(UserManager<UserEntity> userManager, IJwtTokenService jwtTokenService, IMapper mapper, ISmtpEmailService smtpEmailService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
            _smtpEmailService = smtpEmailService;
        }

        [HttpPost("google/login")]
        public async Task<IActionResult> GoogleLogin([FromBody] string googleToken)
        {
            if (string.IsNullOrWhiteSpace(googleToken))
                return BadRequest();

            var payload = await _jwtTokenService.VerifyGoogleToken(googleToken);

            if (payload == null)
                return BadRequest("Токен авторизации устарел");

            string provider = "Google";
            var info = new UserLoginInfo(provider, payload.Subject, provider);
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                    return BadRequest();

                var resultUserLogin = await _userManager.AddLoginAsync(user, info);
                if (!resultUserLogin.Succeeded)
                    return BadRequest();
            }

            var time = await _userManager.GetLockoutEndDateAsync(user);
            if (time != null)
                return BadRequest($"Аккаунт заблокирован до {time.ToString()}");

            var token = await _jwtTokenService.CreateToken(user);
            return Ok(new { token });
        }

        [HttpPost("google/register")]
        public async Task<IActionResult> GoogleRegister([FromBody] GoogleRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var payload = await _jwtTokenService.VerifyGoogleToken(model.Token);

                if (payload == null)
                    return BadRequest("Токен устарел или недействителен");

                string provider = "Google";
                var info = new UserLoginInfo(provider, payload.Subject, provider);
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(payload.Email);
                    if (user == null)
                    {
                        user = new UserEntity
                        {
                            Email = payload.Email,
                            FirstName = model.FirstName,
                            UserName = payload.Email,
                            LastName = model.LastName,
                            Image = model.Image
                        };

                        var resultCreate = await _userManager.CreateAsync(user);
                        if (!resultCreate.Succeeded)
                            return BadRequest(resultCreate.Errors);

                        var resultRole = await _userManager.AddToRoleAsync(user, Roles.User);
                        if (!resultRole.Succeeded)
                            return BadRequest(resultRole.Errors);
                    }

                    var resultUserLogin = await _userManager.AddLoginAsync(user, info);
                    if (!resultUserLogin.Succeeded)
                        return BadRequest(resultUserLogin.Errors);
                }

                var token = await _jwtTokenService.CreateToken(user);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в GoogleRegister: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);

                if (user == null)
                    return BadRequest("Неверный адрес электронной почты или пароль!");

                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                    return BadRequest("Неверный адрес электронной почты или пароль!");

                var time = await _userManager.GetLockoutEndDateAsync(user);
                if (time != null)
                    return BadRequest($"Аккаунт заблокирован до {time.ToString()}");

                var token = await _jwtTokenService.CreateToken(user);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                var user = _mapper.Map<UserEntity>(model);

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Отправить письмо с подтверждением
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                    var emailMessage = new Message
                    {
                        To = user.Email,
                        Subject = "Подтверждение электронной почты",
                        Body = $"Пожалуйста, подтвердите свою электронную почту, перейдя по <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>ссылке</a>."
                    };

                    _smtpEmailService.Send(emailMessage);

                    result = await _userManager.AddToRoleAsync(user, Roles.User);

                    var jwtToken = await _jwtTokenService.CreateToken(user);
                    return Ok(new { token = jwtToken });
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

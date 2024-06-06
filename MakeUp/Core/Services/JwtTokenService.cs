﻿using Core.Interfaces;
using Google.Apis.Auth;
using Infastructure.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<UserEntity> _userManager;

        public JwtTokenService(IConfiguration config, UserManager<UserEntity> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<string> CreateToken(UserEntity user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            List<Claim> claims = new()
            {
                new Claim("email", user.Email),
                new Claim("name", $"{user.FirstName} {user.LastName}"),
                new Claim("emailConfirmed", $"{user.EmailConfirmed}"),
                new Claim("image", user.Image ?? string.Empty)
            };

            foreach (var role in roles)
                claims.Add(new Claim("roles", role));

            var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<String>("JwtSecretKey")));
            var signinCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                signingCredentials: signinCredentials,
                expires: DateTime.Now.AddDays(10),
                claims: claims
            );
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string tokenId)
        {
            try
            {
                string clientID = _config["GoogleAuthSettings:ClientId"];

                var setting = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { clientID }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(tokenId, setting);
                return payload;
            }
            catch (Exception ex)
            {
                // Добавляем логирование ошибки
                Console.WriteLine($"Error in VerifyGoogleToken: {ex.Message}");
                throw; // Пробрасываем исключение дальше
            }
        }

    }
}

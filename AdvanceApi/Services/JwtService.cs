using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AdvanceApi.Services
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpireMinutes { get; set; } = 60;
    }

    public interface IJwtService
    {
        string GenerateToken(string subject, IEnumerable<Claim>? additionalClaims = null);
        bool TryValidateToken(string token, out ClaimsPrincipal? principal);
    }

    public class JwtService : IJwtService
    {
        private readonly JwtSettings _settings;
        private readonly byte[] _keyBytes;
        private readonly SigningCredentials _signingCredentials;
        private readonly SymmetricSecurityKey _signingKey;
        private readonly ILogger<JwtService>? _logger;

        public JwtService(IOptions<JwtSettings> options, ILogger<JwtService>? logger = null)
        {
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;

            if (string.IsNullOrWhiteSpace(_settings.Key) || _settings.Key.Length < 32)
                throw new ArgumentException("JWT Key missing or too short. Provide a secure key with at least 32 characters.");

            _keyBytes = Encoding.UTF8.GetBytes(_settings.Key);
            _signingKey = new SymmetricSecurityKey(_keyBytes);
            _signingCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
        }

        public string GenerateToken(string subject, IEnumerable<Claim>? additionalClaims = null)
        {
            var now = DateTime.UtcNow;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            if (additionalClaims != null)
                claims.AddRange(additionalClaims);

            var jwt = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_settings.ExpireMinutes),
                signingCredentials: _signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public bool TryValidateToken(string token, out ClaimsPrincipal? principal)
        {
            principal = null;
            if (string.IsNullOrWhiteSpace(token)) return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                ValidateIssuer = !string.IsNullOrWhiteSpace(_settings.Issuer),
                ValidIssuer = _settings.Issuer,
                ValidateAudience = !string.IsNullOrWhiteSpace(_settings.Audience),
                ValidAudience = _settings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            };

            try
            {
                principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal != null;
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger?.LogWarning(ex, "Token expired.");
                return false;
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Token validation failed.");
                return false;
            }
        }
    }
}
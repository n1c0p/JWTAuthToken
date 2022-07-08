using JWTAuthToken.DataAccessLayer.CSModel;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWTAuthToken.BusinessLayer.Repository.JWTManager
{
    public class JWTManagerRepository : IJWTManagerRepository
	{


		private readonly IConfiguration iconfiguration;

		public JWTManagerRepository(IConfiguration iconfiguration)
		{
			this.iconfiguration = iconfiguration;
		}

		public Tokens GenerateToken(UsersCustom user)
		{
			return GenerateJWTTokens(user);
		}

		public Tokens GenerateRefreshToken(UsersCustom user)
		{
			return GenerateJWTTokens(user);
		}

		public Tokens GenerateJWTTokens(UsersCustom user)
		{	

			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var tokenKey = Encoding.UTF8.GetBytes(iconfiguration["JWT:Key"]);
				var tokenDescriptor = new SecurityTokenDescriptor
				{
					Subject = new ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.Id)),
						new Claim(ClaimTypes.Name, user.Username),
                        new Claim("Nome", user.Nome),
                        new Claim("Cognome", user.Cognome),
						new Claim(ClaimTypes.Role, "Administrator")


                    }),
					Expires = DateTime.Now.AddMinutes(1),
					SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
				};
				var token = tokenHandler.CreateToken(tokenDescriptor);
				var refreshToken = GenerateRefreshToken();
				return new Tokens { AccessToken = tokenHandler.WriteToken(token), RefreshToken = refreshToken, UserdId = user.Id };
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public string GenerateRefreshToken()
		{
			var randomNumber = new byte[256];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
				return Convert.ToBase64String(randomNumber);
			}
		}

		public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			var Key = Encoding.UTF8.GetBytes(iconfiguration["JWT:Key"]);

			var tokenValidationParameters = new TokenValidationParameters
			{
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ClockSkew = TimeSpan.Zero
            };

			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
			JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
			if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}


			return principal;
		}
	}
}

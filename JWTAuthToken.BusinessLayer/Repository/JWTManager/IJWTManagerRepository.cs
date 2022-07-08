using JWTAuthToken.DataAccessLayer.CSModel;
using System.Security.Claims;

namespace JWTAuthToken.BusinessLayer.Repository
{
    public interface IJWTManagerRepository
	{	
		Tokens GenerateToken(UsersCustom user);
		Tokens GenerateRefreshToken(UsersCustom user);
		ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
	}
}

using JWTAuthToken.BusinessLayer.Repository;
using JWTAuthToken.DataAccessLayer.CSModel;
using JWTAuthToken.DataAccessLayer.EFModels;
using JWTAuthToken.WEBAPI.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JWTAuthToken.WEBAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase, IUsersController
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IJWTManagerRepository _jWTManagerRepository;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger, IUsersRepository usersRepository, IJWTManagerRepository jWTManagerRepository)
        {
            _logger = logger;
            _usersRepository = usersRepository;
            _jWTManagerRepository = jWTManagerRepository;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AuthenticateAsync(UsersCustom user)
        {
            UsersCustom validUser = await _usersRepository.IsValidUserAsync(user);

            if (validUser == null)
            {
                return Unauthorized("Incorrect username or password!");
            }

            var token = _jWTManagerRepository.GenerateToken(validUser);

            if (token == null)
            {
                return Unauthorized("Invalid Attempt!");
            }

            // saving refresh token to the db
            UserRefreshToken obj = new UserRefreshToken
            {
                RefreshToken = token.RefreshToken,
                UserName = validUser.Username,
                UserId = validUser.Id
            };

            // cancello tutti i token refresh appesi dell'untente
            _usersRepository.DeleteUserRefreshTokens(validUser.Id);
            //aggiungo il nuovo token refresh nel db
            _usersRepository.AddUserRefreshTokens(obj);
            
            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Refresh(Tokens token)
        {
            var principal = _jWTManagerRepository.GetPrincipalFromExpiredToken(token.AccessToken);
            //var userName = principal.Identity?.Name;
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var isAdmin = User.IsInRole("Administrator");

            UsersCustom user = await _usersRepository.utenteDaId(Convert.ToInt32(userId));

            //retrieve the saved refresh token from database
            var savedRefreshToken = _usersRepository.GetSavedRefreshTokens(user.Username, token.RefreshToken);

            if (savedRefreshToken.RefreshToken != token.RefreshToken)
            {
                return Unauthorized("Invalid attempt!");
            }

            var newJwtToken = _jWTManagerRepository.GenerateRefreshToken(user);

            if (newJwtToken == null)
            {
                return Unauthorized("Invalid attempt!");
            }

            // saving refresh token to the db
            UserRefreshToken obj = new UserRefreshToken
            {
                RefreshToken = newJwtToken.RefreshToken,
                UserName = user.Username,
                UserId = user.Id
            };

            _usersRepository.DeleteUserRefreshTokens(user.Id);
            _usersRepository.AddUserRefreshTokens(obj);


            return Ok(newJwtToken);
            
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AggiungiUtenti([FromBody] UsersCustom user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var idUtenteAggiunto = await _usersRepository.AggiungiUtenti(user);
                    
                    if (idUtenteAggiunto.Id > 0)
                    {
                        return Ok(idUtenteAggiunto);
                    }
                    else
                    {
                        return NotFound();
                    }
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return BadRequest(ex);
                }

            }

            return BadRequest();
        }
    }
}

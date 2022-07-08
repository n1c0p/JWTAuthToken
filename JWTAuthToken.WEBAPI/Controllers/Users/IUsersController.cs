using JWTAuthToken.DataAccessLayer.CSModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JWTAuthToken.WEBAPI.Interface
{
    public interface IUsersController
    {
        Task<IActionResult> AggiungiUtenti([FromBody] UsersCustom user);
        Task<IActionResult> AuthenticateAsync(UsersCustom user);
        Task<IActionResult> Refresh(Tokens token);
    }
}

using JWTAuthToken.DataAccessLayer.CSModel;
using JWTAuthToken.DataAccessLayer.EFModels;
using System.Threading.Tasks;

namespace JWTAuthToken.BusinessLayer.Repository
{
    public interface IUsersRepository
    {
        Task<UsersCustom> IsValidUserAsync(UsersCustom user);
        UserRefreshToken AddUserRefreshTokens(UserRefreshToken user);
        void DeleteUserRefreshTokens(int id);
        UserRefreshToken GetSavedRefreshTokens(string username, string refreshToken);

        Task<UsersCustom> utenteDaId(int id);
        Task<Users> AggiungiUtenti(UsersCustom user);
    }
}

using JWTAuthToken.DataAccessLayer.CSModel;
using JWTAuthToken.DataAccessLayer.EFModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuthToken.BusinessLayer.Repository
{
    public class UsersRepository: IUsersRepository
    {
        private readonly JWTContext _context;

        public UsersRepository(JWTContext contex)
        {
            _context = contex;
        }


        public async Task<UsersCustom> utenteDaId(int id)
        {
            try
            {

                return await (from U in _context.Users
                              where U.Id == id
                              select new UsersCustom
                              {
                                Id = U.Id,
                                Username = U.Username,
                                Nome = U.Nome,
                                Cognome = U.Cognome
                                  
                              }).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Users> AggiungiUtenti(UsersCustom user)
        {
            using var transaction = _context.Database.BeginTransaction();


            try
            {

                Users utenteAggiunto = new Users()
                {
                    Nome = user.Nome,
                    Cognome = user.Cognome,
                    Username = user.Username,
                    Password = MD5Hash(user.Password)
                    
                };

                await _context.Users.AddAsync(utenteAggiunto);
                await _context.SaveChangesAsync();



                transaction.Commit();

                return utenteAggiunto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UsersCustom> IsValidUserAsync(UsersCustom user)
        {

            UsersCustom usersCustom = await (from U in _context.Users
                                             where U.Username == user.Username
                                             where U.Password == MD5Hash(user.Password)
                                             select new UsersCustom
                                             {
                                                 Id = U.Id,
                                                 Nome = U.Nome,
                                                 Cognome = U.Cognome,
                                                 Username = U.Username,
                                                 Password = U.Password
                                             }).FirstOrDefaultAsync();

            return usersCustom;

        }

        public UserRefreshToken AddUserRefreshTokens(UserRefreshToken user)
        {
            
            _context.UserRefreshToken.Add(user);
            _context.SaveChanges();
            return user;
        }

        public void DeleteUserRefreshTokens(int id)
        {
            List<UserRefreshToken> itemDelete = _context.UserRefreshToken.Where(x => x.UserId.Equals(id)).ToList();
            if (itemDelete != null)
            {
                _context.UserRefreshToken.RemoveRange(itemDelete);
                _context.SaveChanges();
            }

            
        }

        public UserRefreshToken GetSavedRefreshTokens(string username, string refreshToken)
        {
            return _context.UserRefreshToken.FirstOrDefault(x => x.UserName == username && x.RefreshToken == refreshToken && x.IsActive == true);
        }

        private static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
    }
}

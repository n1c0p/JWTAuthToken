using System;
using System.Collections.Generic;

#nullable disable

namespace JWTAuthToken.DataAccessLayer.EFModels
{
    public partial class Users
    {
        public Users()
        {
            UserRefreshToken = new HashSet<UserRefreshToken>();
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public virtual ICollection<UserRefreshToken> UserRefreshToken { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace JWTAuthToken.DataAccessLayer.EFModels
{
    public partial class UserRefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string RefreshToken { get; set; }
        public bool? IsActive { get; set; }

        public virtual Users User { get; set; }
    }
}

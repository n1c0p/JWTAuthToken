namespace JWTAuthToken.DataAccessLayer.CSModel
{
    public class Tokens
	{
        public int UserdId { get; set; }
        public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
	}
}

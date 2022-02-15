internal class MyAuthServer
{
    private HashSet<string> _validTokens = new HashSet<string>();
    public MyAuthServer()
    {
    }

    public bool IsTokenValid(string token)
    {
        return _validTokens.Contains(token);
    }

    public string?  TryLogin(string name, string password)
    {
        if(name == "admin" && password == "admin")
        {
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                var buffer = new byte[32];
                rng.GetBytes(buffer);
                var token = Convert.ToBase64String(buffer);
                _validTokens.Add(token);
                return token;
            }
        }
        return null;
    }

    public void Logout(string token)
    {
        _validTokens.Remove(token);
    }
}
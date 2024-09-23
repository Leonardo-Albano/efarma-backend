namespace EFarma.Config
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int saltSize = 128 / 8;
        private const int keySize = 256 / 8;
        public string Hash(string password)
        {
            return "";
        }

        public bool Verify(string passwordHash, string inputPassword)
        {
            return false;
        }
    }
}

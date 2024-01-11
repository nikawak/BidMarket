using System.Security.Cryptography;
using System.Text;

namespace BidMarket.Services
{
    public class Hasher
    {
        private string _salt = "kjn23jj23dj2ndjasdfw";
        public string Compute(string pwd)
        {
            var sha = SHA512.Create();
            var pwdBytes = Encoding.UTF8.GetBytes(pwd + _salt);
            var pwdHashed = Encoding.UTF8.GetString(sha.ComputeHash(pwdBytes));
            return pwdHashed;
        }
    }
}

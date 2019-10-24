using EsLSCor.Entities;
using EsLSCor.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace EsLSCor.Services
{
    public interface IUserService
    {
        bool IsDbEmpty();
        string GetToken(string username, string password);
        UserCreationResult CreateNew(string username, string password);
    }

    public class UserService : IUserService
    {
        private readonly UserDbContext _context;
        private readonly AuthOptions _options;

        public UserService(UserDbContext context, IOptions<AuthOptions> options)
        {
            _context = context;
            _options = options.Value;
        }

        public bool IsDbEmpty()
        {
            return _context.Users.Count() == 0;
        }

        public string GetToken(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(x => x.Username == username && x.PwEnc == GetPwEnc(password));
            if (user != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Convert.FromBase64String(_options.JwtSecret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("friendly_name", user.Username),
                        new Claim(ClaimTypes.Role, user.Role),
                    }),
                    Audience = _options.JwtAudience,
                    Issuer = _options.JwtIssuer,
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }
            else
                return null;
        }

        public UserCreationResult CreateNew(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || username.Length > 16 || !Regex.Match(password, @"^[a-zA-Z0-9_]+$").Success)
                return UserCreationResult.BadUsername;

            // Don't allow creating accounts with poor passwords by talking to the API directly
            if (string.IsNullOrEmpty(password) || !Regex.Match(password, @"^(?=\D*\d)[a-zA-Z0-9]{8,}$").Success)
                return UserCreationResult.WeakPassword;

            // Don't let caller overwrite existing accounts
            if (_context.Users.Any(x => x.Username == username))
                return UserCreationResult.AlreadyExists;

            // Add new user to database
            _context.Users.Add(new DbUserModel()
            {
                CreationStamp = DateTime.UtcNow,
                Username = username,
                PwEnc = GetPwEnc(password),
                Role = Role.Admin
            });
            _context.SaveChanges();

            return UserCreationResult.Ok;
        }

        // Using this method instead of .NET's Convert as it offers better performance and no forced '-' delimiter
        private string ByteArrayToHexString(byte[] bytes)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);
            string hexAlphabet = "0123456789abcdef";

            foreach (byte b in bytes)
            {
                result.Append(hexAlphabet[b >> 4]);
                result.Append(hexAlphabet[b & 0xF]);
            }

            return result.ToString();
        }

        private string GetPwEnc(string plainPassword)
        {
            using AesCryptoServiceProvider aes = new AesCryptoServiceProvider
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.Zeros
            };
            byte[] rawPwBytes = Encoding.Unicode.GetBytes(plainPassword);
            using var sha256 = new SHA256CryptoServiceProvider();
            using var enc = aes.CreateEncryptor(Convert.FromBase64String(_options.PwEncKey), Convert.FromBase64String(_options.PwEncIV));
            return ByteArrayToHexString(sha256.ComputeHash(enc.TransformFinalBlock(rawPwBytes, 0, rawPwBytes.Length)));
        }
    }
}

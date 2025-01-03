using E_Book.Dto;
using E_Book.Models;
using E_Book.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace E_Book.Service
{
    public interface IUserService
    {
        AuthResponseDto Authenticate(string username, string password);
        void Register(UserRegister user);

        ResponseDto<object> GetAllUsers();

        public Task<ResponseDto<object>> SaveUser(UserDto userDto);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public ResponseDto<object> GetAllUsers()
        {
            var users = _repository
                .GetAllUser()
                .Select(ConvertUser)
                .ToList();

            return new ResponseDto<object>
            {
                Data = users,
                Message = "Users retrieved successfully.",
                Success = true
            };
        }

        public async Task<ResponseDto<object>> SaveUser(UserDto userDto)
        {
            
            var user = _repository.FindByEmail(userDto.Email);
            if (user != null)
                return new ResponseDto<object>
                {
                    Success = false,
                    Message = $"User with Email {userDto.Email} already exists."
                };

            try
            {
                _repository.AddUser(new User
                {
                    Fullname = userDto.Fullname,
                    Email = userDto.Email,
                    Username = userDto.Username,
                    PasswordHash = HashPassword(userDto.Password),
                    Role = userDto.Role
                });
                return new ResponseDto<object>
                {
                    Success = true,
                    Message = "User saved successfully."
                };

            }
            catch (Exception e)
            {
                return new ResponseDto<object>
                {
                    Success = false,
                    Message = $"An error occurred: {e.Message}"
                };
            }
        }


        private UserDto ConvertUser(User user)
        {
            return new UserDto
            {
                Fullname = user.Fullname,
                Email = user.Email,
                Username = user.Username,
                Role = user.Role
            };
        }


        public AuthResponseDto Authenticate(string username, string password)
        {
            var user = _repository.GetUserByUsername(username);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                return null; // Invalid username or password
            }

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("id", user.Id.ToString()),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:TokenValidityMins"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                Username = user.Username,
                Role = user.Role,
                UserId = user.Id,
                Fullname = user.Fullname
            };
        }



        public void Register(UserRegister user)
        {
            var newUser = new User
            {
                Fullname = user.Fullname,
                Email = user.Email,
                Username = user.Email,
                PasswordHash = HashPassword(user.Password),
                Role = user.Role
            };
            _repository.AddUser(newUser);
        }

        private string HashPassword(string password)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Hashing:Key"]);
            using var hmac = new HMACSHA256(key);
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Hashing:Key"]);
            using var hmac = new HMACSHA256(key);
            var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return storedHash == computedHash;
        }
    }
}

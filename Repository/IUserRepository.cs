using E_Book.Data;
using E_Book.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Book.Repository
{
    public interface IUserRepository
    {
        User GetUserByUsername(string username);
        void AddUser(User user);

        List<User> GetAllUser();
        User FindByEmail(string email);
    }

    public class UserRepository(ApplicationDbContext context) : IUserRepository
    {
        public User GetUserByUsername(string username)
        {
            var user = context.Users.SingleOrDefault(u => u.Username == username);
            if (user == null)
            {
                Console.WriteLine("User not found in database for username: " + username);
            }
            return user;
        }

        public void AddUser(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
        }

        public List<User> GetAllUser()
        {
            return context.Users
                .ToList();
        }

        public User? FindByEmail(string email)
        {
            return context.Users.FirstOrDefault(u => u.Email == email);
        }
    }
}

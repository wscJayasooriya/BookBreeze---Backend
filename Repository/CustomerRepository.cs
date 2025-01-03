using E_Book.Data;
using E_Book.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Book.Repository
{
    public class CustomerRepository(ApplicationDbContext context)
    {

        internal List<Customer> GetAllCustomers()
        {
            return context.Customers
                .OrderByDescending(c => c.RegisterDate)
                .ToList();
        }

        public void SaveCustomer(Customer customer)
        {
            context.Customers.Add(customer);
            context.SaveChanges();
        }

        internal Customer? FindByUserId(int userId) => context.Customers.FirstOrDefault(c => c != null && c.UserId.Equals(userId));

        public async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            return await context.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Customer> GetCustomerByIdAsync(int userId)
        {
            return await context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
        }


        public void UpdateCustomer(Customer customer)
        {
            context.Customers.Update(customer);
            context.SaveChanges();
        }


        public async Task<int> GetCustomerCountAsync()
        {
            return await context.Customers.CountAsync();
        }
    }
}

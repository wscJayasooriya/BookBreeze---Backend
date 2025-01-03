using E_Book.Dto;
using E_Book.Models;
using E_Book.Repository;
using E_Book.Shared;

namespace E_Book.Service
{
    public class CustomerService(CustomerRepository repository)
    {
        private readonly CustomerRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        public ResponseDto<object> GetAllCustomers()
        {
            List<CustomerDto> customers = _repository
                .GetAllCustomers()
                .Select(ConvertCustomer)
                .ToList();

            return new ResponseDto<object>
            {
                Data = customers,
                Message = "Customers retrieved successfully.",
                Success = true
            };
        }

        public async Task<ResponseDto<object>> SaveCustomer(CustomerDto customerDto)
        {
            try
            {
                // Check if a customer with the given UserId already exists
                var existingCustomer = _repository.FindByUserId(customerDto.UserId);

                if (existingCustomer != null)
                {
                    // Update the existing customer details
                    existingCustomer.Firstname = customerDto.Firstname;
                    existingCustomer.Lastname = customerDto.Lastname;
                    existingCustomer.Email = customerDto.Email;
                    existingCustomer.ContactNo = customerDto.ContactNo;
                    existingCustomer.City = customerDto.City;
                    existingCustomer.PostalCode = customerDto.PostalCode;
                    existingCustomer.Address = customerDto.Address;
                    existingCustomer.RegisterDate = existingCustomer.RegisterDate; // Update the register date

                    _repository.UpdateCustomer(existingCustomer); // Call repository update method
                }
                else
                {
                    // Add a new customer if no existing record is found
                    var newCustomer = new Customer
                    {
                        UserId = customerDto.UserId,
                        Firstname = customerDto.Firstname,
                        Lastname = customerDto.Lastname,
                        Email = customerDto.Email,
                        ContactNo = customerDto.ContactNo,
                        City = customerDto.City,
                        PostalCode = customerDto.PostalCode,
                        Address = customerDto.Address,
                        RegisterDate = customerDto.RegisterDate
                    };

                    _repository.SaveCustomer(newCustomer); // Call repository save method
                }

                return new ResponseDto<object>
                {
                    Success = true,
                    Message = existingCustomer != null
                        ? "Customer updated successfully."
                        : "Customer saved successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto<object>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }



        private CustomerDto ConvertCustomer(Customer? customer)
        {
            return new CustomerDto
            {
                UserId = customer.UserId,
                Firstname = customer.Firstname,
                Lastname = customer.Lastname,
                Email = customer.Email,
                Address = customer.Address,
                City = customer.City,
                ContactNo = customer.ContactNo,
                PostalCode = customer.PostalCode,
                RegisterDate = customer.RegisterDate
            };
        }

        public async Task<ResponseDto<CustomerDto>> GetCustomerByEmailAsync(string email)
        {
            try
            {
                var customer = await _repository.GetCustomerByEmailAsync(email);
                if (customer == null)
                {
                    return new ResponseDto<CustomerDto>
                    {
                        Success = false,
                        Message = "Customer not found."
                    };
                }

                var customerDto = new CustomerDto
                {
                    UserId = customer.UserId,
                    Firstname = customer.Firstname,
                    Lastname = customer.Lastname,
                    Email = customer.Email,
                    ContactNo = customer.ContactNo,
                    City = customer.City,
                    PostalCode = customer.PostalCode,
                    Address = customer.Address,
                    RegisterDate = customer.RegisterDate
                };

                return new ResponseDto<CustomerDto>
                {
                    Success = true,
                    Message = "Customer retrieved successfully.",
                    Data = customerDto
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto<CustomerDto>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving the customer: {ex.Message}"
                };
            }
        }

        public async Task<ResponseDto<CustomerDto>> GetCustomerByIdAsync(int userId)
        {
            try
            {
                var customer = await _repository.GetCustomerByIdAsync(userId);
                if (customer == null)
                {
                    return new ResponseDto<CustomerDto>
                    {
                        Success = false,
                        Message = "Customer not found."
                    };
                }

                var customerDto = new CustomerDto
                {
                    UserId = customer.UserId,
                    Firstname = customer.Firstname,
                    Lastname = customer.Lastname,
                    Email = customer.Email,
                    ContactNo = customer.ContactNo,
                    City = customer.City,
                    PostalCode = customer.PostalCode,
                    Address = customer.Address,
                    RegisterDate = customer.RegisterDate
                };

                return new ResponseDto<CustomerDto>
                {
                    Success = true,
                    Message = "Customer retrieved successfully.",
                    Data = customerDto
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto<CustomerDto>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving the customer: {ex.Message}"
                };
            }
        }

        public async Task<int> GetCustomerCountAsync()
        {
            return await _repository.GetCustomerCountAsync();
        }
    }
}

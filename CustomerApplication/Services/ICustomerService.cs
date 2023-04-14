using CustomerApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerApplication.Services
{
	public interface ICustomerService
	{
		IEnumerable<Customer> GetAllCustomers();
		Customer GetCustomerById(Guid id);
		IEnumerable<Customer> GetAllCustomerByFirstName(string firstName);
		Customer GetCustomerByFirstAndLastName(string firstName, string LastName);
		IEnumerable<Customer> GetAllCustomersBetweenAge(int minAge, int maxAge);
		IEnumerable<Customer> GetAllCustomersByCountry(string country);
		Task<int> AddCustomer(Customer customer);
		Task<int> UpdateCustomer(Guid id, Customer customer);
		IEnumerable<Customer> UpdateAllCustomersAgeByLastName(string lastName, int age);
		void DeleteCustomerById(Guid id);
		void DeleteMultipleCustomerByIds(IEnumerable<Guid> ids);
	}
}

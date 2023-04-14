using CustomerApplication.Data;
using CustomerApplication.Models;
using CustomerApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerApplication.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class CustomerController : Controller
	{
		private readonly ICustomerService _customerService;

		public CustomerController(ICustomerService customerService)
		{
			this._customerService = customerService;
		}

		[HttpPost]
		public async Task<IActionResult> AddCustomer(CustomerRequest customerRequest)
		{
			try
			{
				var customer = new Customer()
				{
					Id = Guid.NewGuid(),
					FirstName = customerRequest.FirstName,
					LastName = customerRequest.LastName,
					Age = customerRequest.Age,
					Address = customerRequest.Address
				};

				await _customerService.AddCustomer(customer);

				return Ok(customer);
			}
			catch (DbUpdateException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet]
		public IActionResult GetAllCustomers()
		{
			var result = _customerService.GetAllCustomers();
			return Ok(result);
		}


		[HttpGet]
		public IActionResult GetCustomerById(Guid id)
		{
			try
			{
				var result = _customerService.GetCustomerById(id);
				return Ok(result);
			}
			catch(Exception ex)
			{
				return NotFound(ex.Message);
			}
		}

		[HttpGet]
		public IActionResult GetCustomerByFirstName(string firstName)
		{
			var customer = _customerService.GetAllCustomerByFirstName(firstName);
			if (customer.Any())
			{
				return Ok(customer);
			}
			return NotFound();
		}

		[HttpGet]
		public IActionResult GetCustomerByFirstAndLastName(string firstName, string lastName)
		{
			var customer = _customerService.GetCustomerByFirstAndLastName(firstName, lastName);
			if (customer != null)
			{
				return Ok(customer);
			}
			return NotFound();
		}

		[HttpGet]
		public IActionResult GetCustomersBetweenAge(int minAge, int maxAge)
		{
			var customers = _customerService.GetAllCustomersBetweenAge(minAge, maxAge);
			if (customers.Any())
			{
				return Ok(customers);
			}
			return NotFound();
		}

		[HttpGet]
		public IActionResult GetCustomerByCountry(string country)
		{
			var customers = _customerService.GetAllCustomersByCountry(country);
			if (customers.Any())
			{
				return Ok(customers);
			}
			return NotFound();
		}

		[HttpPut]
		public async Task<IActionResult> UpdateCustomeById(Guid id, CustomerRequest customerRequest)
		{
			try
			{
				var customer = new Customer()
				{
					Id = id,
					FirstName = customerRequest.FirstName,
					LastName = customerRequest.LastName,
					Age = customerRequest.Age,
					Address = customerRequest.Address
				};

				var result = await _customerService.UpdateCustomer(id, customer);

				return Ok(customer);
			}
			catch (Exception ex)
			{
				return NotFound(ex.Message);
			}
		}

		[HttpPut]
		public IActionResult UpdateAllCustomersAgeByLastName(string lastName, int newAge)
		{
			try
			{
				var result = _customerService.UpdateAllCustomersAgeByLastName(lastName, newAge);

				return Ok(result);
			}
			catch (Exception ex)
			{
				return NotFound(ex.Message);
			}
		}

		[HttpDelete]
		public IActionResult DeleteCustomerById(Guid id)
		{
			try
			{
				_customerService.DeleteCustomerById(id);
				return Ok();
			}
			catch (Exception ex)
			{
				return NotFound(ex.Message);
			}
		}

		[HttpDelete]
		public IActionResult DeleteMultipleCustomerByIds(IEnumerable<Guid> ids)
		{
			try
			{
				_customerService.DeleteMultipleCustomerByIds(ids);
				return Ok();
			}
			catch (Exception ex)
			{
				return NotFound(ex.Message);
			}
		}
	}
}

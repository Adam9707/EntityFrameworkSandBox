using EFSandBoxAPI.Contracts;
using EFSandBoxAPI.DBContexts;
using EFSandBoxAPI.DTO;
using EFSandBoxAPI.Models;
using EFSandBoxAPI.Models.Views;
using EFSandBoxAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Data.Entity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EFSandBoxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public ValuesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //Get repository
            var customerRepository = unitOfWork.Repository<Customer>();

            //Query samples
            var customer = await customerRepository.GetOneAsync(c=>c.CustomerId == 1);
            //It is possible to add many Include Expresion
            var customerWitchInclude = await customerRepository.GetOneAsync(c => c.CustomerId == 1, c=>c.Store);
            var customerList = await customerRepository.GetManyAsync(c => c.CustomerId <= 10);

            //Pagination sample 
            //Create PaginationParameters object
            var sortByColumn = "FirstName";
            var filter = "as";

            var parameters = new PaginationParameters<VIndividualCustomer>()
            {
                PageSize = 10,
                PageNumber = 1,
                SortBy = sortByColumn,
                SortByDesceding = true,
                Filterpredicate = v => v.FirstName.ToLower().Contains(filter),
            };

            //Get PagedResult 
            var pageResult = await unitOfWork.Repository<VIndividualCustomer>().Pagination(parameters);

            //DML operations 
            using ((IDisposable)this.unitOfWork)
            {
                var personToUpdate = await unitOfWork.Repository<Person>().GetOneAsync(p => p.BusinessEntityId == 1);
                personToUpdate.FirstName = "UpdatedName";
                unitOfWork.Repository<Person>().Update(personToUpdate);
            }

            //DML operations witch transaction 
            using ((IDisposable)this.unitOfWork.UseTransaction())
            {
                var personToUpdate = await unitOfWork.Repository<Person>().GetOneAsync(p => p.BusinessEntityId == 2);
                personToUpdate.FirstName = "UpdatedNameNew";
                unitOfWork.Repository<Person>().Update(personToUpdate);
                //It is important to use Complete() method on the end of transaction.
                unitOfWork.Compleate();
            }

            return Ok();

        }
    }
}

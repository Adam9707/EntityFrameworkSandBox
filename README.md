# Generic Repository with Unit of Work & Pagination

## Features
- ### Generic repository
- possibility of using a where predicate
- possibility of using multiple includes
- ### Unit of Work
- easy to use transactions
- easy to use many repositories
- ### Pagination 
- Custom Filter
- Filter by multiple columns

## Use examples
```cs
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
```

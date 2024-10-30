using System;


//using System.Threading.Tasks;
using Catalog.Core.SharedKernel;
//using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Entities.ProductAggregate;

public interface IProductWriteOnlyRepository : IWriteOnlyRepository<Product, Guid>
{
    ///// <summary>
    ///// Checks if a customer with the specified email already exists asynchronously.
    ///// </summary>
    ///// <param name="email">The email to check.</param>
    ///// <returns>True if a customer with the email exists, false otherwise.</returns>
    //Task<bool> ExistsByEmailAsync(Email email);

    ///// <summary>
    ///// Checks if a customer with the specified email and current ID already exists asynchronously.
    ///// </summary>
    ///// <param name="email">The email to check.</param>
    ///// <param name="currentId">The current ID of the customer to exclude from the check.</param>
    ///// <returns>True if a customer with the email and current ID exists, false otherwise.</returns>
    //Task<bool> ExistsByEmailAsync(Email email, Guid currentId);
}
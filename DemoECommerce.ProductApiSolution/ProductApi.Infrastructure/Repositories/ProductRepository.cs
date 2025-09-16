using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories;

internal class ProductRepository(ProductDbContext context) : IProduct
{
    public async Task<Response> CreateAsync(Product entity)
    {
        try
        {
            // Check if a product with the same name already exists
            Product getProduct = await GetByAsync(_ => _.Name!.Equals(entity.Name));
            if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                return new Response(false, $"{entity.Name} already exists.");

            Product currentEntity = context.Products.Add(entity).Entity;
            await context.SaveChangesAsync();
            if (currentEntity is not null && currentEntity.Id > 0)
                return new Response(true, $"{entity.Name} created successfully.");
            else
                return new Response(false, $"Error occurred while creating {entity.Name}.");
        }
        catch (Exception ex)
        {
            // Log the original exception
            LogException.LogExceptions(ex);

            // Return a generic error message
            return new Response(false, "Error occurred while creating the product.");
        }
    }

    public async Task<Response> DeleteAsync(Product entity)
    {
        try
        {
            Product product = await FindByIdAsync(entity.Id);
            if (product is null)
                return new Response(false, $"{entity.Name} does not exist.");

            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return new Response(true, $"{entity.Name} deleted successfully.");
        }
        catch (Exception ex)
        {
            // Log the original exception 
            LogException.LogExceptions(ex);

            // Return a generic error message
            return new Response(false, "Error occurred while deleting the product.");
        }
    }

    public async Task<Product> FindByIdAsync(int id)
    {
        try
        {
            Product? product = await context.Products.FindAsync(id);
            return product is not null ? product : null!;
        }
        catch (Exception ex)
        {
            // Log the original exception 
            LogException.LogExceptions(ex);

            // Return a generic error message
            throw new Exception("Error occurred while retrieving the product.");
        }
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        try
        {
            List<Product> products = await context.Products.AsNoTracking().ToListAsync();
            return products is not null ? products : null!;
        }
        catch (Exception ex)
        {
            // Log the original exception
            LogException.LogExceptions(ex);

            // Return a generic error message
            throw new InvalidOperationException("Error occurred while retrieving the products.");
        }
    }

    public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
    {
        try
        {
            Product? product = await context.Products.Where(predicate).FirstOrDefaultAsync()!;
            return product is not null ? product : null!;
        }
        catch (Exception ex)
        {
            // Log the original exception 
            LogException.LogExceptions(ex);

            // Return a generic error message
            throw new InvalidOperationException("Error occurred while retrieving the product.");
        }
    }

    public async Task<Response> UpdateAsync(Product entity)
    {
        try
        {
            Product? product = await FindByIdAsync(entity.Id);
            if(product is null)
                return new Response(false, $"{entity.Name} not found");

            context.Entry(product).State = EntityState.Detached;
            context.Products.Update(entity);
            await context.SaveChangesAsync();
            return new Response(true, $"{entity.Name} updated successfully.");
        }
        catch (Exception ex)
        {
            // Log the original exception
            LogException.LogExceptions(ex);

            // Return a generic error message
            return new Response(false, "Error occurred while updating the product.");
        }
    }
}

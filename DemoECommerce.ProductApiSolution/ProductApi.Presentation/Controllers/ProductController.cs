using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            // Get all products from repo
            IEnumerable<Product> products = await productInterface.GetAllAsync();
            if(!products.Any())
                return NotFound("No products found.");

            // convert data from entity to DTO and return
            var (_, list) = ProductConversion.FromEntity(null!, products);
            return list!.Any() ? Ok(list) : NotFound("No products found.");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            // Get single product from the repo
            Product product = await productInterface.FindByIdAsync(id);
            if(product is null)
                return NotFound("Product requested not found");

            // convert from entity to DTO and return
            var (_product, _) = ProductConversion.FromEntity(product, null!);
            return _product is not null ? Ok(_product) : NotFound("Product requested not found");
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            // check model state is all data annotations are valid
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            // convert to entity
            Product getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.CreateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            // check model state is all data annotations are valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // convert to entity
            Product getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.UpdateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
        {
            // convert to entity
            Product getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.DeleteAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
    }
}

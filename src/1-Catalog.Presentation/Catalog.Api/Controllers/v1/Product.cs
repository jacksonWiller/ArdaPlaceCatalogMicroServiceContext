using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Catalog.Api.Hosted.Extensions;
using Catalog.Application.Products.CreateProduct;
using Catalog.Application.Products.UpdateProduct;
using Catalog.Application.Products.DeleteProduct;
using Catalog.Application.Products.GetProductById;
using Catalog.Application.Products.GetAllProducts;
using Ardalis.Result;
using Catalog.Api.Models;
using Microsoft.AspNetCore.Http;

namespace Catalog.Api.Hosted.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    ////////////////////////
    // POST: /api/products
    ////////////////////////

    /// <summary>
    /// Register a new product.
    /// </summary>
    /// <response code="200">Returns the Id of the new client.</response>
    /// <response code="400">Returns list of errors if the request is invalid.</response>
    /// <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpPost]
    [Consumes(MediaTypeNames.Multipart.FormData, MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<CreateProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromForm][Required] CreateProductCommand command) =>
        (await _mediator.Send(command)).ToActionResult();

    //////////////////////
    // PUT: /api/products
    //////////////////////

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <response code="200">Returns the response with the success message.</response>
    /// <response code="400">Returns list of errors if the request is invalid.</response>
    /// <response code="404">When no client is found by the given Id.</response>
    /// <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpPut]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromBody][Required] UpdateProductCommand command) =>
        (await _mediator.Send(command)).ToActionResult();

    //////////////////////////////
    // DELETE: /api/products/{id}
    //////////////////////////////

    // <summary>
    // Deletes the product by Id.
    // </summary>
    // <response code="200">Returns the response with the success message.</response>
    // <response code="400">Returns list of errors if the request is invalid.</response>
    // <response code="404">When no client is found by the given Id.</response>
    // <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpDelete("{id:guid}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([Required] Guid id) =>
        (await _mediator.Send(new DeleteProductCommand(id))).ToActionResult();

    ///////////////////////////
    // GET: /api/products/{id}
    ///////////////////////////

    // <summary>
    // Gets the product by Id.
    // </summary>
    // <response code="200">Returns the client.</response>
    // <response code="400">Returns list of errors if the request is invalid.</response>
    // <response code="404">When no client is found by the given Id.</response>
    // <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpGet("{id:guid}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<GetProductByIdQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([Required] Guid id) =>
        (await _mediator.Send(new GetProductByIdQuery(id))).ToActionResult();

    //////////////////////
    // GET: /api/products
    //////////////////////

    // <summary>
    // Gets a list of all products.
    // </summary>
    // <response code="200">Returns the list of clients.</response>
    // <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpGet]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<GetAllProductsQueryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllProductsQuery query) =>
        (await _mediator.Send(query)).ToActionResult();
}
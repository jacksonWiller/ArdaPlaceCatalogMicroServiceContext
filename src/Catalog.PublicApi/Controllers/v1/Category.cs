using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Catalog.PublicApi.Extensions;
using Catalog.PublicApi.Models;
using Catalog.Application.Categories.CreateCategory;
using Catalog.Application.Categories.UpdateCategory;
using Catalog.Application.Categories.DeleteCategory;
using Catalog.Application.Categories.GetCategoryById;
using Ardalis.Result;
using Catalog.Application.Categories.GetAllCategorys;

namespace Catalog.PublicApi.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class CategoriesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    ////////////////////////
    // POST: /api/categories
    ////////////////////////

    /// <summary>
    /// Register a new Category.
    /// </summary>
    /// <response code="200">Returns the Id of the new client.</response>
    /// <response code="400">Returns list of errors if the request is invalid.</response>
    /// <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpPost]
    [Consumes(MediaTypeNames.Multipart.FormData, MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<CreateCategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromForm][Required] CreateCategoryCommand command) =>
        (await _mediator.Send(command)).ToActionResult();

    //////////////////////
    // PUT: /api/categories
    //////////////////////

    /// <summary>
    /// Updates an existing Category.
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
    public async Task<IActionResult> Update([FromBody][Required] UpdateCategoryCommand command) =>
        (await _mediator.Send(command)).ToActionResult();

    //////////////////////////////
    // DELETE: /api/categories/{id}
    //////////////////////////////

    // <summary>
    // Deletes the Category by Id.
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
        (await _mediator.Send(new DeleteCategoryCommand(id))).ToActionResult();

    ///////////////////////////
    // GET: /api/categories/{id}
    ///////////////////////////

    // <summary>
    // Gets the Category by Id.
    // </summary>
    // <response code="200">Returns the client.</response>
    // <response code="400">Returns list of errors if the request is invalid.</response>
    // <response code="404">When no client is found by the given Id.</response>
    // <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpGet("{id:guid}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<GetCategoryByIdQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([Required] Guid id) =>
        (await _mediator.Send(new GetCategoryByIdQuery(id))).ToActionResult();

    //////////////////////
    // GET: /api/categories
    //////////////////////

    // <summary>
    // Gets a list of all categories.
    // </summary>
    // <response code="200">Returns the list of clients.</response>
    // <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpGet]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<GetAllCategorysQueryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllCategoriesQuery query) =>
        (await _mediator.Send(query)).ToActionResult();
}
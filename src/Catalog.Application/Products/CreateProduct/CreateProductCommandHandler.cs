using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Catalog.Core.SharedKernel;
using Catalog.Domain.DataContext;
using Catalog.Domain.Entities.ProductAggregate;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using Catalog.Application.Products.Dtos;
using System.Collections.Generic;
using Catalog.Domain.ValueObjects;

namespace Catalog.Application.Products.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly ICatalogDbContext _context;
    private readonly IValidator<CreateProductCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HttpClient _httpClient;

    public CreateProductCommandHandler(
        ICatalogDbContext context,
        IValidator<CreateProductCommand> validator,
        IUnitOfWork unitOfWork,
        IHttpClientFactory httpClientFactory
    )
    {
        _context = context;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _httpClient = httpClientFactory.CreateClient("IgnoreSslErrors");
    }

    public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Validação do comando
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<CreateProductResponse>.Invalid(validationResult.AsErrors());
        }

        // Construir a URL com os parâmetros de consulta
        //var url = $"https://localhost:44329/api/files/get-by-key?bucketName=jacksonlocal&key=catalog%2Fdownload.jpeg";

        //// Fazer a requisição GET
        //var testeImage = await _httpClient.GetAsync(url);

        //// Verificar se a resposta indica sucesso
        //testeImage.EnsureSuccessStatusCode();

        //// Ler o conteúdo da resposta
        //var image = await testeImage.Content.ReadAsStringAsync();
        //Console.WriteLine("File content: " + image);

        // Processo de upload de imagem
        string imageName = null;
        List<Image> images = [];

        if (request.Files != null && request.Files.Count > 0)
        {
            var file = request.Files[0];
            var bucketName = "jacksonlocal";  // Nome do bucket
            var prefix = "catalog";  // Prefixo opcional para o caminho do arquivo

            using var content = new MultipartFormDataContent();
            using var fileContent = new StreamContent(file.OpenReadStream())
            {
                Headers = { ContentType = new MediaTypeHeaderValue(file.ContentType) }
            };
            content.Add(fileContent, "file", file.FileName);

            // Requisição para o endpoint de upload da API
            var reqImage = await _httpClient.PostAsync($"https://localhost:44329/api/files/upload?bucketName={bucketName}&prefix={prefix}", content, cancellationToken);
            reqImage.EnsureSuccessStatusCode();

            imageName = await reqImage.Content.ReadAsStringAsync();

            images = [new(prefix, file.FileName)];
        }

        var product = new Product(
            request.Name,
            request.Category,
            request.Price,
            request.StockQuantity,
            request.SKU,
            request.Brand
        );

        
        product.AddImage(images);

        _context.Set<Product>().Add(product);
        await _unitOfWork.SaveChangesAsync();

        var response = new CreateProductResponse(product.Id);
        return Result<CreateProductResponse>.Success(response, "Product created successfully.");
    }
}
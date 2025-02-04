using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Catalog.Lambda;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DotNetServerless.Lambda.Functions;

public class CreateItemFunction
{
    private readonly IServiceProvider _serviceProvider;

    public CreateItemFunction() : this(Startup
      .BuildContainer()
      .BuildServiceProvider())
    {
    }

    public CreateItemFunction(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
    public APIGatewayProxyResponse Run(APIGatewayProxyRequest request)
    {
      var requestModel = JsonConvert.DeserializeObject(request.Body);

      var result = requestModel;

      return new APIGatewayProxyResponse { StatusCode = 201, Body = JsonConvert.SerializeObject(result) };
    }
}

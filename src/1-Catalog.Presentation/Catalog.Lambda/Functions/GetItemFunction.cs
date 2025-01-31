using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Catalog.Lambda;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DotNetServerless.Lambda.Functions;

public class GetItemFunction(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public GetItemFunction() : this(Startup
    .BuildContainer()
    .BuildServiceProvider())
    {

    }

    [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
    public APIGatewayProxyResponse Run(APIGatewayProxyRequest request)
    {

      var result = request;

      return result == null ?
        new APIGatewayProxyResponse { StatusCode = 404 } :
        new APIGatewayProxyResponse { StatusCode = 200, Body = JsonConvert.SerializeObject(result) };
    }
}

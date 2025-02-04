using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace DotNetServerless.Lambda.Functions
{
    public class GetItemFunction
  {
    private readonly IServiceProvider _serviceProvider;

    public GetItemFunction() : this(Startup
      .BuildContainer()
    .BuildServiceProvider())
    {
    }

    public GetItemFunction(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
    public async Task<APIGatewayProxyResponse> Run(APIGatewayProxyRequest request)
    {
      var result = new {Id = new Guid(request.PathParameters["id"])}; 
    
      return result == null ? 
        new APIGatewayProxyResponse {StatusCode = 404} : 
        new APIGatewayProxyResponse { StatusCode =  200,  Body = JsonConvert.SerializeObject(result)};
    }
  }
}

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace DotNetServerless.Lambda.Functions
{
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
    public async Task<APIGatewayProxyResponse> Run(APIGatewayProxyRequest request)
    {
      var result = JsonConvert.DeserializeObject(request.Body);

      return new APIGatewayProxyResponse { StatusCode =  201,  Body = JsonConvert.SerializeObject(result)};
    }
  }
}

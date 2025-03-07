//using System.Collections.Generic;
//using System.IO;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;

//namespace Catalog.Application.Services;

//public class ImageService
//{
//    private readonly HttpClient _httpClient;

//    public ImageService(HttpClient httpClient)
//    {
//        _httpClient = httpClient;
//    }

//    public async Task<string> UploadImageAsync(IFormFile file, string bucketName, string prefix)
//    {
//        using var content = new MultipartFormDataContent();
//        using var fileContent = new StreamContent(file.OpenReadStream());
//        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
//        content.Add(fileContent, "file", file.FileName);
//        content.Add(new StringContent(bucketName), "bucketName"); 
//        if (!string.IsNullOrEmpty(prefix))
//        {
//            content.Add(new StringContent(prefix), "prefix");
//        }

//        var response = await _httpClient.PostAsync("api/files/upload", content);
//        response.EnsureSuccessStatusCode();

//        return await response.Content.ReadAsStringAsync();
//    }

//    //public async Task<IEnumerable<S3ObjectDto>> GetAllImagesAsync(string bucketName, string? prefix)
//    //{
//    //    var response = await _httpClient.GetAsync($"api/files/get-all?bucketName={bucketName}&prefix={prefix}");
//    //    response.EnsureSuccessStatusCode();

//    //    return await response.Content.ReadAsAsync<IEnumerable<S3ObjectDto>>();
//    //}

//    public async Task<Stream> GetImageByKeyAsync(string bucketName, string key)
//    {
//        var response = await _httpClient.GetAsync($"api/files/get-by-key?bucketName={bucketName}&key={key}");
//        response.EnsureSuccessStatusCode();

//        return await response.Content.ReadAsStreamAsync();
//    }

//    public async Task DeleteImageAsync(string bucketName, string key)
//    {
//        var response = await _httpClient.DeleteAsync($"api/files/delete?bucketName={bucketName}&key={key}");
//        response.EnsureSuccessStatusCode();
//    }
//}
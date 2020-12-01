using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HttpTriggerApp
{
    public static class HttpTriggerFunc
    {
        [FunctionName("HttpTriggerFunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest httpRequest,
            [Blob("httprequests", Connection = "StorageConnectionString")] CloudBlobContainer outputContainer,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            await outputContainer.CreateIfNotExistsAsync();

            var requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
            var blobName = Guid.NewGuid().ToString();

            var cloudBlockBlob = outputContainer.GetBlockBlobReference(blobName);
            await cloudBlockBlob.UploadTextAsync(requestBody);

            return new OkObjectResult(blobName);
        }
    }
}

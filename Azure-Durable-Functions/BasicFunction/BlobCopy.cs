using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BasicFunction
{
    public static class BlobCopy
    {
        [FunctionName("BlobCopy")]
        public static async Task Run(
            [BlobTrigger("blobcopy/src/{name}")]Stream inBlob,
            [Blob("blobcopy/bck/{name}", FileAccess.Write)] Stream outBlob,
            string name,
            ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {inBlob.Length} Bytes");

            await inBlob.CopyToAsync(outBlob);
        }
    }
}

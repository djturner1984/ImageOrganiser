using System.Threading.Tasks;

namespace ImageOrganiser
{
    public interface IObjectUploader
    {
        Task UploadObjectAsync(string bucketName, string sourceKey, string destinationKey);
    }
}

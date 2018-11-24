using System.Threading.Tasks;

namespace ImageOrganiser
{
    public interface IFaceRecogniser
    {
        Task<RecognitionResult> Recognise(string bucketName, string key, RecognitionRequest request);
    }
}

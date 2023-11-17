
namespace ResumeMetadataLibrary.Services
{
    public interface IUtilities
    {
        public Task<Stream> InsertMetadataDocx(Stream source, string metaDataToInsert);

        public Task<Stream> InsertMetadataPdf(Stream source, string metaDataToInsert);
    }
}

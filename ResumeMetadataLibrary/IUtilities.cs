using DocumentFormat.OpenXml.Packaging;

namespace ResumeMetadataLibrary
{
    public interface IUtilities
    {
        public Task<Stream> InsertMetadata(Stream source, string metaDataToInsert);
    }
}

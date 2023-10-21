using DocumentFormat.OpenXml.Packaging;

namespace ResumeMetadataLibrary
{
    public interface IUtilities
    {
        public string InsertMetadata(String sourcePath, String destinationPath, String metaDataToInsert);

        public WordprocessingDocument OpenWordDocument(string documentPath);

        public string CombinePathIfValid(string oldPathFilename, string newFilename);
    }
}

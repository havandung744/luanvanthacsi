using BlazorInputFile;

namespace luanvanthacsi.Data.Services
{
    public interface IFileUpload
    {
        public Task UploadAsync(IFileListEntry file);
        public Task InputFile();
    }
}

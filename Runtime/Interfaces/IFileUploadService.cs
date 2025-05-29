using System;

namespace PlayFabToolkit.Interfaces
{
    /// <summary>
    /// Interface for file upload service.
    /// </summary>
    /// <remarks>
    /// This interface defines the contract for a file upload service.
    /// </remarks>
    public interface IFileUploadService
    {
        void UploadFile(byte[] data, string fileName, string contentType, Action<bool, string> callback);
    }
}

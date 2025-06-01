using System;
using System.Collections.Generic;

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
        #region Basic File Operations
        /// <summary>
        /// Upload a single file to PlayFab
        /// </summary>
        /// <param name="data">File data as a byte array</param>
        /// <param name="fileName">Name of the file to be uploaded</param>
        /// <param name="contentType">MIME type of the file</param>
        /// <param name="callback">Callback to handle the result of the upload operation</param>
        void UploadFile(byte[] data, string fileName, string contentType, Action<bool, string> callback);
    
        /// <summary>
        /// Get a list of all files for the current player
        /// </summary>
        /// <param name="callback">Callback with file list and error message</param>
        void GetFileList(Action<List<PlayFabFileInfo>, string> callback);
        
        /// <summary>
        /// Delete a specific file by name
        /// </summary>
        /// <param name="fileName">Name of the file to delete</param>
        /// <param name="callback">Callback with success status and error message</param>
        void DeleteFile(string fileName, Action<bool, string> callback);
        
        /// <summary>
        /// Download a file's content by name
        /// </summary>
        /// <param name="fileName">Name of the file to download</param>
        /// <param name="callback">Callback with file data and error message</param>
        void DownloadFile(string fileName, Action<byte[], string> callback);
        
        #endregion
        
        #region File Management with Prefixes
        
        /// <summary>
        /// Get files that start with a specific prefix
        /// </summary>
        /// <param name="prefix">The prefix to filter by</param>
        /// <param name="callback">Callback with filtered file list and error message</param>
        void GetFilesByPrefix(string prefix, Action<List<PlayFabFileInfo>, string> callback);
        
        /// <summary>
        /// Upload a file with automatic old file cleanup based on prefix and max count
        /// </summary>
        /// <param name="data">File data</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="contentType">MIME content type</param>
        /// <param name="prefix">Prefix to manage (e.g., "logs_", "saves_")</param>
        /// <param name="maxFiles">Maximum number of files to keep with this prefix</param>
        /// <param name="callback">Callback with success status and error message</param>
        void UploadFileWithManagement(byte[] data, string fileName, string contentType, string prefix, int maxFiles, Action<bool, string> callback);
        
        /// <summary>
        /// Delete oldest files with a specific prefix, keeping only the specified number
        /// </summary>
        /// <param name="prefix">The prefix to filter by</param>
        /// <param name="maxFilesToKeep">Maximum number of files to keep</param>
        /// <param name="callback">Callback with number of files deleted and error message</param>
        void CleanupFilesByPrefix(string prefix, int maxFilesToKeep, Action<int, string> callback);
        
        /// <summary>
        /// Delete all files with a specific prefix
        /// </summary>
        /// <param name="prefix">The prefix to filter by</param>
        /// <param name="callback">Callback with number of files deleted and error message</param>
        void DeleteFilesByPrefix(string prefix, Action<int, string> callback);
        
        #endregion
        
        #region Batch Operations
        
        /// <summary>
        /// Delete multiple files by name
        /// </summary>
        /// <param name="fileNames">List of file names to delete</param>
        /// <param name="callback">Callback with success status and error message</param>
        void DeleteFiles(List<string> fileNames, Action<bool, string> callback);
        
        #endregion
    }
    
    /// <summary>
    /// Information about a PlayFab file
    /// </summary>
    [System.Serializable]
    public class PlayFabFileInfo
    {
        public string FileName { get; set; }
        public string DownloadUrl { get; set; }
        public long SizeBytes { get; set; }
        public System.DateTime LastModified { get; set; }
        
        public PlayFabFileInfo(string fileName, string downloadUrl, long sizeBytes, System.DateTime lastModified)
        {
            FileName = fileName;
            DownloadUrl = downloadUrl;
            SizeBytes = sizeBytes;
            LastModified = lastModified;
        }
        
        public override string ToString()
        {
            return $"{FileName} ({SizeBytes} bytes, {LastModified})";
        }
    }
}
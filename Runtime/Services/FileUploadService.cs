using PlayFab;
using PlayFab.DataModels;
using PlayFabToolkit.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using PlayFabToolkit.Utils;

namespace PlayFabToolkit.Services
{
    public class FileUploadService : IFileUploadService
    {
        private string entityId;
        private string entityType;

        public void SetEntity(string id, string type)
        {
            entityId = id;
            entityType = type;
        }

        #region Basic File Operations

        public void UploadFile(byte[] data, string fileName, string contentType, Action<bool, string> callback)
        {
            if (!ValidateEntity(callback)) return;

            var request = new InitiateFileUploadsRequest
            {
                Entity = new EntityKey { Id = entityId, Type = entityType },
                FileNames = new List<string> { fileName }
            };

            PlayFabDataAPI.InitiateFileUploads(request, result =>
            {
                string uploadUrl = result.UploadDetails[0].UploadUrl;

                CoroutineRunner.Run(UploadToPlayFab(uploadUrl, data, contentType, () =>
                {
                    FinalizeUpload(fileName, callback);
                }, callback));
            },
            error => callback(false, error.GenerateErrorReport()));
        }

        public void GetFileList(Action<List<PlayFabFileInfo>, string> callback)
        {
            if (!ValidateEntity((success, error) => callback(null, error))) return;

            var request = new GetFilesRequest
            {
                Entity = new EntityKey { Id = entityId, Type = entityType }
            };

            PlayFabDataAPI.GetFiles(request, result =>
            {
                var fileInfos = new List<PlayFabFileInfo>();
                
                if (result.Metadata != null)
                {
                    foreach (var file in result.Metadata)
                    {
                        var fileInfo = new PlayFabFileInfo(
                            file.Key,
                            file.Value.DownloadUrl,
                            file.Value.Size,
                            file.Value.LastModified
                        );
                        fileInfos.Add(fileInfo);
                    }
                }

                callback(fileInfos, null);
            },
            error => callback(null, error.GenerateErrorReport()));
        }

        public void DeleteFile(string fileName, Action<bool, string> callback)
        {
            DeleteFiles(new List<string> { fileName }, callback);
        }

        public void DownloadFile(string fileName, Action<byte[], string> callback)
        {
            GetFileList((files, error) =>
            {
                if (error != null)
                {
                    callback(null, error);
                    return;
                }

                var targetFile = files?.FirstOrDefault(f => f.FileName == fileName);
                if (targetFile == null)
                {
                    callback(null, $"File '{fileName}' not found");
                    return;
                }

                CoroutineRunner.Run(DownloadFromUrl(targetFile.DownloadUrl, callback));
            });
        }

        #endregion

        #region File Management with Prefixes

        public void GetFilesByPrefix(string prefix, Action<List<PlayFabFileInfo>, string> callback)
        {
            GetFileList((files, error) =>
            {
                if (error != null)
                {
                    callback(null, error);
                    return;
                }

                var filteredFiles = files?.Where(f => f.FileName.StartsWith(prefix)).ToList() ?? new List<PlayFabFileInfo>();
                callback(filteredFiles, null);
            });
        }

        public void UploadFileWithManagement(byte[] data, string fileName, string contentType, string prefix, int maxFiles, Action<bool, string> callback)
        {
            // First, upload the new file
            UploadFile(data, fileName, contentType, (uploadSuccess, uploadError) =>
            {
                if (!uploadSuccess)
                {
                    callback(false, uploadError);
                    return;
                }

                // Then cleanup old files if needed
                CleanupFilesByPrefix(prefix, maxFiles, (deletedCount, cleanupError) =>
                {
                    if (cleanupError != null)
                    {
                        Debug.LogWarning($"[FileUploadService] File uploaded successfully, but cleanup failed: {cleanupError}");
                    }
                    else if (deletedCount > 0)
                    {
                        Debug.Log($"[FileUploadService] Cleaned up {deletedCount} old files with prefix '{prefix}'");
                    }

                    callback(true, null);
                });
            });
        }

        public void CleanupFilesByPrefix(string prefix, int maxFilesToKeep, Action<int, string> callback)
        {
            GetFilesByPrefix(prefix, (files, error) =>
            {
                if (error != null)
                {
                    callback(0, error);
                    return;
                }

                if (files == null || files.Count <= maxFilesToKeep)
                {
                    callback(0, null); // No cleanup needed
                    return;
                }

                // Sort by last modified date (oldest first)
                var sortedFiles = files.OrderBy(f => f.LastModified).ToList();
                var filesToDelete = sortedFiles.Take(files.Count - maxFilesToKeep).ToList();

                if (filesToDelete.Count == 0)
                {
                    callback(0, null);
                    return;
                }

                var fileNamesToDelete = filesToDelete.Select(f => f.FileName).ToList();
                DeleteFiles(fileNamesToDelete, (success, deleteError) =>
                {
                    if (success)
                    {
                        callback(filesToDelete.Count, null);
                    }
                    else
                    {
                        callback(0, deleteError);
                    }
                });
            });
        }

        public void DeleteFilesByPrefix(string prefix, Action<int, string> callback)
        {
            GetFilesByPrefix(prefix, (files, error) =>
            {
                if (error != null)
                {
                    callback(0, error);
                    return;
                }

                if (files == null || files.Count == 0)
                {
                    callback(0, null);
                    return;
                }

                var fileNamesToDelete = files.Select(f => f.FileName).ToList();
                DeleteFiles(fileNamesToDelete, (success, deleteError) =>
                {
                    if (success)
                    {
                        callback(files.Count, null);
                    }
                    else
                    {
                        callback(0, deleteError);
                    }
                });
            });
        }

        #endregion

        #region Batch Operations

        public void DeleteFiles(List<string> fileNames, Action<bool, string> callback)
        {
            if (!ValidateEntity(callback)) return;

            if (fileNames == null || fileNames.Count == 0)
            {
                callback(true, null);
                return;
            }

            var request = new DeleteFilesRequest
            {
                Entity = new EntityKey { Id = entityId, Type = entityType },
                FileNames = fileNames
            };

            PlayFabDataAPI.DeleteFiles(request, result =>
            {
                callback(true, null);
            },
            error => callback(false, error.GenerateErrorReport()));
        }

        #endregion

        #region Private Helper Methods

        private bool ValidateEntity(Action<bool, string> callback)
        {
            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityType))
            {
                callback(false, "Missing entity info. Ensure user is logged in and entity token is retrieved.");
                return false;
            }
            return true;
        }

        private IEnumerator UploadToPlayFab(string url, byte[] data, string contentType, Action onSuccess, Action<bool, string> onError)
        {
            UnityWebRequest www = UnityWebRequest.Put(url, data);
            www.method = UnityWebRequest.kHttpVerbPUT;
            www.SetRequestHeader("Content-Type", contentType);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
                onSuccess?.Invoke();
            else
                onError?.Invoke(false, www.error);
        }

        private IEnumerator DownloadFromUrl(string url, Action<byte[], string> callback)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                callback(www.downloadHandler.data, null);
            }
            else
            {
                callback(null, www.error);
            }
        }

        private void FinalizeUpload(string fileName, Action<bool, string> callback)
        {
            var finalizeRequest = new FinalizeFileUploadsRequest
            {
                Entity = new EntityKey { Id = entityId, Type = entityType },
                FileNames = new List<string> { fileName }
            };

            PlayFabDataAPI.FinalizeFileUploads(finalizeRequest,
                result => callback(true, null),
                error => callback(false, error.GenerateErrorReport()));
        }

        #endregion
    }
}
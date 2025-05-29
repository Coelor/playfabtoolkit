using PlayFab;
using PlayFab.DataModels;
using PlayFabToolkit.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
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

        public void UploadFile(byte[] data, string fileName, string contentType, Action<bool, string> callback)
        {
            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(entityType))
            {
                callback(false, "Missing entity info.");
                return;
            }

            var request = new InitiateFileUploadsRequest
            {
                Entity = new EntityKey { Id = entityId, Type = entityType },
                FileNames = new List<string> { fileName }
            };

            PlayFabDataAPI.InitiateFileUploads(request, result =>
            {
                string uploadUrl = result.UploadDetails[0].UploadUrl;

                PlayFabToolkit.Utils.CoroutineRunner.Run(UploadToPlayFab(uploadUrl, data, contentType, () =>
                {
                    FinalizeUpload(fileName, callback);
                }, callback));
            },
            error => callback(false, error.GenerateErrorReport()));
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
    }
}
using PlayFabToolkit.Interfaces;
using UnityEngine;

namespace PlayFabToolkit.Services
{
    /// <summary>
    /// Central service registry for PlayFab Toolkit services.
    /// Initialize once at application startup to access all toolkit services.
    /// </summary>
    public static class ServiceLocator
    {
        public static IAuthService AuthService { get; private set; }
        public static IFileUploadService FileUploadService { get; private set; }
        
        private static bool _isInitialized = false;
        
        /// <summary>
        /// Initializes all PlayFab Toolkit services with the specified title ID.
        /// Call this once at application startup, typically in a bootstrap scene.
        /// </summary>
        /// <param name="titleId">Your PlayFab Title ID</param>
        public static void Initialize(string titleId)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("ServiceLocator is already initialized. Skipping duplicate initialization.");
                return;
            }
            
            if (string.IsNullOrEmpty(titleId))
            {
                Debug.LogError("PlayFab Title ID cannot be null or empty.");
                return;
            }
            
            // Initialize services
            AuthService = new AuthService(titleId);
            var fileService = new FileUploadService();
            FileUploadService = fileService;
            
            // Auto-setup file upload entity when auth succeeds
            SetupEntityTokenCallback(fileService);
            
            _isInitialized = true;
            Debug.Log($"PlayFab Toolkit initialized with Title ID: {titleId}");
        }
        
        /// <summary>
        /// Checks if the ServiceLocator has been properly initialized.
        /// </summary>
        public static bool IsInitialized => _isInitialized;
        
        /// <summary>
        /// Resets the ServiceLocator state. Primarily used for testing.
        /// </summary>
        public static void Reset()
        {
            AuthService = null;
            FileUploadService = null;
            _isInitialized = false;
        }
        
        private static void SetupEntityTokenCallback(FileUploadService fileService)
        {
            // We'll enhance this when we have better login integration
            // For now, users need to manually call GetEntityToken after login
            // This will be improved in the next iteration
        }
    }
}
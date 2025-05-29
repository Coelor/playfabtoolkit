using PlayFabToolkit.Services;
using UnityEngine;

namespace PlayFabToolkit.Utils
{
    /// <summary>
    /// Bootstrap component that initializes PlayFab Toolkit services on scene load.
    /// Add this to a GameObject in your startup scene or main menu scene.
    /// </summary>
    public class ToolkitBootstrap : MonoBehaviour
    {
        [Header("PlayFab Configuration")]
        [SerializeField, Tooltip("Your PlayFab Title ID (found in Game Manager)")]
        private string titleId = "";

        [Header("Bootstrap Settings")]
        [SerializeField, Tooltip("Whether this GameObject should persist across scene loads")]
        private bool dontDestroyOnLoad = true;

        [SerializeField, Tooltip("Log initialization status to console")]
        private bool enableLogging = true;

        private void Awake()
        {
            InitializeToolkit();

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void InitializeToolkit()
        {
            if (string.IsNullOrEmpty(titleId))
            {
                Debug.LogError("PlayFab Title ID is not set in ToolkitBootstrap. Please configure it in the inspector.", this);
                return;
            }

            if (ServiceLocator.IsInitialized)
            {
                if (enableLogging)
                {
                    Debug.LogWarning("PlayFab Toolkit is already initialized. Skipping duplicate initialization.", this);
                }
                return;
            }

            try
            {
                ServiceLocator.Initialize(titleId);

                if (enableLogging)
                {
                    Debug.Log($"PlayFab Toolkit successfully initialized via ToolkitBootstrap with Title ID: {titleId}", this);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to initialize PlayFab Toolkit: {ex.Message}", this);
            }
        }

        /// <summary>
        /// Manually re-initialize the toolkit (useful for testing or title ID changes)
        /// </summary>
        [ContextMenu("Re-Initialize Toolkit")]
        public void ReinitializeToolkit()
        {
            ServiceLocator.Reset();
            InitializeToolkit();
        }

        /// <summary>
        /// Validate the current configuration
        /// </summary>
        [ContextMenu("Validate Configuration")]
        public void ValidateConfiguration()
        {
            if (string.IsNullOrEmpty(titleId))
            {
                Debug.LogError("Title ID is not configured!", this);
                return;
            }

            if (titleId.Length < 4)
            {
                Debug.LogWarning("Title ID seems too short. Please verify it's correct.", this);
                return;
            }

            Debug.Log($"Configuration looks good! Title ID: {titleId}", this);
        }

        /// <summary>
        /// Initialize the toolkit with a specific title ID (useful for testing)
        /// </summary>
        /// <param name="testTitleId">Title ID to use for initialization</param>
        public void InitializeWithTitleId(string testTitleId)
        {
            if (string.IsNullOrEmpty(testTitleId))
            {
                Debug.LogError("Test Title ID cannot be null or empty.", this);
                return;
            }

            // Temporarily override the titleId for testing
            string originalTitleId = titleId;
            titleId = testTitleId;
            
            try
            {
                InitializeToolkit();
            }
            finally
            {
                // Restore original titleId
                titleId = originalTitleId;
            }
        }
    }
}
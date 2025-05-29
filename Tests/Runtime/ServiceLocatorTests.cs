using NUnit.Framework;
using PlayFabToolkit.Services;
using PlayFabToolkit.Utils;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace PlayFabToolkit.Tests
{
    /// <summary>
    /// Tests for ServiceLocator initialization and service registration.
    /// </summary>
    public class ServiceLocatorTests
    {
        private const string TEST_TITLE_ID = "TEST123";
        
        [SetUp]
        public void Setup()
        {
            // Reset ServiceLocator before each test
            ServiceLocator.Reset();
        }
        
        [TearDown]
        public void Teardown()
        {
            // Clean up after each test
            ServiceLocator.Reset();
        }
        
        [Test]
        public void Initialize_WithValidTitleId_ServicesAreNotNull()
        {
            // Act
            ServiceLocator.Initialize(TEST_TITLE_ID);
            
            // Assert
            Assert.IsNotNull(ServiceLocator.AuthService, "AuthService should not be null after initialization");
            Assert.IsNotNull(ServiceLocator.FileUploadService, "FileUploadService should not be null after initialization");
            Assert.IsTrue(ServiceLocator.IsInitialized, "ServiceLocator should report as initialized");
        }
        
        [Test]
        public void Initialize_WithEmptyTitleId_DoesNotInitialize()
        {
            // Capture initial state
            bool wasInitialized = ServiceLocator.IsInitialized;
            
            // Act
            ServiceLocator.Initialize("");
            
            // Assert
            Assert.IsFalse(ServiceLocator.IsInitialized, "ServiceLocator should not initialize with empty title ID");
            LogAssert.Expect(LogType.Error, "PlayFab Title ID cannot be null or empty.");
        }
        
        [Test]
        public void Initialize_WithNullTitleId_DoesNotInitialize()
        {
            // Act
            ServiceLocator.Initialize(null);
            
            // Assert
            Assert.IsFalse(ServiceLocator.IsInitialized, "ServiceLocator should not initialize with null title ID");
            LogAssert.Expect(LogType.Error, "PlayFab Title ID cannot be null or empty.");
        }
        
        [Test]
        public void Initialize_CalledTwice_ShowsWarning()
        {
            // Act
            ServiceLocator.Initialize(TEST_TITLE_ID);
            ServiceLocator.Initialize(TEST_TITLE_ID); // Second call
            
            // Assert
            Assert.IsTrue(ServiceLocator.IsInitialized, "ServiceLocator should remain initialized");
            LogAssert.Expect(LogType.Warning, "ServiceLocator is already initialized. Skipping duplicate initialization.");
        }
        
        [Test]
        public void Reset_AfterInitialization_ClearsServices()
        {
            // Arrange
            ServiceLocator.Initialize(TEST_TITLE_ID);
            Assert.IsTrue(ServiceLocator.IsInitialized, "Precondition: ServiceLocator should be initialized");
            
            // Act
            ServiceLocator.Reset();
            
            // Assert
            Assert.IsFalse(ServiceLocator.IsInitialized, "ServiceLocator should not be initialized after reset");
            Assert.IsNull(ServiceLocator.AuthService, "AuthService should be null after reset");
            Assert.IsNull(ServiceLocator.FileUploadService, "FileUploadService should be null after reset");
        }
        
        [UnityTest]
        public IEnumerator ToolkitBootstrap_InitializesServicesOnAwake()
        {
            // Arrange
            GameObject testObject = new GameObject("TestBootstrap");

            UnityEngine.TestTools.LogAssert.Expect(LogType.Error, "PlayFab Title ID is not set in ToolkitBootstrap. Please configure it in the inspector.");

            var bootstrap = testObject.AddComponent<ToolkitBootstrap>();
            
            // Wait one frame for Awake to complete
            yield return null;
            
            // Act - Use the test method to initialize with a test title ID
            bootstrap.InitializeWithTitleId(TEST_TITLE_ID);
            
            // Wait another frame to ensure initialization completes
            yield return null;
            
            // Assert
            Assert.IsTrue(ServiceLocator.IsInitialized, "ServiceLocator should be initialized by ToolkitBootstrap");
            Assert.IsNotNull(ServiceLocator.AuthService, "AuthService should be initialized by ToolkitBootstrap");
            
            // Cleanup
            Object.DestroyImmediate(testObject);
        }
        
        [Test]
        public void ToolkitBootstrap_ManualInitialization_WorksCorrectly()
        {
            // Test the ToolkitBootstrap's re-initialization functionality
            
            // Arrange
            GameObject testObject = new GameObject("TestBootstrap");

            UnityEngine.TestTools.LogAssert.Expect(LogType.Error, "PlayFab Title ID is not set in ToolkitBootstrap. Please configure it in the inspector.");

            var bootstrap = testObject.AddComponent<ToolkitBootstrap>();
            
            // Act
            bootstrap.InitializeWithTitleId(TEST_TITLE_ID);
            
            // Assert
            Assert.IsTrue(ServiceLocator.IsInitialized, "ServiceLocator should be initialized after manual initialization");
            Assert.IsNotNull(ServiceLocator.AuthService, "AuthService should be available after initialization");
            
            // Cleanup
            Object.DestroyImmediate(testObject);
        }
        
        [Test]
        public void ServiceLocator_ServicesImplementCorrectInterfaces()
        {
            // Act
            ServiceLocator.Initialize(TEST_TITLE_ID);
            
            // Assert
            Assert.That(ServiceLocator.AuthService, Is.InstanceOf<PlayFabToolkit.Interfaces.IAuthService>(), 
                "AuthService should implement IAuthService interface");
            Assert.That(ServiceLocator.FileUploadService, Is.InstanceOf<PlayFabToolkit.Interfaces.IFileUploadService>(), 
                "FileUploadService should implement IFileUploadService interface");
        }
    }
}
<<<<<<< Updated upstream
# playfabtoolkit
=======
# PlayFab Toolkit

A headless, service‑oriented wrapper around the PlayFab Unity SDK that standardises authentication, player‑data access, and file upload workflows across projects.

> **Version 0.1.0 (Preview)** – API surface is still stabilising.

---

## ✨ Features

| Area                          | What you get                                                                                                        |
| ----------------------------- | ------------------------------------------------------------------------------------------------------------------- |
| **AuthService**               | One‑line login, registration, logout, password‑reset & entity‑token retrieval (email / password and Custom ID).     |
| **FileUploadService**         | Upload/download/delete player‑files via PlayFab Data API with automatic multipart handling & async/await callbacks. |
| **Interfaces & Abstractions** | Clean `IAuthService`, `IFileUploadService`, etc. so gameplay code never touches the raw SDK.                        |
| **Utilities**                 | `CoroutineRunner` for running coroutines from non‑MonoBehaviours.                                                   |
| **Transform Logger sample**   | Shows real‑world usage: logs object transforms, then uploads a CSV/JSON log to the logged‑in player profile.        |

---

## 📋 Requirements

* **Unity 2022.3 LTS** or newer
* **PlayFab Unity SDK ≥ 3.5.0** (installed as a UPM package – see below)

---

## 🔧 Installation

### 1 – Install PlayFab SDK as a package (one‑time)

```text
Window ▶ Package Manager ▶ + ▶ Add package from Git URL…
https://github.com/PlayFab/UnitySDK.git#upm
```

> Or embed your own `com.playfab.unitysdk` folder in *Packages/*.
> Make sure there is **no copy** of the SDK under *Assets/*.

### 2 – Install PlayFab Toolkit

*Option A – Git URL*

```text
+ ▶ Add package from Git URL…
https://github.com/CoelorLabs/PlayFabToolkit.git#0.1.0
```

*Option B – Local folder*

```text
+ ▶ Add package from disk…
…/Packages/com.coelor.playfabtoolkit/package.json
```

Unity will compile the `PlayFabToolkit` assembly and list the package under **In Project**.

---

## ⚡ Quick Start

### Option A: Using ToolkitBootstrap (Recommended)

1. **Add the Bootstrap Component**
   - Create an empty GameObject in your startup scene
   - Add the `ToolkitBootstrap` component
   - Set your **PlayFab Title ID** in the inspector
   - The toolkit will auto-initialize when the scene loads

2. **Use the Services**
```csharp
using PlayFabToolkit.Services;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public void OnLoginButtonClicked()
    {
        // Services are ready to use after bootstrap
        ServiceLocator.AuthService.LoginWithEmail(
            email: "player@example.com",
            password: "abc123",
            onComplete: (success, msg, displayName) =>
            {
                Debug.Log(success ? $"Welcome {displayName}!" : $"Login failed: {msg}");
                
                if (success)
                {
                    // After login, get entity token for file uploads
                    ServiceLocator.AuthService.GetEntityToken(
                        (entityId, entityType) => {
                            if (ServiceLocator.FileUploadService is FileUploadService fileService)
                            {
                                fileService.SetEntity(entityId, entityType);
                                Debug.Log("File upload service ready!");
                            }
                        },
                        error => Debug.LogError($"Entity token error: {error}")
                    );
                }
            });
    }
    
    public void UploadSampleFile()
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes("Hello PlayFab");
        ServiceLocator.FileUploadService.UploadFile(
            data,
            fileName: "hello.txt",
            contentType: "text/plain",
            callback: (ok, err) => Debug.Log(ok ? "Uploaded!" : $"Error: {err}"));
    }
}
```

### Option B: Manual Initialization

```csharp
using PlayFabToolkit.Services;

// Call once at application startup
ServiceLocator.Initialize("YOUR_PLAYFAB_TITLE_ID");

// Then use services anywhere in your code
ServiceLocator.AuthService.LoginWithCustomId(
    customId: "player123",
    onComplete: (success, msg) => Debug.Log(msg));
```

### Important Notes

- **Entity Token**: File uploads require an entity token. Call `AuthService.GetEntityToken()` after successful login and use `SetEntity()` on the file service.
- **Initialization**: Only call `ServiceLocator.Initialize()` once. The ToolkitBootstrap handles this automatically.
- **Scene Persistence**: The ToolkitBootstrap can persist across scenes to maintain your services.


---

## 🎮 Samples

Open **Window ▶ Package Manager ▶ PlayFab Toolkit ▶ Samples ► Import** to add the *Transform Logger* demo scene to your project:

```
Samples~/Transform Logger Sample/
   SampleScene.unity
   Sample README.md
```

Press ▶ Play, log in with a Custom ID, move around, then hit **Upload Log** – a JSON/CSV file appears in your PlayFab account’s File section.

---

## 📂 Folder Structure

```
Runtime/
├── Interfaces/
├── Services/
├── Utils/
Editor/
Tests/ (optional)
Samples~/ (optional demos)
```

---

## 🚧 Roadmap

* 0.2 – LeaderboardService, CloudScriptService
* 0.3 – Editor validation window & PlayFab settings checker
* 1.0 – Stable API, documentation site

---

## 📑 Changelog

See **CHANGELOG.md**.

---

## 📝 License

MIT © 2025 Gary Brenden (Coelor)
>>>>>>> Stashed changes

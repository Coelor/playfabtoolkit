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

## ⚡ Quick Start

```csharp
using PlayFabToolkit.Interfaces;
using PlayFabToolkit.Services;

// 1 – Init once at startup
ServiceLocator.Initialize("YOUR_PLAYFAB_TITLE_ID");

// 2 – Log in (e.g. UI button)
ServiceLocator.AuthService.LoginWithEmail(
    email: "player@example.com",
    password: "abc123",
    onComplete: (success, msg, displayName) =>
    {
        Debug.Log(msg);
    });

// 3 – Upload a file
byte[] data = System.Text.Encoding.UTF8.GetBytes("Hello PlayFab");
ServiceLocator.FileUploadService.UploadFile(
    data,
    fileName: "hello.txt",
    contentType: "text/plain",
    callback: (ok, err) => Debug.Log(ok ? "Uploaded" : err));
```

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

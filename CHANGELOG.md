# Changelog

All notable changes to **PlayFab Toolkit** will be documented in this file.

This project adheres to [Semantic Versioning](https://semver.org/) and follows the [Keep a Changelog](https://keepachangelog.com/) conventions.

---

## \[Unreleased]

### Added- Automated unit‑test scaffold under `Tests/Runtime`.

### Changed

* Docs: expanded README quick‑start and sample instructions.

### Fixed

* *TBD*

---

## \[0.1.0] – 2025‑05‑28

### Added

* **AuthService** supporting email/password & Custom ID login, registration, logout, password reset.
* **FileUploadService** for PlayFab Data API file uploads with async callback.
* **Interfaces** (`IAuthService`, `IFileUploadService`) decoupling gameplay code from PlayFab SDK.
* **CoroutineRunner** utility for running coroutines from non‑MonoBehaviours.
* **TransformLogger** sample demonstrating toolkit integration and file upload.
* Package structure with Runtime & Editor asmdefs (`PlayFabToolkit`, `PlayFabToolkit.Editor`).
* UPM‑compliant `package.json`, initial README, and sample scene under `Samples~/`.

### Changed

* N/A – first public preview.

### Fixed

* N/A – first public preview.

---

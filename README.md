# EOCS (Engine on CSharp)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET Version](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/)
[![OpenTK](https://img.shields.io/badge/OpenTK-4.9.4-green)](https://opentk.net/)
[![Windows](https://img.shields.io/badge/Windows-10%2B-0078D6)](https://www.microsoft.com/windows)

> A simple, open-source 3D game engine built from scratch in C# using OpenTK.
> **[Wiki of this project](https://github.com/Xwared-Team/Engine-on-CSharp/wiki)**

Welcome to **EOCS**! This is a personal project aimed at creating a flexible and modular 3D game engine. The entire codebase is open-source, meaning you are free to download, modify, experiment with, and build the project yourself.

## Features

*   **Custom Rendering Pipeline:** Built on top of OpenGL 3.3 Core Profile via OpenTK.
*   **Modular Architecture:** Separation of concerns (Camera, Shader, Mesh, GameObject).
*   **Asset Management:** Basic loading for OBJ models and PNG textures.
*   **Skybox Support:** Environment rendering for immersive scenes.
*   **MIT License:** Free to use and modify with attribution.

*(Note: This project is in early development. Features like Lua scripting, ECS, and Vulkan support are planned for future versions.)*

## Tech Stack

*   **Language:** C# (.NET 10.0)
*   **Graphics API:** OpenGL 3.3 (via OpenTK 4.9.4)
*   **IDE:** Visual Studio Code / Visual Studio 2022

## Installation & Build

### Prerequisites
*   [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later.
*   Graphics drivers supporting OpenGL 3.3+.

### Steps
1.  Clone the repository:
    ```bash
    git clone https://github.com/dovintc-off/Engine-on-CSharp.git
    cd Engine-on-CSharp
    ```

2.  Restore dependencies (optional, `dotnet run` does this automatically):
    ```bash
    dotnet restore
    ```

3.  Build and Run:
    ```bash
    dotnet run
    ```

## How You Can Help

1.  **Report Bugs:** If you find a crash or unexpected behavior, please open an [Issue](https://github.com/dovintc-off/Engine-on-CSharp/issues).
2.  **Suggest Features:** Have an idea for a new rendering feature or UI improvement? Let’s discuss it in Discussions or Issues.
3.  **Pull Requests:** Feel free to fork the repo, fix bugs, or add features, and submit a PR. Please follow the coding style of the project.

## License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

## Author

**Dov1ntc**  
[GitHub Profile](https://github.com/dovintc-off)

---
*If you like this project, consider giving it a ⭐ on GitHub!*

# Changelog
# All changes in project EOCS
# Project create by Dov1ntc!

## [0.2.0] - 2026-04-24
### Added
- Engine architecture: The basic structure of the engine has been implemented, divided into “Core” (Main.cs) and “Game Content” (GameBase.cs / Script.cs).
- Scene System: Added abstract class BaseGame with lifecycle methods Load(), Update() and Draw().
- Modular camera: The camera logic has been moved to a separate class Camera.cs with support for WASD, mouse and zoom control (FOV 30°–110°).
- Global imports: Implemented the GlobalUsings.cs file to automatically include the main OpenTK libraries and engine modules throughout the project.
- Entities (GameObject): The GameObject class was created to encapsulate meshes, shaders and transformations (position, scale, color), which simplifies working with the scene.

### Changed
- Main.cs refactoring: Hard-coded game logic (teapot, skybox) has been completely removed from the main window class. Now Main acts only as a container and render manager.
- Input control: The keyboard and mouse processing logic has been moved inside the Camera class and the Update method of the user's game.
- Code optimization: Removed unnecessary using from files thanks to global imports. Namespace name conflicts have been fixed (the EOCS.Camera namespace has been renamed/managed through aliases).

### Fixed
- Bug with FOV: Fixed the bug of “breaking” the viewing angle when zooming with the mouse wheel (added the correct Clamp range from 30 to 110 degrees).
- Bug CS1612: Fixed problem with changing coordinates of Vector3 structures in the GameObject class (now full vector assignment is used).
- Bug CS0118: Resolved conflict between namespace name and Camera class name.

### Tech Debt / Known Issues
- Light is still transmitted as hardcode to the shader. The plans include creating a Light class and a lighting manager.
- Collisions have not yet been implemented.
- There is no resource loading system with path checking (if the file is not there, the game may crash).

## [0.1.1] - 2026.04.21

### Changed
- Improved readability of the debug menu (in code)

## [TD-0.1.1] - 2026.04.21

### Add
- Dynamic Text Color: Implemented support for changing text color via Vector3 parameter in DrawString.
- Invariant Culture Formatting: Fixed number formatting in debug overlay to always use dots (`.`) instead of commas (`,`) regardless of system locale.
- Created dedicated branch text-development for experimental text features.

### Changed
- Improved readability of the debug menu (in code)

## [0.1.0] - 2026.04.21 (Anniversary!!!)

### Changed
- The menu with additional information opens only when you press f3
- A menu with additional information displays the position and rotation of the camera
- Now each file is in its own namespace (render/Mesh.cs -> namespace EOCS.render)

### Added
- New classes make it easier to create more than one object

## [0.0.7] - 2026.04.21

### Added
- F11 toggles fullscreen/windowed mode.
- Default window state is now windowed.

### Changed
- Moved game logic (input, camera, matrices) to OnUpdateFrame.
- Separated update logic from rendering code.

## [0.0.6] - 2026.04.18

### Fixed
- Fixed lighting visibility issue caused by incorrect render order and uniform binding sequence.

## [0.0.5] - 2026.04.17

### Added
- Text Render (Padding, Resizing, Coordinates)
- [Vertex Shader](Assets/shaders/shader.vert) and [Fragment Shader](Assets/shaders/shader.frag) from text

## [0.0.4] - 2026.04.17

### Added
- Generation Text Atlas
- Text rendering base class. (GlyphData)

## [0.0.3] - 2026.04.17

### Changed
- Renamed project files and directory from **3dGame** to **EOCS**
- Updated root namespace from **BPX** to **EOCS**
- README.md

### Added
- LICENSE
- CHANGELOG.md

### Removed
- Debug Files (debug_atlas_test.png)
# Changelog
# All changes in project EOCS
# Project create by Dov1ntc!

## [0.1.1] - 2026.04.21

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
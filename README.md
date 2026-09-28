# Terrain Snapper Tools

[English](README.md) | [简体中文](README_CN.md)

A simple and practical set of Unity Editor tools for snapping objects to the Terrain surface.

These tools let you quickly align selected GameObjects or Spline Knots to the nearest Terrain, so you no longer have to hand-tune the Y position of every object placed on uneven ground.

## ✨ Features

### GameObject Terrain Snapper

Batch-snap selected GameObjects to the nearest Terrain.

- Select multiple GameObjects at once
- Snaps each object's Transform position to the Terrain surface height
- Only the Y position changes — X and Z are preserved
- Automatically picks the nearest Terrain when multiple Terrain tiles exist
- Supports Unity Undo
- Reports the number of snapped GameObjects in the Console

### Spline Terrain Snapper

Batch-snap Spline Knots to the nearest Terrain.

- Select one or more Spline Containers at once
- Snaps every Knot of every Spline inside the Container to the Terrain surface
- Only the local Y position changes — X and Z are preserved
- Automatically picks the nearest Terrain when multiple Terrain tiles exist
- Rounds the snapped local Y to 2 decimal places
- Supports Unity Undo
- Reports the number of snapped Knots and processed Containers in the Console

## 📸 Preview

*Tools Menu*  
![Tools Menu](Preview/ToolsMenu.png)

## 📦 Installation

Place the scripts inside an `Editor` folder in your Unity project:

```text
Assets/
└── Editor/
    └── TerrainSnapperTools/
        ├── GameObjectTerrainSnapper.cs
        └── SplineTerrainSnapper.cs
```

`GameObjectTerrainSnapper` has no additional dependencies. `SplineTerrainSnapper` requires the **Unity Splines package** (`com.unity.splines`), which you can install via **Window > Package Manager**.

After importing the scripts, the tools will appear in the Unity Editor menu:

```text
Tools
└── Terrain Snapper Tools
    ├── GameObject Terrain Snapper
    └── Spline Terrain Snapper
```

## 🕹️ Usage

1. Open the tool from the **Tools > Terrain Snapper Tools** menu.
2. Select one or more GameObjects (or Spline Containers) in the **Hierarchy**.
3. Make sure at least one Terrain exists in the scene.
4. Click the snap button in the tool window.

## ↩️ Undo Support

Both tools support Unity's Undo system.

Snapped GameObjects and Spline Knots can be restored using `Ctrl + Z` or **Edit > Undo**.

## 🎯 Use Cases

Terrain Snapper Tools can be useful for:

- Level Design
- Environment Setup
- Scene Organization
- Placing props, foliage, or decals on uneven terrain
- Aligning roads, rivers, or fences authored with Splines to the ground
- Quickly correcting objects that float above or sink below the Terrain
- Reducing repetitive manual Transform editing

## 📋 Requirements

- Unity 2022.3 or later
- Unity Editor
- At least one Terrain in the scene
- Unity Splines package (`com.unity.splines`) — required only for Spline Terrain Snapper
- Editor-only tools

## ⚠️ Notes

- These tools are intended for Unity Editor workflows and should be placed inside an `Editor` folder.
- The tools operate on the currently selected GameObjects in the Unity Hierarchy.
- Height is sampled from the Terrain heightmap, so only the Y position is modified; X and Z are left untouched.
- When a selected object or Knot is not located inside any Terrain bounds, the nearest Terrain tile is used.
- No runtime components are required.

## 📄 License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

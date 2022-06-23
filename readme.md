# Readme
## Final Project for the *Computer Animation* course on the *Technical University Vienna*

by *Florian Holzmann*

<br>

## General
---

The Project is created using Blender and Unity.

>Blender Version 3.0.0
>
>Unity Version 2020.3.25f1

It was implemented and tested on Windows 10 and Manjaro Linux.

### Files

> - BlenderFiles &emsp;&emsp;// used Blender Files and exported FBX Files
> - UnityProject &emsp;&emsp;// the Unity Project
> - Builds &emsp;&emsp;// Win64 builds of the animations
> - story-board.pdf &emsp;&emsp;// the Storyboard of the Animation

## Assets
---
Cat (Gizmo) - provided by course

House - by **jassjadvani98** *(modified)* - <https://www.cgtrader.com/free-3d-models/exterior/house/house-flat-floor-plan-non-texture-version>

Road - by **SorianModels** - <https://www.cgtrader.com/free-3d-models/vehicle/other/roads-by-sorianmodels>

Trees - by **Nik-Y** - <https://www.cgtrader.com/free-3d-models/plant/other/forest-assets-low-poly-including-bonus-animal>

Fish - by **rkuhl** - <https://www.cgtrader.com/free-3d-models/animals/fish/low-poly-fish-b981402c-4bac-491b-a4d8-6bc91b8e08b0>

Music - by **Musictown** - <https://pixabay.com/de/music/schone-stucke-light-and-sweet-piano-orchestra-1-22210/>

Door Sound - by **rivernile7** *(modified)* - <https://freesound.org/people/rivernile7/sounds/234244/>

Meow Sound - by **fthgurdy** *(modified)* - <https://freesound.org/people/fthgurdy/sounds/528193/>


## Unity Project
---

### Assets Structure

>  - Assets
>     - Models &emsp;&emsp;// Models in Subfolders as FBX files
>         - ..
>         - ...
>     - Prefabs &emsp;&emsp;// Prefab GameObjects to be used in Animation
>     - Scenes
>     - Scripts
>         - Animations &emsp;&emsp;// Unity Scripts for all animations
>         - Flock &emsp;&emsp;// Unity Scripts for Flock and Boids
>         - Spline &emsp;&emsp;// Catmull-Rom-Spline classes
>         - Util
>     - Settings &emsp;&emsp;// includes Unity Animators and InputController
>     - Shaders &emsp;&emsp;// Circular-Wipe Shader and Controller
>     - Sounds &emsp;&emsp;// Music and Sound Effects

### Hierachy Structure

>- AnimationScene
>   - AnimationCamera &emsp;&emsp;// the Camera used for the Animation
>   - Directional Light
>   - Animation_Scene
>       - Static_Scene &emsp;&emsp;// static Models (the House)
>       - Animation_Timeline &emsp;&emsp;// all Path-Animations (Root Node, children build up the animation tree)
>       - Model_Animations &emsp;&emsp;// Character-Animations, Sound-Animations, Circular-Fade-Out/-In
>       - Models &emsp;&emsp;// animated Models
>           - Aquarium &emsp;&emsp;// also includes the Flocking-System
>           - ...

### Path-Animations

All Path-Animations have the **CatmullRomSplineScript**, which defines the parameters of the path animation.

#### Parameter: 
> - Resolution of the Spline
> - If the Rotation should start smooth
> - Start delay 
> - Duration of the Animation
> - Stiffness of Rotations
> - The animated Target
> - If the Camera Target should be used
> - Option to set a external Camera Target
> - A List of Animations, which should run parallel with the animation
> - A List of Animations, which should start after this snimation ended

#### Hierachy Structure
> - Catmull-Rom-Spline-Animation
>   - Points &emsp;&emsp;// the Point GameObjects used to generate the Spline
>   - Path &emsp;&emsp;// the Path segments generated (only used for debugging)
>   - CameraTarget &emsp;&emsp;// the Camera Target GameObject

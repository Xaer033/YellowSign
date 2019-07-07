Decal Master

System contains two main objects:
DeferredDecalsSystem component and Decal component

DeferredDecalsSystem - main component that must be on scene as instance (singletone)
Decal - component to register decal in rendering pathes

Quick start:
0. Drag and drop Knife/Decal Master/Prefabs/DecalsSystem prefab to scene
1. Create plane - GameObject/3D Object/Plane
2. Create decal - GameObject/3D Object/Decal
3. Assign any material with shader Knife/Decals/PBR to Material property in inspector

Sample scene - Knife/Decal Master/Samples/Scenes/Samples

Knife/Decals/PBR shader parameters:
- Color - color of the decal (multiplies to diffuse map color)
- Diffuse - color map of decal
- Normals - normal map of decal
- Normal Scale - power of normal map value
- Specular - specular smoothness map (RGB - specular, A - smoothness)
- EmissionColor - color of emission (multiplies on emission map color)
- Emission - emission color map of the decal
- Smoothness - specular smoothness multiplier
- Blend Normals - this value controls normals blending of decal normals with scene normals
- Terrain decal - this toggle provide terrain only decal blending
- Clip by normals - this toggle controls to clip decal by scene normals or alpha blend
- Normal Edge Blending - this toggle controls normal blending by radius of decal
- Normal Mask - this toggle controls normal blending by diffuse alpha value
- Clip normals - threshold decal clipping by scene normal
- Terrain height clip - threshold decal clipping by terrains height
- Terrain height clip power - threshold power

All decals can rendered with instancing (just enable GPU instancing in shader)

Decal component parameters:
- Material - decal material
- Sorting order - decal rendering order (different sorting orders breaks gpu instancing)
- Instanced color - decal instanced color (affects only if gpu instancing enabled in material)
- NeedDrawGizmos - draw selection gizmos of decal

Decal automatically register in rendering pipe on enable, and remove from rendering pipe on disable

Decal Placement Tool
You can use decal placement tool to place decals on scene very fast. You don't need colliders to place decals on surfaces.

Open:
Window/Knife/Decal Placement Tool

At top you can see 3 buttons
1 - Simple placing (one click - one decal will be placed)
2 - Burst placing (one click - many decals will be placed randomly in circle)
3 - Painting (left mouse button holding - paint many decals in minimal distance)

At middle:
- Position Jitter - position randomness
- Rotation Jitter - rotation randomness
- Size Jitter - size randomness
- Project Offset - distance threshold to except z-fighting or projection clipping
- Min Painting Distance - painting mode parameter, no decals will be placed in distance that lower than that value
- Sorting Order - sorting order of currently placing decals
- Parent To Hitted Renderer - parent decals to hitted renderer
- Rotate To Next - painting mode parameter, will rotate forward axis of decal to next placing decal
- Size - 3D scale value of decal
- Rotation - rotation offset
- Scale - scale multiplier
- Burst size - burst mode parameter, controls size of burst in screen space (blue circle in burst mode)
- Burst count - burst mode parameter, how much decals will be spawned in burst shot

At bottom you can 3 buttons (Load, Save, Clear) and add decal template button

Add Decal Template button - provide you add decal material to current templates list
Load button - provide you load preset of templates to current templates list
Save button - provide you save selected decals to preset file
Clear button - clears current templates list

Quick start with Decal Placement Tool:
0. Drag and drop Knife/Decal Master/Prefabs/DecalsSystem prefab to scene
1. Open Decal Placement Tool window
2. Create new material in project window
3. Set Knife/Decals/PBR shader to created material
4. Drag and drop created material to Decal Placement Tool window OR click to add decal template button and select created material
5. Click to new decal template in Decal Placement Tool window it will be highlighted by blue color
6. Select first mode (Simple placing) and scene view will be highlighted by thin blue frame on borders of window
7. Move mouse to scene view and click on any renderer to place decals
8. You can add more decals to decal placement tool and select them all
Click - clear selection and select clicked decal template
Shift + Click - select all decal templates beetween first selected and current selected
Ctrl (or Cmd) + click - add clicked decal template to selection

To enable terrain blending:
0. Select DecalsSystem object on scene and choose your setup on TerrainDecals property:
- One Terrain - only one terrain on scene
- Multi Terrains - two or more terrains on scene
1. Right mose click on component title (or click to gear)
2. Click copy terrains heightmaps
3. Enable terrain decal parameters on your decals materials

Terrain blending sample scene - Knife/Decal Master/Samples/Scenes/Terrain
- Open scene and move to cubes on scene (you can find in hierarchy and double click to one)
- You can see that decal is blending on objects that more higher that terrain by world Position

Video Tutorial https://www.youtube.com/watch?v=x8vEQaMj01M

Support: knifeent@gmail.com
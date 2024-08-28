
Presents 3D Pack
----------------

Colorful animated and interactive loot box presents.
Drag and drop present prefabs into your project and press Play!
Demo scene included.
PC and mobile friendly.

Easter, Christmas, Valentine, Halloween and Stripes themed.
Animated present boxes with 3 different shapes and bows.
123 present prefabs, 41 paper and 29 ribbon PBR materials.
13 particle effects prefabs.
Scripted playback for animation, player interactions, particles and contained items.

Supports Unity 2018.4 Standard.
No external package required.


Demo
----

A demo scene with animated and interactive presents, animated camera, a 3d coin (inside a present), a ground model covered in snow and a Christmas themed sky-box are all included.


Present Boxes
-------------

There are 3 boxes included : square, rectangular and cylinder shaped.
There are also 3 bow decorations for the top of the box, those are separate meshes and can easily be swapped.
Each present prefabs is assigned materials based on different themes.


Present Animator Script
-----------------------

The “Present animator” script is used to handle each present’s interaction, animation, particles and contained items.

The following interactions can be toggled on/off for each present:

 - “Mouse-over”
 - “Click” (or Tap)
 - “Close back”

The default “Idle” state cannot be turned off.

Each state can be assigned an animation and multiple particle effects “FX”.
FX can be set to pre-load (instantiated on Start), looped and they can be played after a delay.

The Close back interaction can be set as auto with a delay (the present will close on its own after the delay) or by click/tap.

Multiple “Contained items” can be added to the present, those will be revealed when the present is opened (by using the “Open” or “Explode” animations) Contained items are set to pre-load by default.

Finally, public callbacks are available for the Idle, Mouse over, Opening and Closing states.
This means that your own methods can be called on a specific present's state.
Please see the CallbackShowcase.cs script (assigned to "Present_B_13" in the demo scene) for an example on how to use this.


Animations
----------

Each present comes with the following animations:

 - Idle – no motion
 - Breath – the present softly deforms mimicking a breathing motion
 - Jump – bounces in place
 - Open – pops open instantly
 - Explode – the box shakes rapidly then suddenly opens


Triangle count
--------------

 - Present A – square box – 726 triangles
 - Present B – rectangular box – 658 triangles
 - Present C – cylinder box – 1360 triangles
 - Bow A – 720 triangles
 - Bow B – 372 triangles
 - Bow C – 660 triangles


Materials
---------

Materials are all Unity’s Standard PBR material (Metallic/Roughness).
The paper and the ribbon/bow are using separate materials which allow to combine any paper with any ribbon/bow material.


Textures
--------

Papper and ribbon's textures are all 1024*1024 pixels and tile-able.
FX textures range from 512*512 to 1024*1024 and 512*2048 pixels.
The ground snow textures are 128*128 pixels and tile-able.
The sky-box texture is 5460*2730 pixels and tile-able in one direction.


Particle Effects
----------------

The “Confetti Glitter” and “Confetti Streamer” particle effects are both using animated textures.
The “Explosion” and “Sparkles” are both standard particle effects.

There are 13 particle effects prefabs:
 - FX_Absorb_Glitter
 - FX_Absorb_Sparkle
 - FX_Absorb_Streamer
 - FX_Explode
 - FX_Explode_Glitter
 - FX_Explode_Sparkle
 - FX_Explode_Streamer
 - FX_Fall_Glitter
 - FX_Fall_Sparkle
 - FX_Fall_Streamer
 - FX_Float_Glitter
 - FX_Float_Sparkle
 - FX_Float_Streamer


Support
-------

Please contact us on our site https://www.axion-studios.com or directly at support@axion-studios.com
Come chat with us and the community at https://discord.gg/KYFgfXa


Social Media
------------

https://twitter.com/axionstudiosltd
https://www.instagram.com/axionstudiosltd/
https://www.facebook.com/AxionStudiosLimited
https://www.youtube.com/channel/UCUBw-eG59-R-UbFCOM86xbw


Thank you
---------

Thank you for purchasing our product. We really hope you love it!
If you do, please consider posting a review online, this helps us to continue providing great products.
Thank you in advance.


Version history
---------------

v1.5
- Added Easter themed textures, materials and prefabs

v1.4
- Added Halloween themed textures, materials and prefabs
- Fixed some UV distortion on some of the present models

v1.3
- Added callbacks to the Present Animator

v1.2
- Updated all 3d mesh and UV to allow texture tiling
- Created all textures and materials from scratch
- Created a new Present Animator script
- Present shape (cylinder) and bow added
- Materials and textures (stripes) added
- Support for interactions added
- Support for contained items added
- Removed Substance materials

v1.1
- Added patterns for Valentine’s day
- Renamed substance materials to generic names
- Added zipped material themes presets for Christmas and Valentine
- The Valentine’s materials are loaded by default
- Fixed a minor bug to position the particle effect before it starts playing
- Changed the particles to simulate in Local space

v1.0
- Initial release
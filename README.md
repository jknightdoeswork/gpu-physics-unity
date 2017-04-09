A GPU Accelerated Voxel Physics Solver for Unity

Read more at
[http://www.00jknight.com/blog/gpu-accelerated-voxel-physics-solver](http://www.00jknight.com/blog/gpu-accelerated-voxel-physics-solver)

64,000 cubes
![gif](https://thumbs.gfycat.com/MammothInfantileDunlin-size_restricted.gif "Gif")

1024 cubes
![gif](https://fat.gfycat.com/JovialKnobbyBrahmanbull.gif "Gif")

A Unity Command Buffer is used by GPUPhysics.cs to dispatch the compute and render shaders. 

This has been designed such that no per voxel data is transferred between the GPU and the CPU at runtime.

Speed will likely by further through research and optimization of the per particle Kernels within the Compute Shader.

LICENSE

You can use this software in a commercial game, but you cannot sell this software on the Unity Asset Store or any other platform that sells software tools for developers.


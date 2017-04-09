A GPU Accelerated Voxel Physics Solver for Unity

Read more at

[http://www.00jknight.com/blog/gpu-accelerated-voxel-physics-solver](http://www.00jknight.com/blog/gpu-accelerated-voxel-physics-solver)

![gif](https://thumbs.gfycat.com/MammothInfantileDunlin-size_restricted.gif "Gif")

64,000 cubes

![gif](https://fat.gfycat.com/JovialKnobbyBrahmanbull.gif "Gif")

1024 cubes

A Unity Command Buffer is used by GPUPhysics.cs to dispatch the compute and render shaders. 

This has been designed such that no per voxel data is transferred between the GPU and the CPU at runtime.

Speed will likely by further through research and optimization of the per particle Kernels within the Compute Shader.

LICENSE

You can use this software in a commercial game, but you cannot sell this software on the Unity Asset Store or any other platform that sells software tools for developers.


Further Improvements

Build the Voxel Grid around the bounds of the simulation dynamically
- auto apply the "renderer bounds" and the "gridDimensions"
- does "renderer bounds" even do anything? - looks like not

Eliminate the Voxel Grid Clear Step

Establish the pattern for collision with solid objects

Find out why the Damping force and Tangential forces described in Takahiro Harada's
system do not seem to have good effects

Determine if a better shadow pass can be constructed to speed up shadows

Find out how to reliably render through CommandBuffer.DrawMeshInstancedIndirect (Unity bugs?)

Optimize the thread grouping


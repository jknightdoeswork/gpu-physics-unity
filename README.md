A GPU Accelerated Voxel Physics Solver for Unity

[gif](https://media.giphy.com/media/xUA7aRaZxX1wphJcME/giphy.gif)

A Unity Command Buffer is used by GPUPhysics.cs to dispatch the compute and render shaders. 

This has been designed such that no per voxel data is transferred between the GPU and the CPU at runtime.

Speed will likely by further through research and optimization of the per particle Kernels within the Compute Shader.

Read more at

[http://www.00jknight.com/blog/gpu-accelerated-voxel-physics-solver](http://www.00jknight.com/blog/gpu-accelerated-voxel-physics-solver)


LICENSE

You can use this software in a commercial game, but you cannot sell this software on the Unity Asset Store or any other platform that sells software tools for developers.
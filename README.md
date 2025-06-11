# FlockingExperiment

![Fish Simulation Banner](https://github.com/user-attachments/assets/7314012f-1633-4734-b688-28147bc7d194)


A fish flocking simulation based on Craig Reynolds BOIDS algorithm implemented using Unity's Jobs system and the Burst compiler.
Because of how beautiful flocking looks in real life I wanted to recreate the behaviour in Unity. 

I started by studying Craig Reynolds site (https://www.red3d.com/cwr/boids/) and programmed a first naive iteration.
I implemented the 3 rules of the boids algorithm (Alignment, Cohesion, Separation) using simple Vector math. 
Then I modified the rules slightly so the behaviour mimiced fish flocking behaviour more closely than bird flocking. 
This implementation was not scalable due to the heavy calculations blocking the main thread because each flockmember ran its own calculations.
For more than 100 fish it ran at about 30 FPS. 

![Fish 01](https://github.com/user-attachments/assets/2853cbd8-9b97-4607-9e97-0e029f52c04b)

I then searched the web how I could improve performance in neighbour searching/avoidance calculations and came across the spatial partitioning technique in a graphics programming thread. 
A user explained the Octree in spatial partioning as following: 
"Octrees usually refer to a type of a space-partitioning tree where every node has the same box shape (often just cube) and non-leaf nodes are evenly split into 8 children along axis-aligned planes. 
This means that they basically model an adaptive 3D grid, enabling faster spatial queries like nearest neighbors and ray tracing." 
Spatial partitioning was a new concept for me and was quite interesting to experiment with, it was very cool to see the 3D grid (with OnDrawGizmos) subdividing and unsubdividing itself when fishes entered and exited a cell.
Using a basic octree system the simulation got a performance boost. It could run the flocking behaviour at around 70-90 FPS for a maximum of 1000 fish. It wasnt performant enough to simulate 1000's of fishes so I looked for other optimisations.  

![Octree](https://github.com/user-attachments/assets/aa9a11d0-49da-469f-997e-217f3a37a8c6)

It dawned on me that drawing the fish meshes itself is computational very heavy and I looked into compute shaders which could make it possible to render all the meshes in a single drawcall. 
Using the URP Cookbook samples as a guide the simulation ran the flocking behaviour with tens of thousands of fish. I learned about using Graphics.RenderMeshInstanced (https://docs.unity3d.com/6000.1/Documentation/ScriptReference/Graphics.RenderMeshInstanced.html)
that renders multiple instances of a mesh using GPU instancing which greatly reduces drawcalls. One downside was that a compute shader is hard to debug and this made adjusting behaviour quite difficult. 
Because of time restraints I decided to move forward and revisit learning about compute shaders at a better suitable time.  

I found an alternative strategy to run the simulation with a large amount of fish which was using the Job system. It took a little time to familiarize myself with the Mathematics library.
I split the behaviour up in 2 jobs:

1.Update Behaviour => calculates the new position and direction of each fish using alignment, cohesion, and separation rules.
2.InterpolationJob => smooths out motion between simulation ticks for stable visual output at high FPS (stutters were very noticable when moving the camera).

By combining Unity's Jobs system and burst compiler with gpu-instancing allowed the simulation to run a maximum of 10000 fish at ~100 fps which was my end-goal.

![10000 FISH](https://github.com/user-attachments/assets/c9e4feb5-33e6-4d6a-b84d-196b320b601a)


This project taught me practical strengths of Unity's DOTS approach (Jobs, Burst, and NativeContainers), and how importent data layout and memory management are for high-performance real-time simulations. 
It also helped me become a little more comfortable writing multithreaded code.

Learning Resources: 

1. Boids by Craig Reynolds https://www.red3d.com/cwr/boids/
2. Unity URP Cookbook ebook 
3. Unity Jobs documentation https://docs.unity3d.com/Manual/job-system.html
4. Spatial Partitioning https://gameprogrammingpatterns.com/spatial-partition.html
5. Sebastian Lague Boids https://www.youtube.com/watch?v=bqtqltqcQhw

The simulation was tested on my old gaming laptop with following specs:
16GB Ram
512GB M2 SSD
Intel(R) Core(TM) i7-10750H CPU @ 2.60GHz
NVIDIA RTX 2060 6GB Vram

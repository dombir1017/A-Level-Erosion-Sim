# Erosion Simulation

- A basic erosion simulation made for my A-Level Computer Science degree
- The program allows for the generation of terrain using Unity's perlin noise method as a heightmap and then for the simulation of erosion on these meshes
- This erosion simulation is very basic and is primarily meant to improve the look of generated terrain rather than being physically accurate.

![Image of terrain before erosion run](https://github.com/user-attachments/assets/1edb905f-48bb-4947-a639-15af26847259)
![Image of terrain after erosion completed](https://github.com/user-attachments/assets/6cf0a052-3c3e-4d62-a7f6-bc92b0a5ce7e)

## Future Features:

- As this project was done for an A-Level, I was relativley inexperienced when I wrote it
- In future I would like to revisit this project and change the implementation/add new features such as:
    - Implementing the erosion sim as a compute shader rather than using multithreading to speed the process up
    - Trying to make the erosion process more physically accurate
    - Changing the material used for the mesh as it is very flat and does not allow for much of the mesh's detail to be seen.

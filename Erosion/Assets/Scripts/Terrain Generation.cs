using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    public GameObject Terrain;
    private Mesh mesh;
    public int width, height;

    // Start is called before the first frame update
    void Start()
    {
        Vector3[,] verts = new Vector3[width, height];
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                verts[i, j] = new Vector3(i, 0, j);
            }
        }
        for (int i = 0; i < height; i++)
        {
            mesh.triangles
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

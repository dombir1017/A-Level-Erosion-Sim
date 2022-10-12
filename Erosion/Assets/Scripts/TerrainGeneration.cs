using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainGeneration : MonoBehaviour
{
    public int width, height;
    [Range(0, 50)]
    public int amplitude;
    [Range(0.1f, 100f)]
    public float scale;
    public int octaves;

    private Mesh mesh;
    void OnRenderObject()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        this.GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "terrain mesh";

        Vector3[] verts = new Vector3[(width + 1) * (height + 1)];
        for (int i = 0, y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                verts[i] = new Vector3(x, Noise(x, y), y);
            }
        }
        mesh.vertices = verts;

        int[] tris = new int[width * height * 6];
        for (int i = 0, j = 0, y = 0; y < height; y++, j++)
        {
            for (int x = 0; x < width; x++, i += 6, j++)
            {
                tris[i] = j;
                tris[i + 3] = tris[i + 2] = j + 1;
                tris[i + 4] = tris[i + 1] = j + width + 1;
                tris[i + 5] = j + width + 2;
            }
        }
        mesh.triangles = tris;
    }

    float Noise(int x, int y)
    {
        float val = 0;
        for(int i = 1; i < octaves; i++)
        {
            val += Mathf.PerlinNoise(x / (scale*i), y / (scale*i)) * amplitude/i;
        }

        return val;
    }
}

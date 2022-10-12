using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainGeneration : MonoBehaviour
{
    public int width, height;
    [Range(0.1f, 100f)]
    public float terrainHeight;
    [Range(0.1f, 100f)]
    public float scale;
    [Range(1f, 10f)]
    public float lacunarity; //Each octave's frequency multiplied by lacunarity
    [Range(0.001f, 0.1f)]
    public float persistance; //Each octave's amplitude multiplied by persistance
    public int octaves;
    public Gradient gr;

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

        Color[] colours = new Color[verts.Length];
        for (int i = 0, y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                float vertHeight = Mathf.InverseLerp(terrainHeight, 0, verts[i].y);
                colours[i] = gr.Evaluate(vertHeight);
            }
        }
        mesh.colors = colours;

        mesh.RecalculateNormals();
    }

    float Noise(int x, int y)
    {
        float val = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 1; i < octaves; i++)
        {
            float scaledX = x / scale * frequency;
            float scaledY = y / scale * frequency;

            float noiseValue = Mathf.PerlinNoise(scaledX, scaledY);
            val += noiseValue * amplitude * terrainHeight;
            frequency *= lacunarity;
            amplitude *= persistance;
        }

        return val;
    }
}

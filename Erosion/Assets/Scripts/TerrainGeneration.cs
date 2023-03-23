using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    private const int octaves = 5;
    private const float persistence = 0.15f, lacunarity = 4f;
    public readonly int length = 100;


    private float terrainHeight;
    private float scale;
    private int offset;


    public Dictionary<Vector3, float> vertHardness;

    public float[] values
    {
        get { return new float[] {terrainHeight, scale, offset}; }
        set { terrainHeight = value[0]; scale = value[1]; offset = Convert.ToInt32(value[2]); }
    }

    public Gradient gr;
    private Mesh mesh;

    public void GenerateTerrain()//Generates terrain with current values
    {
        vertHardness = new Dictionary<Vector3, float>();
        this.GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "terrain mesh";
        Color[] colours = new Color[(length + 1) * (length + 1)];

        for (int i = 0, y = 0; y <= length; y++) //Constructs grid of vertices with heights offset and assigns colours to each vertex
        {
            for (int x = 0; x <= length; x++, i++)
            {
                float height = GetHeight(x, y);
                vertHardness.Add(new Vector3(x, height, y), getHardness(x, y));
                float vertHeight = Mathf.InverseLerp(terrainHeight, 0, vertHardness.ElementAt(i).Key.y);
                colours[i] = gr.Evaluate(vertHeight);
            }
        }

        int[] tris = new int[length * length * 6];
        for (int i = 0, j = 0, y = 0; y < length; y++, j++) //Place indices of vertices into triangles array in correct order
        {
            for (int x = 0; x < length; x++, i += 6, j++)
            {
                tris[i] = j;
                tris[i + 3] = tris[i + 2] = j + 1;
                tris[i + 4] = tris[i + 1] = j + length + 1;
                tris[i + 5] = j + length + 2;
            }
        }

        mesh.vertices = vertHardness.Keys.ToArray();
        mesh.triangles = tris;
        mesh.colors = colours;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public void RedisplayMesh(Vector3[] v)//Sets mesh vertices to new values
    {
        mesh.vertices = v;
        mesh.RecalculateNormals();
    }

    float GetHeight(int x, int y) //Uses perlin noise to generate height of each vertex
    {
        float val = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            float scaledX = (x+offset) / scale * frequency;
            float scaledY = (y + offset) / scale * frequency;

            float noiseValue = Mathf.PerlinNoise(scaledX, scaledY);
            val += noiseValue * amplitude * terrainHeight;
            frequency *= lacunarity;  //Each octave's frequency multiplied by lacunarity
            amplitude *= persistence;//Each octave's amplitude multiplied by persistence
        }
        return val;
    }
    
    float getHardness(int x, int y) //Gets hardness value in range 0 to 1 for given vertex - higher vertices have higher hardness values
    {
        float val;
        float scaledX = (x + offset) / scale;
        float scaledY = (y + offset) / scale;

        val = Mathf.PerlinNoise(scaledX, scaledY);
        return val;
    }
}

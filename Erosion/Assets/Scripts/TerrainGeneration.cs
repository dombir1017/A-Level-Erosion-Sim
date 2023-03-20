using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class TerrainGeneration : MonoBehaviour
{
    private const int length = 100, radius = 65, octaves = 5;
    private const float persistance = 0.15f, lacunarity = 4f;
                                                             
    private float terrainHeight;
    private float scale;
    private int offset;

    public Vector3[] verts;

    public float[] values
    {
        get { return new float[] {terrainHeight, scale, offset}; }
        set { terrainHeight = value[0]; scale = value[1]; offset = Convert.ToInt32(value[2]); }
    }

    public Gradient gr;
    private Mesh mesh;

    public Erosion er;

    public void GenerateTerrain()//Generates terrain with current values
    {
        this.GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "terrain mesh";

        verts = new Vector3[(length + 1) * (length + 1)];

        for (int i = 0, y = 0; y <= length; y++) //Constructs grid of vertices with heights offset based upon noise values
        {
            for (int x = 0; x <= length; x++, i++)
            {
                float length = Noise(x, y);
                verts[i] = new Vector3(x, length, y);
            }
        }
        mesh.vertices = verts;

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
        mesh.triangles = tris;

        Color[] colours = new Color[verts.Length];
        for (int i = 0, y = 0; y <= length; y++) //Colour each vertex of terrain depending upon its height
        {
            for (int x = 0; x <= length; x++, i++)
            {
                float vertHeight = Mathf.InverseLerp(terrainHeight, 0, verts[i].y);
                colours[i] = gr.Evaluate(vertHeight);
            }
        }
        mesh.colors = colours;
        mesh.RecalculateNormals();
        er.mesh = mesh;
        er.length = length;
    }

    float Noise(int x, int y) //Uses perlin noise to generate height of each vertex
    {
        float val = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 1; i < octaves; i++)
        {
            float scaledX = (x+offset) / scale * frequency;
            float scaledY = (y + offset) / scale * frequency;

            float noiseValue = Mathf.PerlinNoise(scaledX, scaledY);
            val += noiseValue * amplitude * terrainHeight;
            frequency *= lacunarity;  //Each octave's frequency multiplied by lacunarity
            amplitude *= persistance;//Each octave's amplitude multiplied by persistance
        }
        return val;
    }
}

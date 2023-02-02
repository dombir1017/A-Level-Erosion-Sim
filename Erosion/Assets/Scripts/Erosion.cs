using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;

public class Erosion : MonoBehaviour
{
    public Mesh mesh;
    public int dropletAttempts, length, dropsCompleted = 0;
    public float carryAmount;
    private int[] tris;
    public MenuManager menuManager;
    List<int> randomTriangles;

    private List<int> HashTriangles(int start, int end, int numberOfElements)
    {
        var random = new System.Random();
        HashSet<int> ints = new HashSet<int>();
        while(ints.Count < numberOfElements)
        {
            ints.Add(random.Next(start, end));
        }
        return ints.ToList();
    }

    public void StartErosion()
    {
        dropsCompleted = 0;
        Vector3[] verts = mesh.vertices;
        tris = mesh.triangles;

        randomTriangles = HashTriangles(0, mesh.triangles.Length / 3, dropletAttempts);
        List<Thread> threads = new List<Thread>();

        for (int i = 0; i < 10; i++)
        {
            int startValue = i; //Thread requires local variable -
            Debug.Log(i);
            Thread t = new(() => ThreadStart(i, ref verts));
            t.Start();
            threads.Add(t);
        }

        mesh.vertices = verts;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void ThreadStart(int startPoint, ref Vector3[] verts)
    {
        //Debug.Log(startPoint);
        for (int i = startPoint * dropletAttempts/10; i < (startPoint+1) * dropletAttempts / 10; i++) //From frist allocated point to last
        {
            try
            {
                Debug.Log(i);
            }
            catch
            {
                Debug.Log(i);
            }
            
            //RunDroplet(randomTriangles[i], ref verts);
        }
    }
    
    private void RunDroplet(int triIndex, ref Vector3[] verts, float sediment = 0f, int numMoved = 0)
    {
        if (verts[tris[triIndex * 3]].y > 0 && numMoved < 100)
        {
            Vector3 normalToTriangle = calculateNormal(triIndex * 3, verts);
            Vector2 normalDirection = new Vector2(normalToTriangle.x, normalToTriangle.z).normalized;

            int newPosition = 0;
            int verticalIndex = length * 2 - 2;
            float angle = Vector2.SignedAngle(normalDirection, Vector2.up);
            int direction = Mathf.RoundToInt(angle / 45f);
            switch (Math.Abs(direction))
            {
                case 0:
                    newPosition = triIndex + verticalIndex;
                    break;
                case 1:
                    newPosition = triIndex + verticalIndex + direction;
                    break;
                case 2:
                    newPosition = triIndex + (direction / 2);
                    break;
                case 3:
                    newPosition = triIndex - verticalIndex + (direction / 3);
                    break;
                case 4:
                    newPosition = triIndex - verticalIndex;
                    break;
            }

            float gradient = Vector3.Angle(Vector3.up, normalToTriangle)/90; //Can never have overhang so will always be between 0 and 1
            float removedMaterial = 0.00005f;
            float depositedMaterial = 0;

            if (removedMaterial + sediment > carryAmount)
            {
                depositedMaterial = removedMaterial + sediment - carryAmount * (1-gradient);
            }
            float newSediment = sediment + removedMaterial - depositedMaterial;

            verts[tris[triIndex * 3]].y += depositedMaterial - removedMaterial;
            verts[tris[triIndex * 3 + 1]].y += depositedMaterial - removedMaterial;
            verts[tris[triIndex * 3 + 2]].y += depositedMaterial - removedMaterial;

            RunDroplet(newPosition, ref verts, newSediment, numMoved + 1);
        }
        else
        {
            dropsCompleted += 1;
        }
    }

    private Vector3 calculateNormal(int triangle, Vector3[] verts)
    {
        Vector3 U = verts[tris[triangle]] - verts[tris[triangle + 1]];
        Vector3 V = verts[tris[triangle]] - verts[tris[triangle + 2]];

        Vector3 normal = new(){
            x = U.y * V.z - U.z * V.y,
            y = U.z * V.x - U.x * V.z,
            z = U.x * V.y - U.y * V.x
        };
        return Vector3.Normalize(normal);
    }
}

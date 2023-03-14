using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public class Erosion : MonoBehaviour
{
    private const int dropletAttempts = 10000;
    private int[] tris;
    private List<int> randomVerts;

    public int numIterations;
    public int numIterationsRun;
    public Mesh mesh;
    public int length;
    public float removal, deposition, maxSediment;
    public MenuManager menuManager;

    private List<int> HashVerts(int start, int end, int numberOfElements)
    {
        var random = new System.Random();
        HashSet<int> ints = new HashSet<int>();
        while(ints.Count < numberOfElements)
        {
            ints.Add(random.Next(start, end));
        }
        return ints.ToList();
    }

    public IEnumerator StartErosion()
    {
        numIterationsRun = 0;
        Vector3[] verts = mesh.vertices;
        tris = mesh.triangles;

        for(int j = 0; j < numIterations; j++)
        {
            randomVerts = HashVerts(0, mesh.vertices.Length, dropletAttempts); //Hash vertices to get droplet start points for iteration
            Thread[] threads = new Thread[10];

            for (int i = 0; i < 10; i++)//Split each iteration between 10 threads
            {
                int startPoint = i;
                Thread t = new(() => ThreadStart(startPoint, ref verts));
                t.Start();
                threads[i] = t;
            }
            for (int i = 0; i < 10; i++)//Wait for each thread to finish
            {
                threads[i].Join();
            }

            numIterationsRun ++;
            mesh.vertices = verts; //Redisplay terrain
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            yield return new WaitForFixedUpdate();
        }
    }

    private void ThreadStart(int startPoint, ref Vector3[] verts)//Run for each thread - allocates start and end points for each thread to work on
    {
        for (int i = startPoint * dropletAttempts/10; i < (startPoint+1) * dropletAttempts / 10; i++)
        {
            RunDroplet(randomVerts[i], ref verts);
        }
    }

    private void RunDroplet(int vertIndex, ref Vector3[] verts, float sediment = 0f, float velocity = 0f)//Runs a single droplet down terrain from given point recursively
    {
        if (verts[vertIndex].y > 0)//If height of vertex above sea level
        {
            verts[vertIndex].y -= removal;

            Dictionary<int, float> heights = new Dictionary<int, float>();
            for (int i = -1; i < 3; i++)//Add vertex data to dictionary if its height is lower than central vertex
            {
                int index = vertIndex;
                switch (Math.Abs(i))
                {
                    case 0:
                        index -= 1;
                        break;
                    case 1:
                        index += i * (length + 1);
                        break;
                    case 2:
                        index += 1;
                        break;
                }
                Vector3 examinedVertex = verts[index];
                if (examinedVertex.y < verts[vertIndex].y)
                {
                    heights.Add(index, examinedVertex.y);
                }
            }

            if (heights.Count > 0) //If vertex is not lowest available, run again
            {
                int newPosition = heights.OrderBy(x => x.Value).First().Key;
                float newVelocity = verts[vertIndex].y - verts[newPosition].y;
                RunDroplet(newPosition, ref verts);
            }
            else
            {
                verts[vertIndex].y += sediment; //Deposit all currently carried sediment
            }
        }
    }
}
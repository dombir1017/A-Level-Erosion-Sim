using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;

public class Erosion : MonoBehaviour
{
    private const int dropletAttempts = 10000;
    private List<int> randomVerts;
    private int numIterations;
    private float removal, maxSediment;

    public float[] values
    {
        get {return new float[] { numIterations, removal, maxSediment };}
        set { numIterations = Convert.ToInt32(value[0]); removal = value[1]; maxSediment = value[2] ; }
    }

    public int numIterationsRun;
    public TerrainGeneration tg;
    private int length;
    private Dictionary<Vector3, float> vertHardness;

    private List<int> HashVerts(int start, int end, int numberOfElements) //Creates list of random integers that are used as start points for each droplet
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
        length = tg.length;
        numIterationsRun = 0;
        vertHardness = tg.vertHardness;
        Vector3[] verts = vertHardness.Keys.ToArray();
        float[] hardness = vertHardness.Values.ToArray();

        for(int j = 0; j < numIterations; j++)
        {
            randomVerts = HashVerts(0, vertHardness.Count, dropletAttempts); //Hash vertices to get droplet start points for iteration
            Thread[] threads = new Thread[10];

            for (int i = 0; i < 10; i++)//Split each iteration between 10 threads
            {
                int startPoint = i;
                Thread t = new(() => ThreadStart(startPoint, ref verts, ref hardness));
                t.Start();
                threads[i] = t;
            }
            for (int i = 0; i < 10; i++)//Wait for each thread to finish
            {
                threads[i].Join();
            }

            numIterationsRun ++;
            tg.RedisplayMesh(verts);
            yield return new WaitForFixedUpdate();
        }
    }

    private void ThreadStart(int startPoint, ref Vector3[] verts, ref float[] hardness)//Run for each thread - allocates start and end points for each thread to work on
    {
        for (int i = startPoint * dropletAttempts/10; i < (startPoint+1) * dropletAttempts / 10; i++)
        {
            RunDroplet(randomVerts[i], ref verts, ref hardness);
        }
    }

    private void RunDroplet(int vertIndex, ref Vector3[] verts, ref float[] hardness, float sediment = 0f)//Runs a single droplet down terrain from given point recursively
    {
        int vertX = vertIndex % (length + 1);
        int vertY = vertIndex / (length + 1);
        if (vertX < length - 1 && vertX > 1 && vertY < length - 1 && vertY > 1)
        {
            float removedMaterial = removal / 10000f * (1-hardness[vertIndex]); //Removes material based upon terrain hardness at point
            float newSediment = Math.Clamp(removedMaterial + sediment, 0, maxSediment);
            float depositedMaterial = 0;
            if (newSediment == sediment)
            {
                depositedMaterial = removedMaterial;
            }

            verts[vertIndex].y -= removedMaterial - depositedMaterial;

            Dictionary<int, float> heights = new();
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
                RunDroplet(newPosition, ref verts, ref hardness, newSediment);
            }
            else
            {
                verts[vertIndex].y += sediment; //Deposit all currently carried sediment
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Erosion : MonoBehaviour
{
    public Mesh mesh;
    public int dropletAttempts, length;
    public float carryAmount;
    private TerrainGeneration tg;
    public MenuManager menuManager;
    public float a;

    //public List<Int32> indices;

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.blue;
    //    for(int i =0; i < indices.Count; i++)
    //    {
    //        Vector3 centre = (mesh.vertices[mesh.triangles[indices[i]*3]] + mesh.vertices[mesh.triangles[indices[i]*3+1]] + mesh.vertices[mesh.triangles[indices[i]*3+2]])/3;
    //        Gizmos.DrawSphere(centre, 0.3f);
    //    }
    //}

    private void Start()
    {
        tg = GetComponent<TerrainGeneration>();
    }

    private List<int> GetRandomTriangle(int start, int end, int numberOfElements)
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

        dropletAttempts = mesh.triangles.Length / 3;
        for (int i = 0; i < mesh.triangles.Length / 3; i++)
        {
            //int x = Random.Range(0, 19999);
            //indices = new List<int>();
            RunDroplet(1);
            menuManager.progress.text = string.Join("/", i + 1, dropletAttempts);
            yield return new WaitForSecondsRealtime(0.001f);
        }
    }

    private void RunDroplet(int triIndex, int prevPoint = 0, float sediment = 0f, int numMoved = 0)
    {
        //indices.Add(triIndex);
        if (mesh.vertices[mesh.triangles[triIndex * 3]].y > 0)
        {
            Vector3 normalToTriangle = calculateNormal(triIndex * 3);
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
            float newSediment = Math.Clamp(sediment + gradient, 0, carryAmount); //Gradient determines how much material is removed

            AddToTriangle(triIndex, (sediment - newSediment) * a);

            if (newPosition != prevPoint && numMoved < 100)
            {
                RunDroplet(newPosition, triIndex, newSediment, numMoved + 1);
            }
            else
            {
                mesh.vertices = tg.verts;
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
            }
        }
    }

    private void AddToTriangle(int triIndex, float amount)
    {
        tg.verts[mesh.triangles[triIndex * 3]].y += amount;
    }

    private Vector3 calculateNormal(int triangle)
    {
        Vector3 U = mesh.vertices[mesh.triangles[triangle]] - mesh.vertices[mesh.triangles[triangle + 1]];
        Vector3 V = mesh.vertices[mesh.triangles[triangle]] - mesh.vertices[mesh.triangles[triangle + 2]];

        Vector3 normal = new(){
            x = U.y * V.z - U.z * V.y,
            y = U.z * V.x - U.x * V.z,
            z = U.x * V.y - U.y * V.x
        };
        return Vector3.Normalize(normal);
    }
}

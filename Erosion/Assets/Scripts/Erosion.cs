using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class Erosion : MonoBehaviour
{
    public Mesh mesh;
    public int dropletAttempts, length;
    public float startVelocity = 0, mass, removal;
    private TerrainGeneration tg;


    List<Int32> points;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        for(int i = 0; i < points.Count; i ++)
        {
            Gizmos.DrawSphere(mesh.vertices[points[i]], 0.05f);
        }
    }

    public void StartErosion()
    {
        points = new List<Int32>();
        for (int i = 0; i < dropletAttempts; i++)
        {
            int x = Random.Range(30, 70);
            int z = Random.Range(30, 70);
            if(mesh.vertices[x + (length + 1) * z].y >= 0)
            {
                points.Add(x + (length + 1) * z);
                RunDroplet(points[0]);
            }
        }
        
    }

    private void RunDroplet(int point, int prevPoint = 0)
    {
        Vector3 normalToPoint = calculateNormal(point);
        Vector2 normalDirection = (new Vector2(normalToPoint.x, normalToPoint.z)).normalized;
        int newPosition = 0;
        float angle = Vector2.SignedAngle(normalDirection, Vector2.up);
        int direction = Mathf.RoundToInt(angle / 45f);
        switch (Math.Abs(direction))
        {
            case 0:
                newPosition = point + length + 1;
                break;
            case 1:
                newPosition = point + length + 1 + direction;
                break;
            case 2:
                newPosition = point + (direction/2);
                break;
            case 3:
                newPosition = point - length - 1 + (direction / 3);
                break;
            case 4:
                newPosition = point - length - 1;
                break;
        }
        if (mesh.vertices[newPosition].y > 0 && newPosition != prevPoint)
        {
            points.Add(newPosition);
            mesh.vertices[point].y -= 1f;
            RunDroplet(newPosition, point);
        }
        else
        {
            mesh.RecalculateNormals();
        }
    }

    public Vector3 calculateNormal(int vertexNumber)
    {
        int column = vertexNumber % (length + 1);
        int row = vertexNumber / (length + 1);

        Vector3 U = mesh.vertices[mesh.triangles[(row * length * 2 + column * 2) * 3 + 1]] - mesh.vertices[mesh.triangles[(row * length * 2  + column * 2) * 3]];
        Vector3 V = mesh.vertices[mesh.triangles[(row * length * 2 + column * 2) * 3 + 2]] - mesh.vertices[mesh.triangles[(row * length * 2 + column * 2) * 3]];

        Vector3 normal = new Vector3{
            x = U.y * V.z - U.z * V.y,
            y = U.z * V.x - U.x * V.z,
            z = U.x * V.y - U.y * V.x
        };
        return Vector3.Normalize(normal);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class Erosion : MonoBehaviour
{
    public Mesh mesh;
    public int dropletAttempts, length;
    public float velocity = 3, mass, removal, carryAmount, depositionAngle;
    private TerrainGeneration tg;
    private List<Int32> points;
    public MenuManager menuManager;

    private void Start()
    {
        tg = GetComponent<TerrainGeneration>();
    }


    public IEnumerator StartErosion()
    {
        points = new List<Int32>();
        for (int i = 0; i < dropletAttempts; i++)
        {
            int x = Random.Range(15, 85);
            int z = Random.Range(15, 85);
            if (mesh.vertices[x + (length + 1) * z].y >= 0)
            {
                points.Add(x + (length + 1) * z);
                RunDroplet(points[points.Count - 1]);
            }
            menuManager.progress.text = string.Join("/", i + 1, dropletAttempts);
            yield return new WaitForFixedUpdate();
        }
    }

    private void RunDroplet(int point, int prevPoint = 0, float sediment = 0f)
    {
        Vector3 normalToPoint = calculateNormal(point);
        Vector2 normalDirection = new Vector2(normalToPoint.x, normalToPoint.z).normalized;
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
        float gradient = Vector2.Angle(Vector2.up, new Vector2(normalToPoint.x, normalToPoint.y));
        float removedMaterial = gradient/90; //Can never have overhang so will always be between 0 and 1
        float depositedMaterial = sediment * (1-removedMaterial);
        float newSediment = sediment + removedMaterial;
        
        tg.verts[point].y -= removedMaterial;

        if (gradient < depositionAngle)
        {
            tg.verts[point].y += depositedMaterial;
            newSediment -= depositedMaterial;
        }


        if (mesh.vertices[newPosition].y > 0 && newPosition != prevPoint)
        {
            points.Add(newPosition);
            RunDroplet(newPosition, point, newSediment);
        }
        else
        {
            tg.verts[point].y += sediment;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
        mesh.vertices = tg.verts;
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

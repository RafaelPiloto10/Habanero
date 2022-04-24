using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Terrain : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    public int xSize = 150;
    public int zSize = 150;

    public float dt = 2f;

    private float offsetX = 0.0f;
    private float offsetZ = 0.0f;

    public float freq1 = 0.03f;
    public float freq2 = 0.03f;
    public float freq3 = 0.01f;

    public float amp1 = 20;
    public float amp2 = 10;
    public float amp3 = 2;

    public float noiseStrength = 5f;

    public Gradient gradient;
    public static Gradient gradientCopy;

    private static float minHeight;
    private static float maxHeight;


    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        UpdateMesh();
        gradientCopy = gradient;
    }

    private void FixedUpdate()
    {
        CreateShape();
        UpdateMesh();

        offsetX += dt * Time.deltaTime;
        offsetZ += dt * Time.deltaTime;
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = amp1 * Mathf.PerlinNoise((x + offsetX) * freq1, (z + offsetZ) * freq1) +
                    amp2 * Mathf.PerlinNoise((x + offsetX) * freq2, (z + offsetZ) * freq2) +
                    amp3 * Mathf.PerlinNoise((x + offsetX) * freq3, (z + offsetZ) * freq3)
                        * noiseStrength;

                vertices[i] = new Vector3(x, y, z);

                if (y > maxHeight)
                {
                    maxHeight = y;
                }

                if (y < minHeight)
                {
                    minHeight = y;
                }

                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }

    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        
    }

    public static Color WhereAmI(float height)
    {
        return gradientCopy.Evaluate(Mathf.InverseLerp(minHeight, maxHeight, height));
    }

}
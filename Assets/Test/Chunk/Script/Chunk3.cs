using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk3 : MonoBehaviour
{
    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();

    private Mesh _mesh;

    private int[,,] _map;
    public int width = 20, height = 10;

	void Start () {
       CalculateMap();
	}

    private void CalculateMap()
    {
        _mesh = new Mesh();
        _mesh.name = "Chunck";

        _map = new int[width, height, width];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    _map[x, y, z] = 1;
                }
            }
        }
        CalculateMesh();
    }

    private void CalculateMesh()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    if (_map[x, y, z] != 0)
                    {
                        AddCube(x, y, z);
                    }
                }
            }
        }

        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();

        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = _mesh;
        GetComponent<MeshFilter>().mesh = _mesh;
    }

    private void AddCube(int x, int y, int z)
    {
        if (IsBlockTransparent(x, y, z - 1))
            AddCubeFront(x, y, z);
        if (IsBlockTransparent(x, y, z + 1))
            AddCubeBack(x, y, z);
        if (IsBlockTransparent(x - 1, y, z))
            AddCubeLeft(x, y, z);
        if (IsBlockTransparent(x + 1, y, z))
            AddCubeRight(x, y, z);
        if (IsBlockTransparent(x, y + 1, z))
            AddCubeTop(x, y, z);
        if (IsBlockTransparent(x, y - 1, z))
            AddCubeBottom(x, y, z);
    }

    private void AddCubeFront(int x, int y, int z)
    {
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);

        _vertices.Add(new Vector3(-0.5f + x, -0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3( 0.5f + x, -0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3( 0.5f + x,  0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x,  0.5f + y, -0.5f + z));
    }

    private void AddCubeBack(int x, int y, int z)
    {
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);

        _vertices.Add(new Vector3(-0.5f + x, -0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3( 0.5f + x, -0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3( 0.5f + x,  0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x,  0.5f + y, 0.5f + z));
    }

    private void AddCubeLeft(int x, int y, int z)
    {
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);

        _vertices.Add(new Vector3(-0.5f + x, -0.5f + y,  0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, -0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x,  0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x,  0.5f + y,  0.5f + z));
    }

    private void AddCubeRight(int x, int y, int z)
    {
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);

        _vertices.Add(new Vector3(0.5f + x, -0.5f + y,  0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, -0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(0.5f + x,  0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(0.5f + x,  0.5f + y,  0.5f + z));
    }

    private void AddCubeTop(int x, int y, int z)
    {
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);

        _vertices.Add(new Vector3(-0.5f + x, 0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3( 0.5f + x, 0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3( 0.5f + x, 0.5f + y,  0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, 0.5f + y,  0.5f + z));
    }

    private void AddCubeBottom(int x, int y, int z)
    {
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);

        _vertices.Add(new Vector3(-0.5f + x, -0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3( 0.5f + x, -0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3( 0.5f + x, -0.5f + y,  0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, -0.5f + y,  0.5f + z));
    }

    public bool IsBlockTransparent(int x, int y, int z)
    {
        if (x >= width || y >= height || z >= width || x < 0 || y < 0 || z < 0)
            return true;

        return false;
    }
}

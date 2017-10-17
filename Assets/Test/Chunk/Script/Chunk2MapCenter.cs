using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk2MapCenter : MonoBehaviour
{
    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();

    private Mesh _mesh;

    private int[,,] _map;
    public int length;
    public int width;
    public int height;

    void Start()
    {
        CalculateMap();
    }

    /// <summary>
    /// 预处理地形的函数
    /// 通过不同的算法产生地形数据
    /// </summary>
    private void CalculateMap()
    {
        _mesh = new Mesh();
        _mesh.name = "Chunck";

        _map = new int[length, height, width];
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    _map[x, y, z] = 1;
                }
            }
        }
        //StartCoroutine(CalculateMeshByStep());
        CalculateMesh();
    }

    private IEnumerator CalculateMeshByStep()
    {
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    if (_map[x, y, z] != 0)
                    {
                        AddCube(x, y, z);
                        _mesh.vertices = _vertices.ToArray();
                        _mesh.triangles = _triangles.ToArray();

                        _mesh.RecalculateBounds();
                        _mesh.RecalculateNormals();
                        GetComponent<MeshCollider>().sharedMesh = _mesh;
                        GetComponent<MeshFilter>().mesh = _mesh;

                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }
        }
    }

    private void CalculateMesh()
    {
        for (int x = 0; x < length; x++)
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

    #region 组织立方体顶点
    private void AddCube(int x, int y, int z)
    {
        // 将选择点移到中心
        int centerX = x - length / 2;
        int centerY = y - height / 2;
        int centerZ = z - width / 2;

        if (IsBlockTransparent(x, y, z - 1))
            AddCubeFront(centerX, centerY, centerZ);
        if (IsBlockTransparent(x, y, z + 1))
            AddCubeBack(centerX, centerY, centerZ);
        if (IsBlockTransparent(x - 1, y, z))
            AddCubeLeft(centerX, centerY, centerZ);
        if (IsBlockTransparent(x + 1, y, z))
            AddCubeRight(centerX, centerY, centerZ);
        if (IsBlockTransparent(x, y + 1, z))
            AddCubeTop(centerX, centerY, centerZ);
        if (IsBlockTransparent(x, y - 1, z))
            AddCubeBottom(centerX, centerY, centerZ);
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
        _vertices.Add(new Vector3(0.5f + x, -0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, 0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, 0.5f + y, -0.5f + z));
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
        _vertices.Add(new Vector3(0.5f + x, -0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, 0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, 0.5f + y, 0.5f + z));
    }

    private void AddCubeLeft(int x, int y, int z)
    {
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);

        _vertices.Add(new Vector3(-0.5f + x, -0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, -0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, 0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, 0.5f + y, 0.5f + z));
    }

    private void AddCubeRight(int x, int y, int z)
    {
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);

        _vertices.Add(new Vector3(0.5f + x, -0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, -0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, 0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, 0.5f + y, 0.5f + z));
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
        _vertices.Add(new Vector3(0.5f + x, 0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, 0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, 0.5f + y, 0.5f + z));
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
        _vertices.Add(new Vector3(0.5f + x, -0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, -0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, -0.5f + y, 0.5f + z));
    }
    #endregion

    /// <summary>
    /// 关键的优化方法，只显示边界的面
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool IsBlockTransparent(int x, int y, int z)
    {
        if (x >= length || y >= height || z >= width || x < 0 || y < 0 || z < 0)
            return true;

        return false;
    }
}

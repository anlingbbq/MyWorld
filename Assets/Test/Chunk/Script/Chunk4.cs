using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk4 : MonoBehaviour
{
    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();
    private List<Vector2> _uvs = new List<Vector2>();

    public float _textureOffset = 1 / 16f;

    private Mesh _mesh;

    private Block[,,] _map;
    public int length = 10;
    public int width = 10;
    public int height = 10;

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

        _map = new Block[length, height, width];
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    if (y == height - 1)
                        _map[x, y, z] = BlockMap.GetBlock("Grass");
                    else
                        _map[x, y, z] = BlockMap.GetBlock("Dirt");
                }
            }
        }
        CalculateMesh();
    }

    private void CalculateMesh()
    {
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    if (_map[x, y, z] != null)
                    {
                        AddCube(x, y, z);
                    }
                }
            }
        }

        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.uv = _uvs.ToArray();

        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = _mesh;
        GetComponent<MeshFilter>().mesh = _mesh;
    }

    #region 创建立方体
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
        _vertices.Add(new Vector3(0.5f + x, -0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, 0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, 0.5f + y, -0.5f + z));

        Block block = _map[x, y, z];
        _uvs.Add(new Vector2(block.texture_u_fb * _textureOffset, block.texture_v_fb * _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_fb * _textureOffset + _textureOffset, block.texture_v_fb * _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_fb * _textureOffset + _textureOffset, block.texture_v_fb * _textureOffset + _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_fb * _textureOffset, block.texture_v_fb * _textureOffset + _textureOffset));
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

        Block block = _map[x, y, z];
        _uvs.Add(new Vector2(block.texture_u_fb * _textureOffset, block.texture_v_fb * _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_fb * _textureOffset + _textureOffset, block.texture_v_fb * _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_fb * _textureOffset + _textureOffset, block.texture_v_fb * _textureOffset + _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_fb * _textureOffset, block.texture_v_fb * _textureOffset + _textureOffset));
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

        Block block = _map[x, y, z];
        _uvs.Add(new Vector2(block.texture_u_lr * _textureOffset, block.texture_v_lr * _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_lr * _textureOffset + _textureOffset, block.texture_v_lr * _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_lr * _textureOffset + _textureOffset, block.texture_v_lr * _textureOffset + _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_lr * _textureOffset, block.texture_v_lr * _textureOffset + _textureOffset));
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

        Block block = _map[x, y, z];
        _uvs.Add(new Vector2(block.texture_u_lr * _textureOffset, block.texture_v_lr * _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_lr * _textureOffset + _textureOffset, block.texture_v_lr * _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_lr * _textureOffset + _textureOffset, block.texture_v_lr * _textureOffset + _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_lr * _textureOffset, block.texture_v_lr * _textureOffset + _textureOffset));
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

        Block block = _map[x, y, z];
        _uvs.Add(new Vector2(block.texture_u_top * _textureOffset, block.texture_v_top * _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_top * _textureOffset + _textureOffset, block.texture_v_top * _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_top * _textureOffset + _textureOffset, block.texture_v_top * _textureOffset + _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_top * _textureOffset, block.texture_v_top * _textureOffset + _textureOffset));
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

        Block block = _map[x, y, z];
        _uvs.Add(new Vector2(block.texture_u_bottom * _textureOffset, block.texture_v_bottom * _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_bottom * _textureOffset + _textureOffset, block.texture_v_bottom * _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_bottom * _textureOffset + _textureOffset, block.texture_v_bottom * _textureOffset + _textureOffset));
        _uvs.Add(new Vector2(block.texture_u_bottom * _textureOffset, block.texture_v_bottom * _textureOffset + _textureOffset));
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

        if (_map[x, y, z] == null)
            return true;

        return false;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk4Perlin : MonoBehaviour
{
    private static List<Chunk4Perlin> _chunks = new List<Chunk4Perlin>();

    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();
    private List<Vector2> _uvs = new List<Vector2>();

    /// <summary>
    /// 图块贴图的长和宽
    /// </summary>
    public float _textureOffset = 1 / 16f;

    private Mesh _mesh;

    private Block[,,] _map;
    public static int length = 16;
    public static int width = 16;
    public static int height = 16;

    private static bool _working;
    private bool _ready;

    public static int seed;

    void Start()
    {
        _chunks.Add(this);
    }

    void Update()
    {
        if (!_working && !_ready)
        {
            _ready = true;
            _working = true;

            _mesh = new Mesh();
            _mesh.name = "Chunk";
            _map = new Block[length, height, width];
            gameObject.name = "[" + transform.position.x / length
                + "," + transform.position.y / height
                + "," + transform.position.z / width + "]";

            CreateMap();
        }
    }

    /// <summary>
    /// 预处理地形的函数
    /// 通过不同的算法产生地形数据
    /// </summary>
    private void CreateMap()
    {
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    _map[x, y, z] = GetTheoreticalBlock(new Vector3(x, y, z) + transform.position);
                }
            }
        }
        //yield return null;
        StartCoroutine(CalculateMesh());
    }

    public Block GetTheoreticalBlock(Vector3 pos)
    {
        Random.InitState(seed);
        Vector3 offset = new Vector3(Random.value * 100000, Random.value * 100000, Random.value * 100000);
        float noiseX = Mathf.Abs(pos.x + offset.x) / 20;
        float noiseY = Mathf.Abs(pos.y + offset.y) / 20;
        float noiseZ = Mathf.Abs(pos.z + offset.z) / 20;
        float noiseValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);
        noiseValue += (20.0f - pos.y) / 18;
        noiseValue /= pos.y / 4;

        return noiseValue > 0.2f ? BlockMap.GetBlock("Dirt") : null;
    }

    private IEnumerator CalculateMesh()
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

        yield return null;
        _working = false;
    }

    #region 创建立方体
    /////////////////////////////////////////////////////////////
    /// 顶点坐标
    /// 在左手坐标系中
    /// 统一以左下角为第一个，左上角为最后一个的顺序添加
    /// -----------------
    /// |3             2|
    /// |               |
    /// |               |
    /// |               |
    /// |0             1|
    /// -----------------
    /// 
    /// 纹理方向
    /// 上下显示方向同观察的正面(z轴负方向显示的面)
    /// 四周方向连续
    /////////////////////////////////////////////////////////////
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

    /// <summary>
    /// 处理uv的缝隙
    /// </summary>
    private float _shrinkSize = 0.005f;

    private void AddCubeFront(int x, int y, int z)
    {
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);

        _vertices.Add(new Vector3(-0.5f + x, -0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, -0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, 0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, 0.5f + y, -0.5f + z));

        Block block = _map[x, y, z];
        Vector2 orgUv = new Vector2(block.texture_u_fb * _textureOffset, block.texture_v_fb * _textureOffset);
        _uvs.Add(orgUv + new Vector2(_shrinkSize, _shrinkSize));
        _uvs.Add(orgUv + new Vector2(_textureOffset, 0) + new Vector2(-_shrinkSize, _shrinkSize));
        _uvs.Add(orgUv + new Vector2(_textureOffset, _textureOffset) + new Vector2(-_shrinkSize, -_shrinkSize));
        _uvs.Add(orgUv + new Vector2(0, _textureOffset) + new Vector2(_shrinkSize, -_shrinkSize));
    }

    private void AddCubeBack(int x, int y, int z)
    {
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);

        _vertices.Add(new Vector3(0.5f + x, -0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, -0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, 0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, 0.5f + y, 0.5f + z));

        Block block = _map[x, y, z];
        Vector2 orgUv = new Vector2(block.texture_u_fb * _textureOffset, block.texture_v_fb * _textureOffset);
        _uvs.Add(orgUv + new Vector2(_shrinkSize, _shrinkSize));
        _uvs.Add(orgUv + new Vector2(_textureOffset, 0) + new Vector2(-_shrinkSize, _shrinkSize));
        _uvs.Add(orgUv + new Vector2(_textureOffset, _textureOffset) + new Vector2(-_shrinkSize, -_shrinkSize));
        _uvs.Add(orgUv + new Vector2(0, _textureOffset) + new Vector2(_shrinkSize, -_shrinkSize));
    }

    private void AddCubeLeft(int x, int y, int z)
    {
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);

        _vertices.Add(new Vector3(-0.5f + x, -0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, 0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, 0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, -0.5f + y, -0.5f + z));

        Block block = _map[x, y, z];
        Vector2 orgUv = new Vector2(block.texture_u_lr * _textureOffset, block.texture_v_lr * _textureOffset);
        _uvs.Add(orgUv + new Vector2(_shrinkSize, _shrinkSize));
        _uvs.Add(orgUv + new Vector2(0, _textureOffset) + new Vector2(_shrinkSize, -_shrinkSize));
        _uvs.Add(orgUv + new Vector2(_textureOffset, _textureOffset) + new Vector2(-_shrinkSize, -_shrinkSize));
        _uvs.Add(orgUv + new Vector2(_textureOffset, 0) + new Vector2(-_shrinkSize, _shrinkSize));
    }

    private void AddCubeRight(int x, int y, int z)
    {
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);

        _vertices.Add(new Vector3(0.5f + x, -0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, -0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, 0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, 0.5f + y, -0.5f + z));

        Block block = _map[x, y, z];
        Vector2 orgUv = new Vector2(block.texture_u_lr * _textureOffset, block.texture_v_lr * _textureOffset);
        _uvs.Add(orgUv + new Vector2(_shrinkSize, _shrinkSize));
        _uvs.Add(orgUv + new Vector2(_textureOffset, 0) + new Vector2(-_shrinkSize, _shrinkSize));
        _uvs.Add(orgUv + new Vector2(_textureOffset, _textureOffset) + new Vector2(-_shrinkSize, -_shrinkSize));
        _uvs.Add(orgUv + new Vector2(0, _textureOffset) + new Vector2(_shrinkSize, -_shrinkSize));
    }

    private void AddCubeTop(int x, int y, int z)
    {
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);

        _vertices.Add(new Vector3(-0.5f + x, 0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, 0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, 0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, 0.5f + y, 0.5f + z));

        Block block = _map[x, y, z];
        Vector2 orgUv = new Vector2(block.texture_u_top * _textureOffset, block.texture_v_top * _textureOffset);
        _uvs.Add(orgUv + new Vector2(_shrinkSize, _shrinkSize));
        _uvs.Add(orgUv + new Vector2(_textureOffset, 0) + new Vector2(-_shrinkSize, _shrinkSize));
        _uvs.Add(orgUv + new Vector2(_textureOffset, _textureOffset) + new Vector2(-_shrinkSize, -_shrinkSize));
        _uvs.Add(orgUv + new Vector2(0, _textureOffset) + new Vector2(_shrinkSize, -_shrinkSize));
    }

    private void AddCubeBottom(int x, int y, int z)
    {
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);

        _vertices.Add(new Vector3(-0.5f + x, -0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, -0.5f + y, -0.5f + z));
        _vertices.Add(new Vector3(0.5f + x, -0.5f + y, 0.5f + z));
        _vertices.Add(new Vector3(-0.5f + x, -0.5f + y, 0.5f + z));

        Block block = _map[x, y, z];
        Vector2 orgUv = new Vector2(block.texture_u_bottom * _textureOffset, block.texture_v_bottom * _textureOffset);
        _uvs.Add(orgUv + new Vector2(_shrinkSize, _shrinkSize));
        _uvs.Add(orgUv + new Vector2(_textureOffset, 0) + new Vector2(-_shrinkSize, _shrinkSize));
        _uvs.Add(orgUv + new Vector2(_textureOffset, _textureOffset) + new Vector2(-_shrinkSize, -_shrinkSize));
        _uvs.Add(orgUv + new Vector2(0, _textureOffset) + new Vector2(_shrinkSize, -_shrinkSize));
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
        // 只显示矩形边界的面
        if (x >= length || y >= height || z >= width || x < 0 || y < 0 || z < 0)
        {
            //return true;
            return (GetTheoreticalBlock(new Vector3(x, y, z) + transform.position) == null);
        }

        // 显示被去除的方块产生的面
        if (_map[x, y, z] == null)
            return true;

        return false;
    }

    public static Chunk4Perlin GetChunk(int x, int y, int z)
    {
        for (int i = 0; i < _chunks.Count; i++)
        {
            Vector3 pos = new Vector3(x, y, z);
            Vector3 chunkPos = _chunks[i].transform.position;
            if (chunkPos.Equals(pos))
            {
                return _chunks[i];
            }
            if (pos.x < chunkPos.x || pos.y < chunkPos.y || pos.z < chunkPos.z
                || pos.x >= chunkPos.x + length || pos.y >= chunkPos.y + height || pos.z >= chunkPos.z + width)
            {
                continue;
            }
            return _chunks[i];
        }
        return null;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    private static List<Chunk> _chunks = new List<Chunk>();

    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();
    private List<Vector2> _uvs = new List<Vector2>();

    [Label("纹理偏移")]
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
        _map = new Block[length, height, width];
    }

    void Update()
    {
        if (!_working && !_ready)
        {
            _ready = true;
            InitEnvironment();
        }
    }

    private void InitEnvironment()
    {
        _working = true;

        _mesh = new Mesh();
        _mesh.name = "Chunck";

        StartCoroutine(CalculateMap());
    }

    /// <summary>
    /// 平原图块
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Block GetPlainBlock(Vector3 pos)
    {
        Random.InitState(seed);
        Vector3 offset = new Vector3(Random.value * 100000, Random.value * 100000, Random.value * 100000);
        float noiseX = Mathf.Abs(pos.x + offset.x) / 20;
        float noiseY = Mathf.Abs(pos.y + offset.x) / 20;
        float noiseZ = Mathf.Abs(pos.z + offset.x) / 20;

        float noiseValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);
        noiseValue += (50 - pos.y) / 5;
        noiseValue /= pos.y / 4;
        if (noiseValue > 0.2f)
        {
            return BlockMap.GetBlock("Dirt");
        }
        return null;
    }

    /// <summary>
    /// 预处理地形的函数
    /// 通过不同的算法产生地形数据
    /// </summary>
    private IEnumerator CalculateMap()
    {
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    _map[x, y, z] = GetPlainBlock(new Vector3(x, y, z));
                }
            }
        }
        yield return null;
        StartCoroutine(CalculateMesh());
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
                        //if (y < 4)
                            //continue;
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

        _working = false;
        yield return null;
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

    /// <summary>
    /// 处理uv的缝隙
    /// </summary>
    private float _shrinkSize = 0.005f;
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
        _uvs.Add(new Vector2(block.texture_u_fb * _textureOffset,
            block.texture_v_fb * _textureOffset) + new Vector2(_shrinkSize, _shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_fb * _textureOffset + _textureOffset,
            block.texture_v_fb * _textureOffset) + new Vector2(-_shrinkSize, _shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_fb * _textureOffset + _textureOffset,
            block.texture_v_fb * _textureOffset + _textureOffset) + new Vector2(-_shrinkSize, -_shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_fb * _textureOffset,
            block.texture_v_fb * _textureOffset + _textureOffset) + new Vector2(_shrinkSize, -_shrinkSize));
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
        _uvs.Add(new Vector2(block.texture_u_fb * _textureOffset,
            block.texture_v_fb * _textureOffset) + new Vector2(_shrinkSize, _shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_fb * _textureOffset + _textureOffset,
            block.texture_v_fb * _textureOffset) + new Vector2(-_shrinkSize, _shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_fb * _textureOffset + _textureOffset,
            block.texture_v_fb * _textureOffset + _textureOffset) + new Vector2(-_shrinkSize, -_shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_fb * _textureOffset,
            block.texture_v_fb * _textureOffset + _textureOffset) + new Vector2(_shrinkSize, -_shrinkSize));
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
        _uvs.Add(new Vector2(block.texture_u_lr * _textureOffset,
            block.texture_v_lr * _textureOffset) + new Vector2(_shrinkSize, _shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_lr * _textureOffset + _textureOffset,
            block.texture_v_lr * _textureOffset) + new Vector2(_shrinkSize, _shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_lr * _textureOffset + _textureOffset,
            block.texture_v_lr * _textureOffset + _textureOffset) + new Vector2(-_shrinkSize, -_shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_lr * _textureOffset,
            block.texture_v_lr * _textureOffset + _textureOffset) + new Vector2(_shrinkSize, -_shrinkSize));
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
        _uvs.Add(new Vector2(block.texture_u_lr * _textureOffset,
            block.texture_v_lr * _textureOffset) + new Vector2(_shrinkSize, _shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_lr * _textureOffset + _textureOffset,
            block.texture_v_lr * _textureOffset) + new Vector2(-_shrinkSize, _shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_lr * _textureOffset + _textureOffset,
            block.texture_v_lr * _textureOffset + _textureOffset) + new Vector2(-_shrinkSize, -_shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_lr * _textureOffset,
            block.texture_v_lr * _textureOffset + _textureOffset) + new Vector2(_shrinkSize, -_shrinkSize));
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
        _uvs.Add(new Vector2(block.texture_u_top * _textureOffset,
            block.texture_v_top * _textureOffset) + new Vector2(_shrinkSize, _shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_top * _textureOffset + _textureOffset,
            block.texture_v_top * _textureOffset) + new Vector2(-_shrinkSize, _shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_top * _textureOffset + _textureOffset,
            block.texture_v_top * _textureOffset + _textureOffset) + new Vector2(-_shrinkSize, -_shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_top * _textureOffset,
            block.texture_v_top * _textureOffset + _textureOffset) + new Vector2(_shrinkSize, -_shrinkSize));
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
        _uvs.Add(new Vector2(block.texture_u_bottom * _textureOffset,
            block.texture_v_bottom * _textureOffset) + new Vector2(_shrinkSize, _shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_bottom * _textureOffset + _textureOffset,
            block.texture_v_bottom * _textureOffset) + new Vector2(-_shrinkSize, _shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_bottom * _textureOffset + _textureOffset,
            block.texture_v_bottom * _textureOffset + _textureOffset) + new Vector2(-_shrinkSize, -_shrinkSize));
        _uvs.Add(new Vector2(block.texture_u_bottom * _textureOffset,
            block.texture_v_bottom * _textureOffset + _textureOffset) + new Vector2(_shrinkSize, -_shrinkSize));
    }
    #endregion

    /// <summary>
    /// 关键的优化方法，用于检测当前位置的面是否透明(不存在)
    /// 透明的面返回true，则可使后面的面显示
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool IsBlockTransparent(int x, int y, int z)
    {
        if (x >= length || y >= height || z >= width || x < 0 || y < 0 || z < 0)
        {
            if (GetPlainBlock(transform.position + new Vector3(x, y, z)) == null)
                return true;
            else
                return false;
        }
            

        if (_map[x, y, z] == null)
            return true;

        return false;
    }

    public static Chunk GetChunk(int x, int y, int z)
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

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using LibNoise;
using LibNoise.Generator;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk8Stone : MonoBehaviour
{
    // 单个chunk的模型数据
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

    // 控制加载图快
    public static bool working;
    [HideInInspector]
    public bool ready;

    // 图块的格子坐标
    public int chunkX;
    public int chunkY;
    public int chunkZ;

    public static int seed;

    // 周围的图块
    private Chunk8Stone _topChunk;
    private Chunk8Stone _bottomChunk;
    private Chunk8Stone _rightChunk;
    private Chunk8Stone _leftChunk;
    private Chunk8Stone _frontChunk;
    private Chunk8Stone _backChunk;

    /// <summary>
    /// 保存自身的位置
    /// 在线程中使用
    /// </summary>
    private Vector3 _selfPos;

    public void Init(int chunkX, int chunkY, int chunkZ)
    {
        gameObject.name = "[" + chunkX + "," + chunkY + "," + chunkZ + "]";
        this.chunkX = chunkX;
        this.chunkY = chunkY;
        this.chunkZ = chunkZ;

        _map = new Block[length, height, width];
        _selfPos = transform.position;
    }

    private static Thread _threadMap;

    public IEnumerator CreateMap()
    {
        if (_threadMap == null || !_threadMap.IsAlive)
        {
            working = true;
            _threadMap = new Thread(CalcuateMap);
            _threadMap.Start();
            while (_threadMap.IsAlive)
                yield return null;

            _threadMap = null;
            StartCoroutine(CreateMesh());
        }
    }

    /// <summary>
    /// 预处理地形的函数
    /// 通过不同的算法产生地形数据
    /// </summary>
    private void CalcuateMap()
    {
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    Block block = GetTheoreticalBlock(new Vector3(x, y, z) + _selfPos);
                    if (block != null)
                    {
                        if (GetTheoreticalBlock(new Vector3(x, y + 1, z) + _selfPos) == null &&
                            block == BlockMap.GetBlock("Dirt"))
                        {
                            _map[x, y, z] = BlockMap.GetBlock("Grass");
                        }
                        else
                        {
                            _map[x, y, z] = block;
                        }
                    }
                }
            }
        }
    }

    Perlin noise = new Perlin(1.0f, 0.2f, 0.2f, 8, seed, QualityMode.High);
    public Block GetTheoreticalBlock(Vector3 pos)
    {
        System.Random random = new System.Random(seed);
        Vector3 offset = new Vector3((float)random.NextDouble() * 100000, 
            (float)random.NextDouble() * 100000, (float)random.NextDouble() * 100000);
        float noiseX = Mathf.Abs(pos.x + offset.x) / 20;
        float noiseY = Mathf.Abs(pos.y + offset.y) / 20;
        float noiseZ = Mathf.Abs(pos.z + offset.z) / 20;
        //float noiseValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);
        double noiseValue = noise.GetValue(noiseX, noiseY, noiseZ);
        noiseValue += (200.0f - pos.y) / 18;
        noiseValue /= pos.y / 19.0f;

        if (noiseValue > 0.5f)
        {
            if (noiseValue > 0.6f)
            {
                return BlockMap.GetBlock("Stone");
            }
            return BlockMap.GetBlock("Dirt");
        }
        return null;
    }

    /// <summary>
    /// 构建网格
    /// </summary>
    /// <returns></returns>
    private IEnumerator CreateMesh()
    {
        _mesh = new Mesh();
        _mesh.name = "Chunk";

        _threadMap = new Thread(CalculateMesh);
        _threadMap.Start();
        while (_threadMap.IsAlive)
            yield return null;
        _threadMap = null;

        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.uv = _uvs.ToArray();

        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = _mesh;
        GetComponent<MeshFilter>().mesh = _mesh;

        yield return new WaitForEndOfFrame();
        working = false;
        ready = true;
    }

    private void CalculateMesh()
    {
        _vertices.Clear();
        _triangles.Clear();
        _uvs.Clear();

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
    }

    /// <summary>
    /// 重建网格
    /// </summary>
    /// <returns></returns>
    private IEnumerator RebuildMesh()
    {
        _mesh = new Mesh();
        _mesh.name = "Chunk";

        _vertices.Clear();
        _triangles.Clear();
        _uvs.Clear();

        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    if (_map[x, y, z] != null)
                    {
                        RebuildCube(x, y, z);
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
    }

    /// <summary>
    /// 加载周围的图块
    /// </summary>
    public void LoadAround()
    {
        // 左边
        if (_leftChunk == null)
        {
            if (!ChunkMgr8Stone.Instance().IsInPreLoadRange(
                new Vector3((chunkX - 1) * Chunk.length, chunkY * Chunk.height, chunkZ * Chunk.width)))
            {
                return;
            }

            _leftChunk = ChunkMgr8Stone.GetChunkByChunkPos(new Vector3(chunkX - 1, chunkY, chunkZ));
            if (_leftChunk == null)
                ChunkMgr8Stone.Instance().AddChunk(chunkX - 1, chunkY, chunkZ);
        }
        // 右边
        if (_rightChunk == null)
        {
            if (!ChunkMgr8Stone.Instance().IsInPreLoadRange(
               new Vector3((chunkX + 1) * Chunk.length, chunkY * Chunk.height, chunkZ * Chunk.width)))
            {
                return;
            }

            _rightChunk = ChunkMgr8Stone.GetChunkByChunkPos(new Vector3(chunkX + 1, chunkY, chunkZ));
            if (_rightChunk == null)
                ChunkMgr8Stone.Instance().AddChunk(chunkX + 1, chunkY, chunkZ);
        }
        // 前面
        if (_frontChunk == null)
        {
            if (!ChunkMgr8Stone.Instance().IsInPreLoadRange(
               new Vector3(chunkX * Chunk.length, chunkY * Chunk.height, (chunkZ - 1) * Chunk.width)))
            {
                return;
            }

            _frontChunk = ChunkMgr8Stone.GetChunkByChunkPos(new Vector3(chunkX, chunkY, chunkZ - 1));
            if (_frontChunk == null)
                ChunkMgr8Stone.Instance().AddChunk(chunkX, chunkY, chunkZ - 1);
        }
        // 后面
        if (_backChunk == null)
        {
            if (!ChunkMgr8Stone.Instance().IsInPreLoadRange(
               new Vector3(chunkX * Chunk.length, chunkY * Chunk.height, (chunkZ + 1) * Chunk.width)))
            {
                return;
            }

            _backChunk = ChunkMgr8Stone.GetChunkByChunkPos(new Vector3(chunkX, chunkY, chunkZ + 1));
            if (_backChunk == null)
                ChunkMgr8Stone.Instance().AddChunk(chunkX + 1, chunkY, chunkZ + 1);
        }
        // 上面
        if (_topChunk == null)
        {
            if (!ChunkMgr8Stone.Instance().IsInPreLoadRange(
               new Vector3(chunkX * Chunk.length, (chunkY + 1) * Chunk.height, chunkZ * Chunk.width)))
            {
                return;
            }

            _topChunk = ChunkMgr8Stone.GetChunkByChunkPos(new Vector3(chunkX, chunkY + 1, chunkZ));
            if (_topChunk == null)
                ChunkMgr8Stone.Instance().AddChunk(chunkX, chunkY + 1, chunkZ);
        }
        // 下面
        if (_bottomChunk == null)
        {
            if (!ChunkMgr8Stone.Instance().IsInPreLoadRange(
               new Vector3(chunkX * Chunk.length, (chunkY - 1) * Chunk.height, chunkZ * Chunk.width)))
            {
                return;
            }

            _bottomChunk = ChunkMgr8Stone.GetChunkByChunkPos(new Vector3(chunkX, chunkY - 1, chunkZ));
            if (_bottomChunk == null)
                ChunkMgr8Stone.Instance().AddChunk(chunkX, chunkY - 1, chunkZ);
        }
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

    private void RebuildCube(int x, int y, int z)
    {
        if (IsRebuildBlockTransparent(x, y, z - 1))
            AddCubeFront(x, y, z);
        if (IsRebuildBlockTransparent(x, y, z + 1))
            AddCubeBack(x, y, z);
        if (IsRebuildBlockTransparent(x - 1, y, z))
            AddCubeLeft(x, y, z);
        if (IsRebuildBlockTransparent(x + 1, y, z))
            AddCubeRight(x, y, z);
        if (IsRebuildBlockTransparent(x, y + 1, z))
            AddCubeTop(x, y, z);
        if (IsRebuildBlockTransparent(x, y - 1, z))
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
    /// <returns>是否显示</returns>
    private bool IsBlockTransparent(int x, int y, int z)
    {
        // 只显示矩形边界的面
        if (x >= length || y >= height || z >= width || x < 0 || y < 0 || z < 0)
        {
            return (GetTheoreticalBlock(new Vector3(x, y, z) + _selfPos) == null);
        }

        // 显示被去除的方块产生的面
        if (_map[x, y, z] == null)
            return true;

        return false;
    }

    /// <summary>
    /// 控制重建网格后，显示的面
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>是否显示</returns>
    private bool IsRebuildBlockTransparent(int x, int y, int z)
    {
        #region 检查周围是否有图块，没有则显示该面
        Vector3 worldPos = DataUtil.FloorToInt(new Vector3(x, y, z) + transform.position);
        // 右边
        if (x >= length)
        {
            if (_rightChunk == null)
                _rightChunk = ChunkMgr8Stone.GetChunkByChunkPos(chunkX + 1, chunkY, chunkZ);
            if (_rightChunk != null && _rightChunk != this && _rightChunk.ready)
                return _rightChunk.GetBlock(worldPos) == null;

            return true;
        }

        // 左边
        if (x < 0)
        {
            if (_leftChunk == null)
                _leftChunk = ChunkMgr8Stone.GetChunkByChunkPos(chunkX - 1, chunkY, chunkZ);
            if (_leftChunk != null && _leftChunk != this && _leftChunk.ready)
                return _leftChunk.GetBlock(worldPos) == null;

            return true;
        }

        // 前面
        if (z < 0)
        {
            if (_frontChunk == null)
                _frontChunk = ChunkMgr8Stone.GetChunkByWorldPos(worldPos);
            if (_frontChunk != null && _frontChunk != this && _frontChunk.ready)
                return _frontChunk.GetBlock(worldPos) == null;

            return true;
        }

        // 后面
        if (z >= width)
        {
            if (_backChunk == null)
                _backChunk = ChunkMgr8Stone.GetChunkByWorldPos(worldPos);
            if (_backChunk != null && _backChunk != this && _backChunk.ready)
                return _backChunk.GetBlock(worldPos) == null;

            return true;
        }

        // 上面
        if (y >= height)
        {
            if (_topChunk == null)
                _topChunk = ChunkMgr8Stone.GetChunkByWorldPos(worldPos);
            if (_topChunk != null && _topChunk != this && _topChunk.ready)
                return _topChunk.GetBlock(worldPos) == null;

            return true;
        }

        // 下面
        if (y < 0)
        {
            if (_bottomChunk == null)
                _bottomChunk = ChunkMgr8Stone.GetChunkByWorldPos(worldPos);
            if (_bottomChunk != null && _bottomChunk != this && _bottomChunk.ready)
                return _bottomChunk.GetBlock(worldPos) == null;

            return true;
        }
        #endregion

        if (_map[x, y, z] == null)
            return true;

        return false;
    }

    public Block GetBlock(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - transform.position;
        return _map[Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.y), Mathf.FloorToInt(localPos.z)];
    }

    public void SetBlock(Vector3 pos, Block block)
    {
        Vector3 localPos = pos - transform.position;
        int blockX = Mathf.CeilToInt(localPos.x);
        int blockY = Mathf.CeilToInt(localPos.y);
        int blockZ = Mathf.CeilToInt(localPos.z);
        //print("pos: " + pos.x + ", " + pos.y + ", " + pos.z);
        //print("local pos: " + blockX + ", " + blockY + ", " + blockZ);
        _map[blockX, blockY, blockZ] = block;

        StartCoroutine(RebuildMesh());
        if (block != null)
            return;

        #region 重构相邻的图块，补充未显示的面
        // 右边
        if (blockX == length - 1)
        {
            if (_rightChunk == null)
                _rightChunk = ChunkMgr8Stone.GetChunkByChunkPos(blockX + 1, blockY, blockZ);
            StartCoroutine(_rightChunk.RebuildMesh());
            //Debug.Log("rihgt : " + _rightChunk.name);
        }
        // 左边
        if (blockX == 0)
        {
            if (_leftChunk == null)
                _leftChunk = ChunkMgr8Stone.GetChunkByChunkPos(chunkX - 1, chunkY, chunkZ);
            StartCoroutine(_leftChunk.RebuildMesh());
            //Debug.Log("left : " + _leftChunk.name);
        }
        // 前面
        if (blockZ == 0)
        {
            if (_frontChunk == null)
                _frontChunk = ChunkMgr8Stone.GetChunkByChunkPos(blockX, blockY, blockZ - 1);
            StartCoroutine(_frontChunk.RebuildMesh());
            //Debug.Log("front : " + _frontChunk.name);
        }
        // 后面
        if (blockZ == width - 1)
        {
            if (_backChunk == null)
                _backChunk = ChunkMgr8Stone.GetChunkByChunkPos(blockX, blockY, blockZ + 1);
            StartCoroutine(_backChunk.RebuildMesh());
            //Debug.Log("back : " + _backChunk.name);
        }
        // 上面
        if (blockY == height - 1)
        {
            if (_topChunk == null)
                _topChunk = ChunkMgr8Stone.GetChunkByChunkPos(blockX, blockY + 1, blockZ);
            StartCoroutine(_topChunk.RebuildMesh());
            //Debug.Log("top : " + _topChunk.name);
        }
        // 下面
        if (blockY == 0)
        {
            if (_bottomChunk == null)
                _bottomChunk = ChunkMgr8Stone.GetChunkByWorldPos(blockX, blockY - 1, blockZ);
            StartCoroutine(_bottomChunk.RebuildMesh());
            //Debug.Log("bottom : " + _bottomChunk.name);
        }
        #endregion
    }

    public Vector3 GetChunkPos()
    {
        return new Vector3(chunkX, chunkY, chunkZ);
    }
}
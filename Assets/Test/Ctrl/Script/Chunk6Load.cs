using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk6Load : MonoBehaviour
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
    private Chunk6Load _topChunk;
    private Chunk6Load _bottomChunk;
    private Chunk6Load _rightChunk;
    private Chunk6Load _leftChunk;
    private Chunk6Load _frontChunk;
    private Chunk6Load _backChunk;

    public void Init(int chunkX, int chunkY, int chunkZ)
    {
        gameObject.name = "[" + chunkX + "," + chunkY + "," + chunkZ + "]";
        this.chunkX = chunkX;
        this.chunkY = chunkY;
        this.chunkZ = chunkZ;
    }

    public void CreateMap()
    {
        CalculateMap();
    }

    /// <summary>
    /// 预处理地形的函数
    /// 通过不同的算法产生地形数据
    /// </summary>
    private void CalculateMap()
    {
        _map = new Block[length, height, width];
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    Block block = GetTheoreticalBlock(new Vector3(x, y, z) + transform.position);
                    if (block != null)
                    {
                        if (GetTheoreticalBlock(new Vector3(x, y + 1, z) + transform.position) == null)
                            _map[x, y, z] = BlockMap.GetBlock("Grass");
                        else
                            _map[x, y, z] = BlockMap.GetBlock("Dirt");
                    }
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
        working = false;
        ready = true;
    }

    private bool _rebuildWorking;
    private IEnumerator RebuildMesh()
    {
        _rebuildWorking = true;
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
        _rebuildWorking = false;
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
            //return true;
            return (GetTheoreticalBlock(new Vector3(x, y, z) + transform.position) == null);
        }

        // 显示被去除的方块产生的面
        if (_map[x, y, z] == null)
            return true;

        return false;
    }

    /// <summary>
    /// 修改图块后，显示的面
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
                _rightChunk = ChunkMgr6Load.GetChunkByChunkPos(chunkX + 1, chunkY, chunkZ);
            if (_rightChunk != null && _rightChunk != this && _rightChunk.ready)
                return _rightChunk.GetBlock(worldPos) == null;

            return true;
        }

        // 左边
        if (x < 0)
        {
            if (_leftChunk == null)
                _leftChunk = ChunkMgr6Load.GetChunkByChunkPos(chunkX - 1, chunkY, chunkZ);
            if (_leftChunk != null && _leftChunk != this && _leftChunk.ready)
                return _leftChunk.GetBlock(worldPos) == null;

            return true;
        }

        // 前面
        if (z < 0)
        {
            if (_frontChunk == null)
                _frontChunk = ChunkMgr6Load.GetChunkByWorldPos(worldPos);
            if (_frontChunk != null && _frontChunk != this && _frontChunk.ready)
                return _frontChunk.GetBlock(worldPos) == null;

            return true;
        }

        // 后面
        if (z >= width)
        {
            if (_backChunk == null)
                _backChunk = ChunkMgr6Load.GetChunkByWorldPos(worldPos);
            if (_backChunk != null && _backChunk != this && _backChunk.ready)
                return _backChunk.GetBlock(worldPos) == null;

            return true;
        }

        // 上面
        if (y >= height)
        {
            if (_topChunk == null)
                _topChunk = ChunkMgr6Load.GetChunkByWorldPos(worldPos);
            if (_topChunk != null && _topChunk != this && _topChunk.ready)
                return _topChunk.GetBlock(worldPos) == null;

            return true;
        }

        // 下面
        if (y < 0)
        {
            if (_bottomChunk == null)
                _bottomChunk = ChunkMgr6Load.GetChunkByWorldPos(worldPos);
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
        int blockX = Mathf.FloorToInt(localPos.x);
        int blockY = Mathf.FloorToInt(localPos.y);
        int blockZ = Mathf.FloorToInt(localPos.z);
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
                _rightChunk = ChunkMgr6Load.GetChunkByChunkPos(blockX + 1, blockY, blockZ);
            StartCoroutine(_rightChunk.RebuildMesh());
            //Debug.Log("rihgt : " + _rightChunk.name);
        }
        // 左边
        if (blockX == 0)
        {
            if (_leftChunk == null)
                _leftChunk = ChunkMgr6Load.GetChunkByChunkPos(chunkX - 1, chunkY, chunkZ);
            StartCoroutine(_leftChunk.RebuildMesh());
            //Debug.Log("left : " + _leftChunk.name);
        }
        // 前面
        if (blockZ == 0)
        {
            if (_frontChunk == null)
                _frontChunk = ChunkMgr6Load.GetChunkByChunkPos(blockX, blockY, blockZ - 1);
            StartCoroutine(_frontChunk.RebuildMesh());
            //Debug.Log("front : " + _frontChunk.name);
        }
        // 后面
        if (blockZ == width - 1)
        {
            if (_backChunk == null)
                _backChunk = ChunkMgr6Load.GetChunkByChunkPos(blockX, blockY, blockZ + 1);
            StartCoroutine(_backChunk.RebuildMesh());
            //Debug.Log("back : " + _backChunk.name);
        }
        // 上面
        if (blockY == height - 1)
        {
            if (_topChunk == null)
                _topChunk = ChunkMgr6Load.GetChunkByChunkPos(blockX, blockY + 1, blockZ);
            StartCoroutine(_topChunk.RebuildMesh());
            //Debug.Log("top : " + _topChunk.name);
        }
        // 下面
        if (blockY == 0)
        {
            if (_bottomChunk == null)
                _bottomChunk = ChunkMgr6Load.GetChunkByWorldPos(blockX, blockY - 1, blockZ);
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
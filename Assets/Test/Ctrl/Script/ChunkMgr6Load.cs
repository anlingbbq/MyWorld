using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMgr6Load : MonoBehaviour
{
    [Label("玩家")]
    public Transform player;
    [Label("图快预制体")]
    [SerializeField]
    private GameObject _chunkPrefab;
    [SerializeField]
    [Label("点击显示的高亮预制体")]
    private GameObject _hightBlock;

    [SerializeField]
    [Label("是否预加载图块")]
    private bool _preload = true;
    /// <summary>
    /// 加载范围以玩家为中心点的正方向边长的一半
    /// </summary>
    [SerializeField]
    [Header("预加载图块范围")]
    [Range(0, 100)]
    private int _preloadRange = 60;

    [Label("最大图块高度")]
    public int _maxHigh = 8;

    /// <summary>
    /// 保存所有chunk
    /// </summary>
    [HideInInspector]
    private static List<Chunk6Load> _chunks = new List<Chunk6Load>();

    private static ChunkMgr6Load _instance;
    public static ChunkMgr6Load Instance()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        if (_preload)
        {
            for (float x = player.position.x - _preloadRange; x < player.position.x + _preloadRange; x += Chunk.length)
            {
                int chunkX = Mathf.FloorToInt(x / Chunk.length);
                for (float z = player.position.z - _preloadRange; z < player.position.z + _preloadRange; z += Chunk.width)
                {
                    int chunkZ = Mathf.FloorToInt(z / Chunk.width);
                    for (float y = player.position.y - _preloadRange; y < player.position.y + _preloadRange; y += Chunk.height)
                    {
                        int chunkY = Mathf.FloorToInt(y / Chunk.height);
                        if (GetChunkByChunkPos(chunkX, chunkY, chunkZ) == null)
                        {
                            GameObject go = Instantiate(_chunkPrefab, new Vector3(
                                chunkX * Chunk.length, chunkY * Chunk.width, chunkZ * Chunk.height), Quaternion.identity);
                            Chunk6Load chunk = go.GetComponent<Chunk6Load>();
                            chunk.Init(chunkX, chunkY, chunkZ);
                            _chunks.Add(chunk);
                        }
                    }
                }
            }

            SpawnChunk();
        }

        DestoryChunk();
        BlockContrller();
    }

    private void SpawnChunk()
    {
        if (Chunk6Load.working)
            return;

        float lastDis = 99999999;
        Chunk6Load target = null;
        for (int i = 0; i < _chunks.Count; i++)
        {
            Chunk6Load chunk = _chunks[i];
            float dis = Vector3.Distance(chunk.transform.position, player.position);
            if (dis < lastDis)
            {
                if (chunk.ready == false)
                {
                    lastDis = dis;
                    target = chunk;
                }
            }
        }

        if (target != null)
        {
            target.CreateMap();
        }
    }

    private void DestoryChunk()
    {
        for (int i = _chunks.Count - 1; i >= 0; i--)
        {
            Chunk6Load chunk = _chunks[i];
            float dis = Vector3.Distance(chunk.transform.position, player.position);
            if (dis > (_preloadRange * 2 + Chunk.width))
            {
                _chunks.Remove(chunk);
                Destroy(chunk.gameObject);
            }
        }
    }

    private void BlockContrller()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 10f))
        {
            Vector3 pos = hitInfo.point - hitInfo.normal / 2;
            //Vector3 pos = new Vector3(hitX, hitY, hitZ);
            _hightBlock.transform.position = DataUtil.CeilToInt(pos);

            if (Input.GetMouseButton(0))
            {
                Chunk6Load chunk = GetChunkByWorldPos(DataUtil.CeilToInt(pos));
                chunk.SetBlock(pos, null);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                pos = hitInfo.point + hitInfo.normal / 2;
                Chunk6Load chunk = GetChunkByWorldPos(DataUtil.CeilToInt(pos));
                chunk.SetBlock(pos, BlockMap.GetBlock("TNT"));
            }
        }
        else
        {
            _hightBlock.transform.position = new Vector3(10000, 10000, 10000);
        }
    }

    public static Chunk6Load GetChunkByWorldPos(int x, int y, int z)
    {
        Vector3 pos = new Vector3(x, y, z);
        return GetChunkByWorldPos(pos);
    }

    public static Chunk6Load GetChunkByWorldPos(Vector3 pos)
    {
        for (int i = 0; i < _chunks.Count; i++)
        {
            Vector3 chunkPos = _chunks[i].transform.position;
            if (chunkPos.Equals(pos))
                return _chunks[i];

            if (pos.x < chunkPos.x || pos.y < chunkPos.y || pos.z < chunkPos.z
                || pos.x >= chunkPos.x + Chunk.length || pos.y >= chunkPos.y + Chunk.height || pos.z >= chunkPos.z + Chunk.width)
                continue;

            return _chunks[i];
        }
        return null;
    }

    public static Chunk6Load GetChunkByChunkPos(int x, int y, int z)
    {
        Vector3 pos = new Vector3(x, y, z);
        return GetChunkByChunkPos(pos);
    }

    public static Chunk6Load GetChunkByChunkPos(Vector3 pos)
    {
        for (int i = 0; i < _chunks.Count; i++)
        {
            Vector3 chunkPos = _chunks[i].GetChunkPos();
            if (chunkPos.Equals(pos))
                return _chunks[i];
        }
        return null;
    }
}
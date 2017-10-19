using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMgr4Perlin : MonoBehaviour
{
    [Label("玩家")]
    [SerializeField]
    private Transform _player;
    [Label("图快预制体")]
    [SerializeField]
    private GameObject _chunkPrefab;
    /// <summary>
    /// 加载范围以玩家为中心点的正方向边长
    /// </summary>
    [Label("加载图块的范围")]
    [SerializeField]
    private int _loadRange = 60;

    public int _maxHigh = 8;

    private void Update()
    {
        int halfRange = _loadRange / 2;
        for (float x = _player.position.x - halfRange; x < _player.position.x + halfRange; x += Chunk.length)
        {
            int posX = Mathf.FloorToInt(x / Chunk.length) * Chunk.length;
            for (float z = _player.position.z - halfRange; z < _player.position.z + halfRange; z += Chunk.width)
            {
                int posZ = Mathf.FloorToInt(z / Chunk.width) * Chunk.width;
                for (int y = 0; y < Chunk.height * _maxHigh; y += Chunk.height)
                {
                    Chunk4Perlin chunk = Chunk4Perlin.GetChunk(posX, y, posZ);
                    if (chunk == null)
                    {
                        Instantiate(_chunkPrefab, new Vector3(posX, y, posZ), Quaternion.identity);
                    }
                }
            }
        }
    }
}

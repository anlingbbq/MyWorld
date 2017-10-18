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

    private void Update()
    {
        int halfRange = _loadRange / 2;
        for (float x = _player.position.x - halfRange; x < _player.position.x + halfRange; x += Chunk.length)
        {
            for (float z = _player.position.z - halfRange; z < _player.position.z + halfRange; z += Chunk.width)
            {
                int chunkX = Mathf.FloorToInt(x / Chunk.length) * Chunk.length;
                int chunkZ = Mathf.FloorToInt(z / Chunk.width) * Chunk.width;

                Chunk4Perlin chunk = Chunk4Perlin.GetChunk(chunkX, 0, chunkZ);
                if (chunk == null)
                {
                    Instantiate(_chunkPrefab, new Vector3(chunkX, 0, chunkZ), Quaternion.identity);
                }
            }
        }
    }
}

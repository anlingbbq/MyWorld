using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMgr3Texture2D : MonoBehaviour
{
    [Label("玩家")]
    [SerializeField]
    private Transform _player;
    [Label("图快预制体")]
    [SerializeField]
    private GameObject _chunkPrefab;
    [Label("加载图块的范围")]
    [SerializeField]
    private int _loadRange = 30;

    private void Update()
    {
        for (float x = _player.position.x - _loadRange; x < _player.position.x + _loadRange; x += Chunk.length)
        {
            for (float z = _player.position.z - _loadRange; z < _player.position.z + _loadRange; z += Chunk.width)
            {
                int chunkX = Mathf.FloorToInt(x / Chunk.length) * Chunk.length;
                int chunkZ = Mathf.FloorToInt(z / Chunk.width) * Chunk.width;

                //Chunk3TextureCenter chunk = Chunk3TextureCenter.GetChunk(Mathf.FloorToInt(chunkX), 0, Mathf.FloorToInt(chunkZ));
                //if (chunk == null)
                //{
                //    Instantiate(_chunkPrefab, new Vector3(chunkX, 0, chunkZ), Quaternion.identity);
                //}
            }
        }
    }
}

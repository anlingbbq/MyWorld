using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject _chunkPrefab;
    [SerializeField]
    private int _viewRange = 30;

    private void Update()
    {
        for (float x = transform.position.x - _viewRange; x < transform.position.x + _viewRange; x += Chunk.length)
        {
            for (float z = transform.position.z - _viewRange; z < transform.position.z + _viewRange; z += Chunk.width)
            {
                int xx = Mathf.FloorToInt(x / Chunk.length) * Chunk.length;
                int zz = Mathf.FloorToInt(z / Chunk.width) * Chunk.width;
                Chunk chunk = Chunk.GetChunk(Mathf.FloorToInt(xx), 0, Mathf.FloorToInt(zz));
                if (chunk == null)
                {
                    Instantiate(_chunkPrefab, new Vector3(xx, 0, zz), Quaternion.identity);
                }
            }
        }
    }
}

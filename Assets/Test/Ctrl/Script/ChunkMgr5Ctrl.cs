using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMgr5Ctrl : MonoBehaviour
{
    [Label("玩家")]
    [SerializeField]
    private Transform _player;
    [Label("图快预制体")]
    [SerializeField]
    private GameObject _chunkPrefab;
    [SerializeField]
    [Label("点击显示的高亮预制体")]
    private GameObject _hightBlock;
    /// <summary>
    /// 加载范围以玩家为中心点的正方向边长
    /// </summary>
    [Label("加载图块的范围")]
    [SerializeField]
    private int _loadRange = 60;
    [SerializeField]
    private bool _reLoad = true;

    public int _maxHigh = 8;

    private void Update()
    {
        if (_reLoad)
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
                        Chunk5Ctrl chunk = Chunk5Ctrl.GetChunk(posX, y, posZ);
                        if (chunk == null)
                        {
                            Instantiate(_chunkPrefab, new Vector3(posX, y, posZ), Quaternion.identity);
                        }
                    }
                }
            }
        }

        BlockContrllre();
    }

    private void BlockContrllre()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 10f))
        {
            Vector3 pos = hitInfo.point - hitInfo.normal / 2;
            //Vector3 pos = new Vector3(hitX, hitY, hitZ);
            _hightBlock.transform.position = new Vector3(
                Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

            if (Input.GetMouseButtonDown(0))
            {
                Chunk5Ctrl chunk = Chunk5Ctrl.GetChunk(FloorToIntVector3(pos));
                chunk.SetChunk(pos, null);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                pos = hitInfo.point + hitInfo.normal / 2;
                Chunk5Ctrl chunk = Chunk5Ctrl.GetChunk(FloorToIntVector3(pos));
                chunk.SetChunk(pos, BlockMap.GetBlock("TNT"));
            }
        }
        else
        {
            _hightBlock.transform.position = new Vector3(10000, 10000, 10000);
        }
    }

    private Vector3 FloorToIntVector3(Vector3 pos)
    {
        return new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
    }
}
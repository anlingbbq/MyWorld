using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMap : MonoBehaviour
{
    private static Dictionary<string, Block> _blockDict = new Dictionary<string, Block>();

    private void Awake()
    {
        InitBlockMap();
    }

    private void InitBlockMap()
    {
        Block dirt = new Block("Dirt", 2, 15, 2, 15, 2, 15, 2, 15);
        AddBlock(dirt);

        Block grass = new Block("Grass", 3, 15, 0, 15, 2, 15);
        AddBlock(grass);

        Block tnt = new Block("TNT", 8, 15, 8, 15, 8, 15);
        AddBlock(tnt);

        Block testDir = new Block("TestDir", 6, 7, 6, 7, 6, 7);
        AddBlock(testDir);

        Block test = new Block("Test", 7, 8, 7, 8, 7, 8);
        AddBlock(test);
    }

    private void AddBlock(Block block)
    {
        if (!_blockDict.ContainsKey(block.name))
            _blockDict.Add(block.name, block);
    }

    public static Block GetBlock(string name)
    {
        Block block = null;
        _blockDict.TryGetValue(name, out block);
        return block;
    }
}

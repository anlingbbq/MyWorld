using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMap : MonoBehaviour
{
    private static Dictionary<string, Block> _blockDict = new Dictionary<string, Block>();

    void Awake()
    {
        Block dirt = new Block("Dirt", 2, 15, 2, 15, 2, 15);
        _blockDict.Add(dirt.name, dirt);
    }

    public static Block GetBlock(string name)
    {
        Block block = null;
        _blockDict.TryGetValue(name, out block);
        return block;
    }
}

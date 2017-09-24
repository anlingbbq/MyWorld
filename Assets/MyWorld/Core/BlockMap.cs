﻿using System.Collections;
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
        _blockDict.Add(dirt.name, dirt);

        Block grass = new Block("Grass", 3, 15, 0, 15, 2, 15);
        _blockDict.Add(grass.name, grass);
    }

    public static Block GetBlock(string name)
    {
        Block block = null;
        _blockDict.TryGetValue(name, out block);
        return block;
    }
}
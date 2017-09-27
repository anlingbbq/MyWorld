using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinText : MonoBehaviour
{
    public int width = 4;
    public int lenght = 128;
    public int heightScale = 20;
    public float detailScale = 25;

    public GameObject block;

    void Start()
    {
        for (int x = 0; x <= lenght; x++)
        {
            for (int z = 0; z <= width; z++)
            {
                int y = (int)(Mathf.PerlinNoise(x / detailScale, z / detailScale) * heightScale);
                Vector3 pos = new Vector3(x, y, z);
                Instantiate(block, pos, Quaternion.identity);
            }
        }
    }

    void Update()
    {
    }
}

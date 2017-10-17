using UnityEngine;

public class PerlinBase : MonoBehaviour
{
    public int x;
    public int y;
    public int z;

    public float scale = 20;

    private void Update()
    {
        // 1.柏林噪音的正值和负值一样
        // 2.相同的参数返回的值一样
        Debug.Log(Mathf.PerlinNoise(x / scale, y / scale));
        Debug.Log(SimplexNoise.Noise.Generate(x / scale, y / scale, z / scale));
    }
}
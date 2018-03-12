using LibNoise;
using LibNoise.Generator;
using UnityEngine;

public class PerlinCubeTest : MonoBehaviour
{
    public int seed;
    public int width = 16;
    public int lenght = 16;
    public int heightScale = 20;
    public float detailScale = 250;

    public GameObject block;

    void Start()
    {
        Perlin noise = new Perlin(1.0f, 0.2f, 0.2f, 8, seed, QualityMode.High);
        float scalePos = 20;
        for (int x = 0; x <= lenght; x++)
        {
            for (int y = 0; y < lenght; y++)
            {
                for (int z = 0; z <= width; z++)
                {
                    float noiseX = Mathf.Abs(x) / scalePos;
                    float noiseY = Mathf.Abs(y) / scalePos;
                    float noiseZ = Mathf.Abs(z) / scalePos;
                    double noiseValue = noise.GetValue(noiseX, noiseY, noiseZ);

                    noiseValue += (200.0f - y) / 18;
                    noiseValue /= y / 19.0f;
                    print(noiseValue);

                    if (noiseValue > 0.5f)
                    {
                        Vector3 pos = new Vector3(x, y, z);
                        Instantiate(block, pos, Quaternion.identity);
                    }
                }
            }
        }
    }
}
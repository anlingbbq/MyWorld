using UnityEngine;

public class PerlinCube : MonoBehaviour
{
    public int width = 64;
    public int lenght = 64;
    public int heightScale = 20;
    public float detailScale = 250;

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
}
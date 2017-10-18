using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunckOrg : MonoBehaviour {
    //1.用List表储存我们的顶点信息和三角面信息
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangulos = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
   
    Block[,,] map;
    public static int width = 1, height = 1;
    public float TextureOffset= 1/16f;
    Mesh mesh;

    private static bool working = false;//判定单次的协程是否完成
    private  bool ready=false;//让每一个Chunck都执行一次协程

    // Update is called once per frame
    void Update()
    {
      
        if (working == false && ready == false)
        {
            ready = true;
            StartFunction();
        }

    }


    void StartFunction() {
        working = true;
        mesh = new Mesh();
        mesh.name = "Chunck";
        map = new Block[width, height, width];
        StartCoroutine(CalculateMap());

    }

    /// <summary>
    /// 预处理地形
    /// </summary>
   IEnumerator CalculateMap() {
       
           for (int x = 0; x < width; x++) {          
            for (int y = 0; y < height; y++) {               
                for (int z = 0; z < width; z++)
                {                 
                    map[x, y, z] = BlockMap.GetBlock("TestDir");                                              
                }
            }
        }

        yield return null;

        StartCoroutine(CalculateMesh());
    }

    /// <summary>
    /// 对网格进行处理
    /// </summary>
    IEnumerator CalculateMesh()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    if (map[x, y, z] != null)
                    {
                        //if (y < 4) continue;

                        if (IsBlockTransparent(x + 1, y, z))
                        {
                            AddCubeFront(x, y, z, map[x, y, z]);
                        }
                        if (IsBlockTransparent(x - 1, y, z))
                        {
                            AddCubeBack(x, y, z, map[x, y, z]);
                        }
                        if (IsBlockTransparent(x, y, z + 1))
                        {
                            AddCubeRight(x, y, z, map[x, y, z]);
                        }
                        if (IsBlockTransparent(x, y, z - 1))
                        {
                            AddCubeLeft(x, y, z, map[x, y, z]);
                        }
                        if (IsBlockTransparent(x, y + 1, z))
                        {
                            AddCubeTop(x, y, z, map[x, y, z]);
                        }
                        if (IsBlockTransparent(x, y - 1, z))
                        {
                            AddCubeBottom(x, y, z, map[x, y, z]);
                        }

                    }
                }
            }

        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangulos.ToArray();
        mesh.uv = uvs.ToArray();
        //4.进行合并
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshFilter>().mesh = mesh;

        yield return null;
        working = false;
    }

    //2.以某种规则建立一个存放顶点信息和三角面的合理方法
    void AddCubeFront(int x, int y,int z,Block b) {


        triangulos.Add(2 + vertices.Count);
        triangulos.Add(1 + vertices.Count);
        triangulos.Add(0 + vertices.Count);
        triangulos.Add(0 + vertices.Count);
        triangulos.Add(3 + vertices.Count);
        triangulos.Add(2 + vertices.Count);

        vertices.Add(new Vector3(0 + x, 0 + y, 0 + z)); //0
        vertices.Add(new Vector3(0 + x, 0 + y, 1 + z)); //1
        vertices.Add(new Vector3(0 + x, 1 + y, 1 + z)); // 2
        vertices.Add(new Vector3(0 + x, 1 + y, 0 + z)); //3
        float ShrinkSize = 0.005f;
        uvs.Add(new Vector2((b.texture_u_fb * TextureOffset), b.texture_v_fb*TextureOffset) + new Vector2(ShrinkSize, ShrinkSize));
        uvs.Add(new Vector2((b.texture_u_fb * TextureOffset)+ TextureOffset, b.texture_v_fb * TextureOffset) + new Vector2(-ShrinkSize, ShrinkSize));
        uvs.Add(new Vector2((b.texture_u_fb * TextureOffset)+ TextureOffset, (b.texture_v_fb * TextureOffset)+TextureOffset) + new Vector2(-ShrinkSize, -ShrinkSize));
        uvs.Add(new Vector2(b.texture_u_fb * TextureOffset, (b.texture_v_fb * TextureOffset)+ TextureOffset) + new Vector2(ShrinkSize, -ShrinkSize));


    }

    void AddCubeBack(int x, int y, int z, Block b)
    {



        triangulos.Add(0 + vertices.Count);
        triangulos.Add(1 + vertices.Count);
        triangulos.Add(2 + vertices.Count);
        triangulos.Add(2 + vertices.Count);
        triangulos.Add(3 + vertices.Count);
        triangulos.Add(0 + vertices.Count);

        vertices.Add(new Vector3(-1 + x, 0+y, 0+z)); //0
        vertices.Add(new Vector3(-1 + x, 0+y, 1+z)); //1
        vertices.Add(new Vector3(-1 + x, 1+y, 1+z)); // 2
        vertices.Add(new Vector3(-1 + x, 1+y, 0+z)); //3

        float ShrinkSize = 0.005f;
        uvs.Add(new Vector2((b.texture_u_fb * TextureOffset), b.texture_v_fb * TextureOffset) + new Vector2(ShrinkSize, ShrinkSize));
        uvs.Add(new Vector2((b.texture_u_fb * TextureOffset) + TextureOffset, b.texture_v_fb * TextureOffset) + new Vector2(-ShrinkSize, ShrinkSize));
        uvs.Add(new Vector2((b.texture_u_fb * TextureOffset) + TextureOffset, (b.texture_v_fb * TextureOffset) + TextureOffset) + new Vector2(-ShrinkSize, -ShrinkSize));
        uvs.Add(new Vector2(b.texture_u_fb * TextureOffset, (b.texture_v_fb * TextureOffset) + TextureOffset) + new Vector2(ShrinkSize,- ShrinkSize));
    }

    void AddCubeRight(int x, int y, int z, Block b)
    {



        triangulos.Add(2 + vertices.Count);
        triangulos.Add(1 + vertices.Count);
        triangulos.Add(0 + vertices.Count);
        triangulos.Add(0 + vertices.Count);
        triangulos.Add(3 + vertices.Count);
        triangulos.Add(2 + vertices.Count);

        vertices.Add(new Vector3(0 + x, 0 + y, 1+z)); //0
        vertices.Add(new Vector3(-1 + x, 0 + y, 1 + z)); //1
        vertices.Add(new Vector3(-1 + x, 1 + y, 1 + z)); // 2
        vertices.Add(new Vector3(0 + x, 1 + y, 1 + z)); //3

        float ShrinkSize = 0.005f;
        uvs.Add(new Vector2((b.texture_u_lr * TextureOffset), b.texture_v_lr * TextureOffset) + new Vector2(ShrinkSize, ShrinkSize));
        uvs.Add(new Vector2((b.texture_u_lr * TextureOffset) + TextureOffset, b.texture_v_lr * TextureOffset) + new Vector2(-ShrinkSize, ShrinkSize));
        uvs.Add(new Vector2((b.texture_u_lr * TextureOffset) + TextureOffset, (b.texture_v_lr * TextureOffset) + TextureOffset) + new Vector2(-ShrinkSize, -ShrinkSize));
        uvs.Add(new Vector2(b.texture_u_lr * TextureOffset, (b.texture_v_lr * TextureOffset) + TextureOffset) + new Vector2(ShrinkSize, -ShrinkSize));
    }

    void AddCubeLeft(int x, int y, int z, Block b)
    {



        triangulos.Add(2 + vertices.Count);
        triangulos.Add(1 + vertices.Count);
        triangulos.Add(0 + vertices.Count);
        triangulos.Add(0 + vertices.Count);
        triangulos.Add(3 + vertices.Count);
        triangulos.Add(2 + vertices.Count);

        vertices.Add(new Vector3(-1 + x, 0+y, 0+z)); //0
        vertices.Add(new Vector3(0 + x, 0 + y, 0 + z)); //1
        vertices.Add(new Vector3(0 + x, 1 + y, 0 + z)); // 2
        vertices.Add(new Vector3(-1 + x, 1 + y, 0 + z)); //3
        float ShrinkSize = 0.005f;
        uvs.Add(new Vector2((b.texture_u_lr * TextureOffset), b.texture_v_lr * TextureOffset) + new Vector2(ShrinkSize, ShrinkSize));
        uvs.Add(new Vector2((b.texture_u_lr * TextureOffset) + TextureOffset, b.texture_v_lr * TextureOffset) + new Vector2(-ShrinkSize, ShrinkSize));
        uvs.Add(new Vector2((b.texture_u_lr * TextureOffset) + TextureOffset, (b.texture_v_lr * TextureOffset) + TextureOffset) + new Vector2(-ShrinkSize, -ShrinkSize));
        uvs.Add(new Vector2(b.texture_u_lr * TextureOffset, (b.texture_v_lr * TextureOffset) + TextureOffset) + new Vector2(ShrinkSize, -ShrinkSize));
    }

    void AddCubeTop(int x, int y, int z, Block b)
    {



        triangulos.Add(3 + vertices.Count);
        triangulos.Add(1 + vertices.Count);
        triangulos.Add(0 + vertices.Count);
        triangulos.Add(1 + vertices.Count);
        triangulos.Add(3 + vertices.Count);
        triangulos.Add(2 + vertices.Count);

        vertices.Add(new Vector3(0 + x, 1+y, 0 + z)); //0
        vertices.Add(new Vector3(0 + x, 1 + y, 1 + z)); //1
        vertices.Add(new Vector3(-1 + x, 1 + y, 1 + z)); // 2
        vertices.Add(new Vector3(-1 + x, 1 + y, 0 + z)); //3
        float ShrinkSize = 0.005f;
        uvs.Add(new Vector2((b.texture_u_top * TextureOffset), b.texture_v_top * TextureOffset) + new Vector2(ShrinkSize, ShrinkSize));
        uvs.Add(new Vector2((b.texture_u_top * TextureOffset) + TextureOffset, b.texture_v_top * TextureOffset) + new Vector2(-ShrinkSize, ShrinkSize));
        uvs.Add(new Vector2((b.texture_u_top * TextureOffset) + TextureOffset, (b.texture_v_top * TextureOffset) + TextureOffset) + new Vector2(-ShrinkSize, -ShrinkSize));
        uvs.Add(new Vector2(b.texture_u_top * TextureOffset, (b.texture_v_top * TextureOffset) + TextureOffset) + new Vector2(ShrinkSize, -ShrinkSize));
    }

    void AddCubeBottom(int x, int y, int z, Block b)
    {



        triangulos.Add(0 + vertices.Count);
        triangulos.Add(1 + vertices.Count);
        triangulos.Add(2 + vertices.Count);
        triangulos.Add(2 + vertices.Count);
        triangulos.Add(3 + vertices.Count);
        triangulos.Add(0 + vertices.Count);

        vertices.Add(new Vector3(0+x, 0 + y, 0+z)); //0
        vertices.Add(new Vector3(0+x, 0 + y, 1+z)); //1
        vertices.Add(new Vector3(-1+x, 0 + y, 1+z)); // 2
        vertices.Add(new Vector3(-1+x, 0 + y, 0+z)); //3
        float ShrinkSize = 0.005f;
        uvs.Add(new Vector2((b.texture_u_bottom * TextureOffset), b.texture_v_bottom * TextureOffset) + new Vector2(ShrinkSize,ShrinkSize));
        uvs.Add(new Vector2((b.texture_u_bottom * TextureOffset) + TextureOffset, b.texture_v_bottom * TextureOffset) + new Vector2(-ShrinkSize, ShrinkSize));
        uvs.Add(new Vector2((b.texture_u_bottom * TextureOffset) + TextureOffset, (b.texture_v_bottom * TextureOffset) + TextureOffset) + new Vector2(-ShrinkSize, -ShrinkSize));
        uvs.Add(new Vector2(b.texture_u_bottom * TextureOffset, (b.texture_v_bottom * TextureOffset) + TextureOffset) + new Vector2(ShrinkSize, -ShrinkSize));
    }
    /// <summary>
    /// 对生成的面进行一个限制
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool IsBlockTransparent(int x, int y, int z){
        if (x >= width || y >= height || z >= width || x < 0 || y < 0 || z < 0)
        {
            return true;
        }
        if (map[x, y, z] == null) return true;
     
        return false;
        }
}

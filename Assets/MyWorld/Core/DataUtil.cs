using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataUtil : MonoBehaviour
{
    public static Vector3 FloorToInt(Vector3 pos)
    {
        return new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
    }
}

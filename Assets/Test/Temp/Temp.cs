using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    public static int seed;

    void Start()
    {
        //Random.InitState(seed);
        for (int i = 0; i < 10; i++)
        {
            StartCoroutine(Test());
        }
    }

    private IEnumerator Test()
    {
        Random.InitState(seed);
        //print("x : " + Random.value + " y : " + Random.value + " z : " + Random.value);
        yield return null;
    }
}

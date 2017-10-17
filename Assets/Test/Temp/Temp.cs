using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Test());
    }

    private IEnumerator Test()
    {
        for (int i = 0; i < 10; i++)
        {
            print(i);
            yield return new WaitForSeconds(1);
        }
    }
}

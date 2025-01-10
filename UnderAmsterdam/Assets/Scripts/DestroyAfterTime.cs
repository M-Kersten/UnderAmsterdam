using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float time;
    void Start()
    {
        StartCoroutine(BreatheSmoke());
    }
    private IEnumerator BreatheSmoke()
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}

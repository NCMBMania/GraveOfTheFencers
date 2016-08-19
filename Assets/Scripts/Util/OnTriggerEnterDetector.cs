using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Collider))]
public class OnTriggerEnterDetector : MonoBehaviour
{
    private Collider thisCollider;

    void OnValidate()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    public Action<Collider> callBack;

    void Awake()
    {
        thisCollider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (callBack != null) callBack(col);
    }

    void OnEnable()
    {
        thisCollider.enabled = true;
    }

    void OnDisable()
    {
        thisCollider.enabled = false;
    }
}


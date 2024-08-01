using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GemOrbit : MonoBehaviour
{
    public Transform target;
    public float orbitDistance = 5f;
    public float orbitSpeed = 30f;
    public float followSpeed = 2f;
    public Vector3 heightOffset = new Vector3(0f, 1f, 0f);

    private Vector3 offset;

    void Start()
    {
        offset = new Vector3(orbitDistance, 0, 0);
    }

    private void Awake()
    {
        this.transform.position = target.position;
    }

    void Update()
    {
        Quaternion rotation = Quaternion.Euler(0, orbitSpeed * Time.deltaTime, 0);
        offset = rotation * offset;
        Vector3 desiredPosition = target.position + offset + heightOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.LookAt(target);
    }
}

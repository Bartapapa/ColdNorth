using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Targeting : MonoBehaviour
{
    [SerializeField]
    private GameObject _particles;
    [SerializeField]
    private float _particleRotationSpeed = 100f;

    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            RotateParticles();
        }
    }

    private void RotateParticles()
    {
        _particles.transform.Rotate(new Vector3(0, _particleRotationSpeed * Time.deltaTime, 0));
    }
}

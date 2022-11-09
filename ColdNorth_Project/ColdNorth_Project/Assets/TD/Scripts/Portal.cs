using GSGD1;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private GameObject _GFX;

    [SerializeField]
    private GameObject _particles;
    [SerializeField]
    private float _particleRotationSpeed = 100f;

    [SerializeField]
    private float _scale = 1f;
    [SerializeField]
    private float _scaleSpeed = 10f;


    [SerializeField]
    private bool isEndPortal = false;
    private bool _isOpened = false;

    public UnityEvent<WaveEntity, Damageable> EnemyTeleported = null;

    private void Awake()
    {
        ClosePortal();
        _GFX.SetActive(false);
        _scale = 0f;
    }

    private void OnEnable()
    {
        EnemyTeleported.RemoveListener(LevelReferences.Instance.SpawnerManager.OnEntityReachedPortal);
        EnemyTeleported.AddListener(LevelReferences.Instance.SpawnerManager.OnEntityReachedPortal);
    }

    private void OnDisable()
    {
        EnemyTeleported.RemoveListener(LevelReferences.Instance.SpawnerManager.OnEntityReachedPortal);
    }

    void Update()
    {
        RotateParticles();
    }

    public void CheckSpawn()
    {
        Debug.Log("Wave is finished.");
    }

    public void OpenPortal()
    {
        StartCoroutine(CloseOrOpenPortal(false));    
    }

    public void ClosePortal()
    {
        StartCoroutine(CloseOrOpenPortal(true));
    }

    IEnumerator CloseOrOpenPortal(bool close)
    {
        if (close)
        {

            while (_scale > 0.05)
            {
                _scale = Mathf.Lerp(_scale, 0f, _scaleSpeed * Time.deltaTime);
                transform.localScale = Vector3.one * _scale;
                yield return new WaitForEndOfFrame();
            }
            transform.localScale = Vector3.zero;
            _isOpened = false;

            if (_particles.activeInHierarchy)
            {
                _particles.SetActive(false);
            }

            _GFX.SetActive(false);
        }

        else
        {
            _GFX.SetActive(true);

            while (_scale < 0.95)
            {
                _scale = Mathf.Lerp(_scale, 1f, _scaleSpeed * Time.deltaTime);
                transform.localScale = Vector3.one * _scale;
                yield return new WaitForEndOfFrame();
            }
            transform.localScale = Vector3.one;
            _isOpened = true;

            if (!_particles.activeInHierarchy)
            {
                _particles.SetActive(true);
            }
        }

        yield return null;
    }

    void RotateParticles()
    {
        _particles.transform.Rotate(new Vector3(0, _particleRotationSpeed * Time.deltaTime, 0));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isEndPortal) return;

        WaveEntity waveEntity = other.GetComponentInParent<WaveEntity>();
        if (waveEntity != null)
        {
            EnemyTeleported.Invoke(waveEntity, waveEntity.GetComponent<Damageable>());
        }
    }
}

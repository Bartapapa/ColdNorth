using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public class Resource : MonoBehaviour
{
    [Header("Resource")]
    [SerializeField] protected ResourceManager.ResourceType _resourceType;
    [SerializeField] protected int _maxResourceQuantity = 3;
    [SerializeField] protected int _resourceQuantity = 3;
    [SerializeField] protected GameObject _resource_FullGFX;
    [SerializeField] protected GameObject _resource_MostGFX;
    [SerializeField] protected GameObject _resource_BarelyGFX;
    [SerializeField] protected GameObject _resource_EmptyGFX;

    protected int _fullQuantity;
    protected int _mostQuantity;

    public ResourceManager.ResourceType ResourceType => _resourceType;
    public int MaxResourceQuantity => _maxResourceQuantity;
    public int ResourceQuantity => _resourceQuantity;

    protected virtual void Awake()
    {
        _resourceQuantity = _maxResourceQuantity;

        _fullQuantity = _maxResourceQuantity;
        _mostQuantity = Mathf.CeilToInt(((_maxResourceQuantity / 3) * 2));

        UpdateGFX();
    }

    public virtual void GetMined(int quantity)
    {
        if (quantity < 0) return;

        _resourceQuantity -= quantity;
        UpdateGFX();

        if (_resourceQuantity <= 0)
        {
            DestroyResource();
        }
    }

    protected void UpdateGFX()
    {
        if (_resourceQuantity == _fullQuantity)
        {
            _resource_FullGFX.SetActive(true);
            _resource_MostGFX.SetActive(false);
            _resource_BarelyGFX.SetActive(false);
            _resource_EmptyGFX.SetActive(false);
        }
        else if (_resourceQuantity < _fullQuantity && _resourceQuantity >= _mostQuantity)
        {
            _resource_FullGFX.SetActive(false);
            _resource_MostGFX.SetActive(true);
            _resource_BarelyGFX.SetActive(false);
            _resource_EmptyGFX.SetActive(false);
        }
        else if (_resourceQuantity < _mostQuantity && _resourceQuantity > 0)
        {
            _resource_FullGFX.SetActive(false);
            _resource_MostGFX.SetActive(false);
            _resource_BarelyGFX.SetActive(true);
            _resource_EmptyGFX.SetActive(false);
        }
        else if (_resourceQuantity == 0)
        {
            _resource_FullGFX.SetActive(false);
            _resource_MostGFX.SetActive(false);
            _resource_BarelyGFX.SetActive(false);
            _resource_EmptyGFX.SetActive(true);
        }
    }

    private void DestroyResource()
    {
        Destroy(this);
    }
}

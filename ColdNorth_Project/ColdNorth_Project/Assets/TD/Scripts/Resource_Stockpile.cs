using GSGD1;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Resource_Stockpile : Resource
{
    //public delegate void ResourceEvent(ResourceManager.ResourceType resourceType, int quantityGained, int newQuantity);
    //public event ResourceEvent ResourcesMined = null;

    public UnityEvent<ResourceManager.ResourceType, int, int> ResourceUpdate = null;

    protected override void Awake()
    {
        ResourceManager.Instance.Stockpiles.Add(this);

        _fullQuantity = _maxResourceQuantity;
        _mostQuantity = Mathf.CeilToInt(((_maxResourceQuantity / 3) * 2));

        UpdateGFX();
        ActivateTargetingRing(false);
    }

    private void Start()
    {
        if (ResourceUpdate != null)
        {
            ResourceUpdate.Invoke(_resourceType, _resourceQuantity, _resourceQuantity);
        }
    }

    public override void GetMined(int quantity)
    {
        if (_resourceQuantity <= 0 && quantity > 0)
        {
            Debug.Log("There's no " + _resourceType.ToString() + " left!");
            return;
        }

        _resourceQuantity -= quantity;

        if (_resourceQuantity > _maxResourceQuantity)
        {
            _resourceQuantity = _maxResourceQuantity;
        }
        UpdateGFX();

        if (ResourceUpdate != null)
        {
            ResourceUpdate.Invoke(_resourceType, -quantity, _resourceQuantity);
        }

        //if (ResourcesMined != null)
        //{
        //    ResourcesMined.Invoke(_resourceType, 1, _resourceQuantity);
        //}
    }
}

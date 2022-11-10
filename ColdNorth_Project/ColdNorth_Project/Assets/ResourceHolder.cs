using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHolder : MonoBehaviour
{
    [SerializeField] private int _step1 = 1;
    [SerializeField] private int _step2 = 2;
    [SerializeField] private int _step3 = 3;

    [SerializeField] private GameObject _wood1GFX;
    [SerializeField] private GameObject _wood2GFX;
    [SerializeField] private GameObject _wood3GFX;
    [SerializeField] private GameObject _stone1GFX;
    [SerializeField] private GameObject _stone2GFX;
    [SerializeField] private GameObject _stone3GFX;

    private void Awake()
    {
        UpdateGFX(ResourceManager.ResourceType.None, 0);
    }

    public void UpdateGFX(ResourceManager.ResourceType resourceType, int quantity)
    {
        if (resourceType == ResourceManager.ResourceType.Wood)
        {
            if (quantity >= _step1 && quantity < _step2)
            {
                _wood1GFX.SetActive(true);
                _wood2GFX.SetActive(false);
                _wood3GFX.SetActive(false);
            }
            else if (quantity >= _step2 && quantity < _step3)
            {
                _wood1GFX.SetActive(false);
                _wood2GFX.SetActive(true);
                _wood3GFX.SetActive(false);
            }
            else if (quantity >= _step3)
            {
                _wood1GFX.SetActive(false);
                _wood2GFX.SetActive(false);
                _wood3GFX.SetActive(true);
            }
        }

        if (resourceType == ResourceManager.ResourceType.Stone)
        {
            if (quantity >= _step1 && quantity < _step2)
            {
                _stone1GFX.SetActive(true);
                _stone2GFX.SetActive(false);
                _stone3GFX.SetActive(false);
            }
            else if (quantity >= _step2 && quantity < _step3)
            {
                _stone1GFX.SetActive(false);
                _stone2GFX.SetActive(true);
                _stone3GFX.SetActive(false);
            }
            else if (quantity >= _step3)
            {
                _stone1GFX.SetActive(false);
                _stone2GFX.SetActive(false);
                _stone3GFX.SetActive(true);
            }

        }

        if (resourceType == ResourceManager.ResourceType.None || quantity == 0)
        {
            _wood1GFX.SetActive(false);
            _wood2GFX.SetActive(false);
            _wood3GFX.SetActive(false);
            _stone1GFX.SetActive(false);
            _stone2GFX.SetActive(false);
            _stone3GFX.SetActive(false);
        }
    }
}

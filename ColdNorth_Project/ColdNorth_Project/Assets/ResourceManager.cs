using GSGD1;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static ResourceManager;

public class ResourceManager : Singleton<ResourceManager>
{
    public enum ResourceType
    {
        Wood,
        Stone,
        None,
    }

    public delegate void ResourceEvent(ResourceManager.ResourceType resourceType, int quantityGained, int newQuantity);
    public event ResourceEvent ResourcesUpdated = null;

    [Header("Resources")]
    [SerializeField] private int _wood = 0;
    [SerializeField] private int _stone = 0;

    [Header("Stockpiles")]
    private List<Resource_Stockpile> _stockpiles = new List<Resource_Stockpile>();

    public int Wood => _wood;
    public int Stone => _stone;

    public List<Resource_Stockpile> Stockpiles => _stockpiles;

    protected override void Start()
    {
        base.Start();
        if (ResourcesUpdated != null)
        {
            ResourcesUpdated.Invoke(ResourceManager.ResourceType.Wood, _wood, _wood);
            ResourcesUpdated.Invoke(ResourceManager.ResourceType.Stone, _stone, _stone);
        }
    }

    public void OnResourceUpdate(ResourceManager.ResourceType resourceType, int quantityGained, int newQuantity)
    {
        if (resourceType == ResourceManager.ResourceType.Wood)
        {
            _wood += quantityGained;

            if (ResourcesUpdated != null)
            {
                ResourcesUpdated.Invoke(resourceType, quantityGained, _wood);
            }

        }

        if (resourceType == ResourceManager.ResourceType.Stone)
        {
            _stone += quantityGained;

            if (ResourcesUpdated != null)
            {
                ResourcesUpdated.Invoke(resourceType, quantityGained, _stone);
            }

        }
    }
}

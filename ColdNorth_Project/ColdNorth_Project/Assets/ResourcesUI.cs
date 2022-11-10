using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _woodCount;
    [SerializeField]
    private TextMeshProUGUI _stoneCount;

    private void Awake()
    {
        ResourceManager.Instance.ResourcesUpdated += UpdateUI;
    }

    private void UpdateUI(ResourceManager.ResourceType resourceType, int quantityGained, int newQuantity)
    {
        if (resourceType == ResourceManager.ResourceType.Wood)
        {
            _woodCount.text = newQuantity.ToString();
        }

        if (resourceType == ResourceManager.ResourceType.Stone)
        {
            _stoneCount.text = newQuantity.ToString();
        }
    }
}

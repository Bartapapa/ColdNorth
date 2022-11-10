using GSGD1;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerFoundation : MonoBehaviour, IPickerGhost, ICellChild
{
    [SerializeField] private Tower _finishedTower;
    [SerializeField] private int _neededWood;
    [SerializeField] private int _currentWood;
    [SerializeField] private int _neededStone;
    [SerializeField] private int _currentStone;

    public void Enable(bool isEnabled)
    {
        enabled = isEnabled;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void OnSetChild()
    {
        Enable(true);
    }

    public void Build(ResourceManager.ResourceType resourceType, int quantity)
    {

    }
}

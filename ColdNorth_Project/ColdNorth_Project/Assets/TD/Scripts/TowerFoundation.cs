using GSGD1;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerFoundation : MonoBehaviour, IPickerGhost, ICellChild
{
    [Header("References")]
    [SerializeField] private Tower _finishedTower;
    [SerializeField] private FoundationResourceUI _foundationResourceUI;

    [Header("Parameters")]
    [SerializeField] private int _neededWood;
    [SerializeField] private int _currentWood;
    [SerializeField] private int _neededStone;
    [SerializeField] private int _currentStone;

    [Header("Targeting")]
    [SerializeField] private GameObject _targetingRing;
    [SerializeField] private bool _canTargetFoundation = false;

    [SerializeField] private AudioClip _travailTermine;

    public int NeededWood => _neededWood;
    public int NeededStone => _neededStone;
    public int CurrentWood => _currentWood;
    public int CurrentStone => _currentStone;

    private void Awake()
    {
        _foundationResourceUI.gameObject.SetActive(false);
        _foundationResourceUI.SetResourcesMax(_neededWood, _neededStone);
        _targetingRing.SetActive(false);
    }

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
        ActivateUI();
        _canTargetFoundation = true;
    }

    public void ActivateUI()
    {
        _foundationResourceUI.gameObject.SetActive(true);
    }

    public void Build(ResourceManager.ResourceType resourceType, int quantity)
    {
        if (!_canTargetFoundation) return;

        if (resourceType == ResourceManager.ResourceType.Wood)
        {
            _currentWood += quantity;
            _foundationResourceUI.UpdateUI(resourceType, _currentWood);
        }

        if (resourceType == ResourceManager.ResourceType.Stone)
        {
            _currentStone += quantity;
            _foundationResourceUI.UpdateUI(resourceType, _currentStone);
        }

        CheckIfCompleted();
    }

    public void SetTower(Tower setTower)
    {
        _finishedTower = setTower;
    }

    public void SetNeeededResources(int woodQuantity, int stoneQuantity)
    {
        _neededWood = woodQuantity;
        _neededStone = stoneQuantity;
    }

    private void CheckIfCompleted()
    {
        if (_currentWood >= _neededWood && _currentStone >= _neededStone)
        {
            BuildComplete();
        }
    }

    private void BuildComplete()
    {
        Tower newTower = Instantiate(_finishedTower, transform.parent);

        SoundManager.Instance.PlaySFX(_travailTermine);

        Destroy(this.gameObject);
    }

    public void ActivateTargetingRing(bool value)
    {
        if (!_canTargetFoundation) return;
        _targetingRing.SetActive(value);
    }
}

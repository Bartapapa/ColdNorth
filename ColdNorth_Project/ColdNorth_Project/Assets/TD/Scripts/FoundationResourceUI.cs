using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FoundationResourceUI : MonoBehaviour
{
    [SerializeField] private GameObject _woodProgressBar;
    [SerializeField] private GameObject _stoneProgressBar;
    [SerializeField] private Color _colorInProgress;
    [SerializeField] private Color _colorComplete;
    [SerializeField] private int _woodMax;
    [SerializeField] private int _stoneMax;
    [SerializeField] private float _maxScale;
    [SerializeField] private float _completionFactorWood = .5f;
    [SerializeField] private float _completionFactorStone = 1f;

    private void Awake()
    {
        _woodProgressBar.GetComponent<SpriteRenderer>().color = _colorInProgress;
        _stoneProgressBar.GetComponent<SpriteRenderer>().color = _colorInProgress;
        _maxScale = _woodProgressBar.transform.localScale.x;

        _woodProgressBar.transform.localScale = new Vector3(0, _woodProgressBar.transform.localScale.y, _woodProgressBar.transform.localScale.z);
        _stoneProgressBar.transform.localScale = new Vector3(0, _stoneProgressBar.transform.localScale.y, _stoneProgressBar.transform.localScale.z);

    }
    public void SetResourcesMax(int woodMax, int stoneMax)
    {
        _woodMax = woodMax;
        _stoneMax = stoneMax;
    }

    public void UpdateUI(ResourceManager.ResourceType resourceType, int newQuantity)
    {
        if (resourceType == ResourceManager.ResourceType.Wood)
        {
            _woodProgressBar.transform.localScale = new Vector3(newQuantity*_completionFactorWood, _woodProgressBar.transform.localScale.y, _woodProgressBar.transform.localScale.z);
            Debug.Log(newQuantity);
            Debug.Log(_completionFactorWood);
            if (newQuantity == _woodMax)
            {
                Complete(_woodProgressBar);
            }
        }

        if (resourceType == ResourceManager.ResourceType.Stone)
        {
            _stoneProgressBar.transform.localScale = new Vector3(newQuantity*_completionFactorStone, _stoneProgressBar.transform.localScale.y, _stoneProgressBar.transform.localScale.z);
            if (newQuantity == _stoneMax)
            {
                Complete(_stoneProgressBar);
            }
        }
    }

    private void Complete(GameObject progressBar)
    {
        progressBar.GetComponent<SpriteRenderer>().color = _colorComplete;
    }
}

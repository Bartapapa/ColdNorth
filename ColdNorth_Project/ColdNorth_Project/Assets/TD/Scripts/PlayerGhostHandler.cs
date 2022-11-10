using GSGD1;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhostHandler : MonoBehaviour
{
    [SerializeField] private GameObject _ghostTowerHolder;

    [System.NonSerialized]
    private IPickerGhost _ghost = null;

    [System.NonSerialized]
    private bool _isActive = false;

    [SerializeField]
    private GridPicker _gridPicker = null;

    public GameObject GhostTowerHolder => _ghostTowerHolder;

    public void ActivateWithGhost(IPickerGhost ghost)
    {
        _ghost = ghost;
        Activate(true);
    }
    public void Activate(bool isActive)
    {
        _isActive = isActive;
        _gridPicker.Activate(isActive, true);
    }

    private void Update()
    {
        if (_isActive)
        {
            PlaceGhost();
        }

    }

    public void PlaceGhost()
    {
        //If nowhere near a tile, place it on holder.
        _ghost.GetTransform().position = _ghostTowerHolder.transform.position;
        //Otherwise, place it near tile.

    }
}

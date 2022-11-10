using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform sight;

    [Header("Gravity")]
    [SerializeField] private float _gravity = 9.81f;

    [Header("Movement")]
    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _acceleration = 5f;
    private float _Xspeed = 0f;
    private float _Zspeed = 0f;
    private bool canMove = true;
    private bool canRotate = true;
    private bool canAct = true;

    [Header("Resources")]
    [SerializeField] private ResourceHolder _resourceHolder;
    [SerializeField] private LayerMask _resourceLayer;
    [SerializeField] private int _maxNumberResourcesCarried = 3;
    [SerializeField] private int _currentNumberResourcesCarried = 0;
    private ResourceManager.ResourceType _currentResource = ResourceManager.ResourceType.None;
    [SerializeField] private Resource _targetedResource;
    [SerializeField] private Resource_Stockpile _targetedStockpile;

    [Header("Tower")]
    [SerializeField] private LayerMask _towerFoundationLayer;
    [SerializeField] private TowerFoundation _targetedTowerFoundation;

    [Header("Animator")]
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject _axe;
    [SerializeField] private GameObject _pickaxe;


    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _pickaxe.SetActive(false);
        _axe.SetActive(true);
    }
    private void Update()
    {
        HandleInputs();
        HandleTargetingResource();
        HandleTargetingTowerFoundation();
    }

    private void HandleTargetingTowerFoundation()
    {
        bool raycastFoundFoundation = Physics.Raycast(sight.position, transform.forward, out RaycastHit hitFoundation, 1f, _towerFoundationLayer);
        if (raycastFoundFoundation)
        {
            _targetedTowerFoundation = hitFoundation.transform.gameObject.GetComponent<TowerFoundation>();
            _targetedTowerFoundation.ActivateTargetingRing(true);
        }
        else
        {
            if (_targetedTowerFoundation != null)
            {
                _targetedTowerFoundation.ActivateTargetingRing(false);
            }

            _targetedTowerFoundation = null;
        }
    }

    private void HandleTargetingResource()
    {
        bool raycastFoundResource = Physics.Raycast(sight.position, transform.forward, out RaycastHit hitResource, 1f, _resourceLayer);
        if (raycastFoundResource)
        {
            _targetedResource = hitResource.transform.gameObject.GetComponent<Resource>();
            _targetedStockpile = hitResource.transform.gameObject.GetComponent<Resource_Stockpile>();

            _targetedResource.ActivateTargetingRing(true);

            if (_targetedResource.ResourceType == ResourceManager.ResourceType.Wood)
            {
                _pickaxe.SetActive(false);
                _axe.SetActive(true);
            }
            else if (_targetedResource.ResourceType == ResourceManager.ResourceType.Stone)
            {
                _pickaxe.SetActive(true);
                _axe.SetActive(false);
            }
        }
        else
        {
            if (_targetedResource != null)
            {
                _targetedResource.ActivateTargetingRing(false);
            }

            _targetedResource = null;
            _targetedStockpile = null;
        }
    }

    private void FixedUpdate()
    {
        Gravity();
        Move();
    }

    private void Move()
    {
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        _Xspeed = Mathf.Lerp(_Xspeed, _maxSpeed * direction.x, _acceleration * Time.deltaTime);
        _Zspeed = Mathf.Lerp(_Zspeed, _maxSpeed * direction.z, _acceleration * Time.deltaTime);

        if (_Xspeed != 0 || _Zspeed != 0)
        {
            _anim.SetBool("isMoving", true);
        }
        else
        {
            _anim.SetBool("isMoving", false);
        }

        if (Mathf.Abs(_Xspeed) < .1f && Input.GetAxisRaw("Horizontal") == 0)
        {
            _Xspeed = 0;
        }
        if (Mathf.Abs(_Zspeed) < .1f && Input.GetAxisRaw("Vertical") == 0)
        {
            _Zspeed = 0;
        }

        if (canMove)
        {
            _rb.velocity = new Vector3(_Xspeed, _rb.velocity.y, _Zspeed);
        }

        if (direction != Vector3.zero && canRotate)
        {
            Quaternion toRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRot, Time.deltaTime * _rotationSpeed);
        }
    }

    private void Gravity()
    {
        _rb.AddForce(Vector3.down * _gravity);
    }

    private void HandleInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canAct)
        {
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
            _anim.SetTrigger("MineBuild");
            //Mine();
            //Build();
        }
    }

    public void Build()
    {
        if (_targetedTowerFoundation != null)
        {
            if (_currentResource == ResourceManager.ResourceType.None || _currentNumberResourcesCarried == 0)
            {
                Debug.Log("You have no resources to build with!");
                return;
            }
            else
            {
                if (_currentResource == ResourceManager.ResourceType.Wood)
                {
                    if (_targetedTowerFoundation.NeededWood == _targetedTowerFoundation.CurrentWood)
                    {
                        Debug.Log("The foundation has enough wood!");
                        return;
                    }

                    if (_currentNumberResourcesCarried > (_targetedTowerFoundation.NeededWood - _targetedTowerFoundation.CurrentWood))
                    {
                        int quantityToGive = _targetedTowerFoundation.NeededWood - _targetedTowerFoundation.CurrentWood;
                        _targetedTowerFoundation.Build(_currentResource, quantityToGive);
                        PlaceResource(-quantityToGive);
                    }
                    else
                    {
                        _targetedTowerFoundation.Build(_currentResource, _currentNumberResourcesCarried);
                        PlaceResource(-_currentNumberResourcesCarried);
                    }
                }
                else if (_currentResource == ResourceManager.ResourceType.Stone)
                {
                    if (_targetedTowerFoundation.NeededStone == _targetedTowerFoundation.CurrentStone)
                    {
                        Debug.Log("The foundation has enough stone!");
                        return;
                    }

                    if (_currentNumberResourcesCarried > (_targetedTowerFoundation.NeededStone - _targetedTowerFoundation.CurrentStone))
                    {
                        int quantityToGive = _targetedTowerFoundation.NeededStone - _targetedTowerFoundation.CurrentStone;
                        _targetedTowerFoundation.Build(_currentResource, quantityToGive);
                        PlaceResource(-quantityToGive);
                    }
                    else
                    {
                        _targetedTowerFoundation.Build(_currentResource, _currentNumberResourcesCarried);
                        PlaceResource(-_currentNumberResourcesCarried);
                    }
                }

            }
        }
    }

    public void Mine()
    {
        //Do mining here;
        if (_targetedStockpile != null)
        {

            if (_currentResource == ResourceManager.ResourceType.None && _targetedStockpile.ResourceQuantity > 0)
            {
                SetResource(_targetedResource.ResourceType);
            }

            if (_currentResource != _targetedResource.ResourceType)
            {
                Debug.Log("You can't mine that right now!");
                return;
            }
            else
            {
                if (_targetedStockpile.ResourceQuantity <= 0)
                {
                    if (_currentNumberResourcesCarried > 0)
                    {
                        _targetedStockpile.GetMined(-_currentNumberResourcesCarried);
                        PlaceResource(-_currentNumberResourcesCarried);
                    }
                    else
                    {
                        Debug.Log("There's nothing here!");
                    }
                    return;
                }
                //If the player has 3, place those into the stockpile, up to the max it can take up.
                else if (_currentNumberResourcesCarried >= _maxNumberResourcesCarried)
                {
                    if (_currentNumberResourcesCarried > (_targetedStockpile.MaxResourceQuantity - _targetedStockpile.ResourceQuantity))
                    {
                        //This is the case where the player carries more resources than the stockpile can muster.
                        int quantityToPlace = _targetedStockpile.MaxResourceQuantity - _targetedStockpile.ResourceQuantity;
                        _targetedStockpile.GetMined(-quantityToPlace);
                        PlaceResource(-quantityToPlace);
                    }
                    else
                    {
                        _targetedStockpile.GetMined(-_currentNumberResourcesCarried);
                        PlaceResource(-_currentNumberResourcesCarried);
                    }
                    return;
                }
                else if (_currentNumberResourcesCarried < _maxNumberResourcesCarried)
                {
                    //If the player has <3, take as many as necessary, up to the amount of the stockpile has, to go up to max.
                    int quantityToTake = _maxNumberResourcesCarried - _currentNumberResourcesCarried;
                    if (quantityToTake > _targetedStockpile.ResourceQuantity)
                    {
                        int newQuantity = _targetedStockpile.ResourceQuantity;
                        _targetedStockpile.GetMined(newQuantity);
                        AcquireResource(quantityToTake);
                    }
                    else
                    {
                        _targetedStockpile.GetMined(quantityToTake);
                        AcquireResource(quantityToTake);
                    }
                    return;
                }
            }
            return;
        }
        if (_targetedResource != null)
        {
            if (_currentResource == ResourceManager.ResourceType.None)
            {
                SetResource(_targetedResource.ResourceType);
            }
            
            if (_currentResource != _targetedResource.ResourceType)
            {
                Debug.Log("You can't mine that right now!");
                return;
            }

            if (_currentNumberResourcesCarried >= _maxNumberResourcesCarried)
            {
                Debug.Log("You're carrying too much right now!");
                return;
            }

            _targetedResource.GetMined(1);
            AcquireResource(1);
            return;
        }
    }
    void SetResource(ResourceManager.ResourceType resourceType)
    {
        _currentResource = resourceType;
    }

    public void AcquireResource(int quantity)
    {
        _currentNumberResourcesCarried += quantity;
        _resourceHolder.UpdateGFX(_currentResource, _currentNumberResourcesCarried);
    }

    void PlaceResource(int quantity)
    {
        _currentNumberResourcesCarried += quantity;
        _resourceHolder.UpdateGFX(_currentResource, _currentNumberResourcesCarried);

        if (_currentNumberResourcesCarried <= 0)
        {
            SetResource(ResourceManager.ResourceType.None);
        }
    }

    //Animator methods

    public void DisableCanRotate()
    {
        canRotate = false;
    }

    public void DisableCanMove()
    {
        canMove = false;
    }

    public void DisableCanAct()
    {
        canAct = false;
    }
    public void EnableCanRotate()
    {
        canRotate = true;
    }

    public void EnableCanMove()
    {
        canMove = true;
    }

    public void EnableCanAct()
    {
        canAct = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(sight.position, sight.position + transform.forward);
    }
}

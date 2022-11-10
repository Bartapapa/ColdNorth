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

    [Header("Resources")]
    [SerializeField] private ResourceHolder resourceHolder;
    [SerializeField] private LayerMask resourceLayer;
    [SerializeField] private int _maxNumberResourcesCarried = 3;
    [SerializeField] private int _currentNumberResourcesCarried = 0;
    private ResourceManager.ResourceType currentResource = ResourceManager.ResourceType.None;
    [SerializeField] private Resource targetedResource;
    [SerializeField] private Resource_Stockpile targetedStockpile;


    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        HandleInputs();
        HandleTargetingResource();
    }

    private void HandleTargetingResource()
    {
        bool raycastFoundResource = Physics.Raycast(sight.position, transform.forward, out RaycastHit hitResource, 1f, resourceLayer);
        if (raycastFoundResource)
        {
            targetedResource = hitResource.transform.gameObject.GetComponent<Resource>();
            targetedStockpile = hitResource.transform.gameObject.GetComponent<Resource_Stockpile>();
        }
        else
        {
            targetedResource = null;
            targetedStockpile = null;
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

        if (Mathf.Abs(_Xspeed) < .1f && Input.GetAxisRaw("Horizontal") == 0)
        {
            _Xspeed = 0;
        }
        if (Mathf.Abs(_Zspeed) < .1f && Input.GetAxisRaw("Vertical") == 0)
        {
            _Zspeed = 0;
        }


        _rb.velocity = new Vector3(_Xspeed, _rb.velocity.y, _Zspeed);

        if (direction != Vector3.zero)
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Mine();
        }
    }

    public void Mine()
    {
        //Do mining here;
        if (targetedStockpile != null)
        {
            int quantityToAcquire = 0;

            if (currentResource == ResourceManager.ResourceType.None && targetedStockpile.ResourceQuantity > 0)
            {
                SetResource(targetedResource.ResourceType);
            }

            if (currentResource != targetedResource.ResourceType)
            {
                Debug.Log("You can't mine that right now!");
                return;
            }
            else
            {
                if (targetedStockpile.ResourceQuantity <= 0)
                {
                    if (_currentNumberResourcesCarried > 0)
                    {
                        targetedStockpile.GetMined(-_currentNumberResourcesCarried);
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
                    if (_currentNumberResourcesCarried > (targetedStockpile.MaxResourceQuantity - targetedStockpile.ResourceQuantity))
                    {
                        //This is the case where the player carries more resources than the stockpile can muster.
                        int quantityToPlace = targetedStockpile.MaxResourceQuantity - targetedStockpile.ResourceQuantity;
                        targetedStockpile.GetMined(-quantityToPlace);
                        PlaceResource(-quantityToPlace);
                    }
                    else
                    {
                        targetedStockpile.GetMined(-_currentNumberResourcesCarried);
                        PlaceResource(-_currentNumberResourcesCarried);
                    }
                    return;
                }
                else if (_currentNumberResourcesCarried < _maxNumberResourcesCarried)
                {
                    //If the player has <3, take as many as necessary, up to the amount of the stockpile has, to go up to max.
                    int quantityToTake = _maxNumberResourcesCarried - _currentNumberResourcesCarried;
                    if (quantityToTake > targetedStockpile.ResourceQuantity)
                    {
                        int newQuantity = targetedStockpile.ResourceQuantity;
                        targetedStockpile.GetMined(newQuantity);
                        AcquireResource(quantityToTake);
                    }
                    else
                    {
                        targetedStockpile.GetMined(quantityToTake);
                        AcquireResource(quantityToTake);
                    }
                    return;
                }
            }
            return;
        }
        if (targetedResource != null)
        {
            if (currentResource == ResourceManager.ResourceType.None)
            {
                SetResource(targetedResource.ResourceType);
            }
            
            if (currentResource != targetedResource.ResourceType)
            {
                Debug.Log("You can't mine that right now!");
                return;
            }

            if (_currentNumberResourcesCarried >= _maxNumberResourcesCarried)
            {
                Debug.Log("You're carrying too much right now!");
                return;
            }

            targetedResource.GetMined(1);
            AcquireResource(1);
            return;
        }
    }
    void SetResource(ResourceManager.ResourceType resourceType)
    {
        currentResource = resourceType;
    }

    public void AcquireResource(int quantity)
    {
        _currentNumberResourcesCarried += quantity;
        resourceHolder.UpdateGFX(currentResource, _currentNumberResourcesCarried);
    }

    void PlaceResource(int quantity)
    {
        _currentNumberResourcesCarried += quantity;
        resourceHolder.UpdateGFX(currentResource, _currentNumberResourcesCarried);

        if (_currentNumberResourcesCarried <= 0)
        {
            SetResource(ResourceManager.ResourceType.None);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(sight.position, sight.position + transform.forward);
    }
}

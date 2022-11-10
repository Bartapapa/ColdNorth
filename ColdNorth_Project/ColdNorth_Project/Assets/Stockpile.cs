using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stockpile : MonoBehaviour
{
    [SerializeField] Resource_Stockpile _woodPile;
    [SerializeField] Resource_Stockpile _stonePile;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateStockpile()
    {
        //either the player grabs resources, or puts resources.
        //Follow this logic:
        //If the player has 3 resources, put it.
        //If the player has less than 3 resources, take some to make 3.
        //If the stockpile doesn't have enough resources to make 3, give everything left.
    }
}

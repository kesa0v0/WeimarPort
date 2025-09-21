using UnityEngine;

public class PlayerState : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


public enum PlayerActionState
{
    None,
    SelectingCityForUnitMove,
    SelectingCityForAddSeat,
    SelectingCityForRemoveSeat,
    SelectingCityForCardEffect 
}
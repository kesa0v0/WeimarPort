using System;
using UnityEngine;

public class SeatView : MonoBehaviour
{
    public void SetColor(Color partyColor)
    {
        GetComponent<SpriteRenderer>().color = partyColor;
    }
}

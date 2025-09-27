using System;
using UnityEngine;

public class PartyBaseView : MonoBehaviour
{
    public void SetColor(Color partyColor)
    {
        GetComponent<SpriteRenderer>().color = partyColor;
    }
}

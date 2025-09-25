using System;
using System.Collections.Generic;
using UnityEngine;

public class Party
{
    public FactionData Data { get; private set; }

    public int victoryPoints;
    public List<MinorPartyData> ControlledMinorParties { get; private set; }
    // public List<Card> hand;


    public Party(FactionData data)
    {
        Data = data;
        ControlledMinorParties = new List<MinorPartyData>();
    }
}
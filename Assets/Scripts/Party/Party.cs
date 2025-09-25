using System;
using System.Collections.Generic;
using UnityEngine;

public class Party: IUnitContainer
{
    public FactionData Data { get; private set; }

    public int victoryPoints;
    public string currentPartyAgenda;
    public List<MinorPartyData> ControlledMinorParties { get; private set; }

    public Dictionary<UnitPresenter, int> ContainedUnits { get; private set; }

    // public List<Card> hand;


    public Party(FactionData data)
    {
        Data = data;
        ControlledMinorParties = new List<MinorPartyData>();
        ContainedUnits = new Dictionary<UnitPresenter, int>();
    }

    public void AddUnit(UnitPresenter unit)
    {
        if (!ContainedUnits.ContainsKey(unit))
            ContainedUnits[unit] = 0;
    }

    public void RemoveUnit(UnitPresenter unit)
    {
        if (ContainedUnits.ContainsKey(unit))
            ContainedUnits.Remove(unit);
    }
}
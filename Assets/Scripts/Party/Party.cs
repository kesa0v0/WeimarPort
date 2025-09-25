using System;
using System.Collections.Generic;
using UnityEngine;

public class Party: IUnitContainer
{
    public FactionData Data { get; private set; }

    public int victoryPoints;
    public string currentPartyAgenda;
    public List<MinorPartyData> ControlledMinorParties { get; private set; }
    public List<UnitModel> supplyUnits;


    // public List<Card> hand;


    public Party(FactionData data)
    {
        Data = data;
        ControlledMinorParties = new List<MinorPartyData>();
        supplyUnits = new List<UnitModel>();
    }


    IList<UnitModel> IUnitContainer.ContainedUnits => supplyUnits;

    public void AddUnit(UnitModel unit)
    {
        if (unit == null) return;
        if (!supplyUnits.Contains(unit))
            supplyUnits.Add(unit);
    }

    public void RemoveUnit(UnitModel unit)
    {
        if (unit == null) return;
        supplyUnits.Remove(unit);
    }

    public List<UnitModel> GetUnits()
    {
        return supplyUnits;
    }

    public string GetContainerName()
    {
        return Data != null ? Data.factionName : "Unknown Party";
    }
}
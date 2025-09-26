using System;
using System.Collections.Generic;
using UnityEngine;

public class PartyModel: IUnitContainer
{
    public FactionData Data { get; private set; }

    public int victoryPoints;
    public string currentPartyAgenda;
    public List<MinorPartyData> ControlledMinorParties { get; private set; }
    public List<UnitModel> supplyUnits;


    // public List<Card> hand;


    public PartyModel(FactionData data)
    {
        Data = data;
        ControlledMinorParties = new List<MinorPartyData>();
        supplyUnits = new List<UnitModel>();
    }

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
        if (supplyUnits == null) return new List<UnitModel>();
        return new List<UnitModel>(supplyUnits);
    }

    public string GetContainerName()
    {
        return Data != null ? Data.factionName : "Unknown Party";
    }
}
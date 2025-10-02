using System.Collections.Generic;
using UnityEngine;

public class Government : IUnitContainer
{
    public List<PartyModel> GoverningParties { get; private set; }
    public PartyModel Chancellor { get; private set; }

    public List<UnitModel> supplyUnits;

    public string Name => "Government";
    public Color Color => Color.white;

    public Government()
    {
        GoverningParties = new List<PartyModel>();
        Chancellor = null;
    }

    // 정부를 새로 구성하는 함수
    public void FormNewGovernment(PartyModel newChancellor, PartyModel newCoalitionPartner = null)
    {
        var newGoverningParties = new List<PartyModel> { newChancellor };
        if (newCoalitionPartner != null)
            newGoverningParties.Add(newCoalitionPartner);

        GoverningParties = newGoverningParties;
        Chancellor = newChancellor;
    }

    // 특정 정당이 현재 정부에 속해 있는지 확인하는 함수
    public bool IsInGovernment(PartyModel party)
    {
        return GoverningParties.Contains(party);
    }

    #region Unit Container Implementation

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
        if (supplyUnits == null)
            return new List<UnitModel>();
        return new List<UnitModel>(supplyUnits);
    }

    public string GetContainerName()
    {
        return Name;
    }

    #endregion
}
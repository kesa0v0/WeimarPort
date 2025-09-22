using System.Collections.Generic;
using UnityEngine;

public class Government : IUnitContainer
{
    public MainParty RulingParty { get; private set; }

    public Dictionary<UnitPresenter, int> ContainedUnits { get; private set; } = new();

    public string Name => "Government";
    public Color Color => RulingParty != null ? RulingParty.partyColor : Color.white;

    public void SetRulingParty(MainParty party)
    {
        RulingParty = party;
    }

    public void AddUnit(UnitPresenter unit)
    {
        if (ContainedUnits.ContainsKey(unit))
            ContainedUnits[unit]++;
        else
            ContainedUnits[unit] = 1;
    }

    public void RemoveUnit(UnitPresenter unit)
    {
        if (ContainedUnits.ContainsKey(unit))
        {
            ContainedUnits[unit]--;
            if (ContainedUnits[unit] <= 0)
                ContainedUnits.Remove(unit);
        }
        else
        {
            Debug.LogWarning("Attempted to remove unit not in government's contained units.");
        }
    }
}

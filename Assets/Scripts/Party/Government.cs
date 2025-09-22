using System.Collections.Generic;
using UnityEngine;

public class Government : IUnitContainer
{
    // Coalition can have 1-2 parties
    public List<MainParty> CoalitionParties { get; private set; } = new List<MainParty>(2);

    // Backward-compat convenience: primary party if any
    public MainParty RulingParty => CoalitionParties.Count > 0 ? CoalitionParties[0] : null;

    public Dictionary<UnitPresenter, int> ContainedUnits { get; private set; } = new();

    public string Name => "Government";
    // Government symbol color is always white (per spec)
    public Color Color => Color.white;

    public void SetRulingParty(MainParty party)
    {
        SetRulingCoalition(party, null);
    }

    public void SetRulingCoalition(MainParty primary, MainParty secondary = null)
    {
        CoalitionParties.Clear();
        if (primary != null)
            CoalitionParties.Add(primary);
        if (secondary != null && secondary != primary)
            CoalitionParties.Add(secondary);
    }

    public void SetRulingCoalition(List<MainParty> parties)
    {
        CoalitionParties.Clear();
        if (parties == null) return;
        foreach (var p in parties)
        {
            if (p == null) continue;
            if (CoalitionParties.Count == 2) break;
            if (!CoalitionParties.Contains(p))
                CoalitionParties.Add(p);
        }
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

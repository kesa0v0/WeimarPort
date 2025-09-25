using System.Collections.Generic;
using UnityEngine;

public class Government : IUnitContainer
{
    public List<Party> GoverningParties { get; private set; }
    public Party Chancellor { get; private set; }

    public Dictionary<Unit, int> ContainedUnits { get; private set; } = new();

    public string Name => "Government";
    public Color Color => Color.white;

    public Government()
    {
        GoverningParties = new List<Party>();
        Chancellor = null;
    }

    // 정부를 새로 구성하는 함수
    public void FormNewGovernment(Party newChancellor, Party newCoalitionPartner = null)
    {
        var newGoverningParties = new List<Party> { newChancellor };
        if (newCoalitionPartner != null)
            newGoverningParties.Add(newCoalitionPartner);

        GoverningParties = newGoverningParties;
        Chancellor = newChancellor;
    }
    
    // 특정 정당이 현재 정부에 속해 있는지 확인하는 함수
    public bool IsInGovernment(Party party)
    {
        return GoverningParties.Contains(party);
    }

    public void AddUnit(Unit unit)
    {
        if (ContainedUnits.ContainsKey(unit))
            ContainedUnits[unit]++;
        else
            ContainedUnits[unit] = 1;

        UIManager.Instance?.governmentPanel?.Redraw();
    }

    public void RemoveUnit(Unit unit)
    {
        if (ContainedUnits.ContainsKey(unit))
        {
            ContainedUnits[unit]--;
            if (ContainedUnits[unit] <= 0)
                ContainedUnits.Remove(unit);
            UIManager.Instance?.governmentPanel?.Redraw();
        }
        else
        {
            Debug.LogWarning("Attempted to remove unit not in government's contained units.");
        }
    }
}

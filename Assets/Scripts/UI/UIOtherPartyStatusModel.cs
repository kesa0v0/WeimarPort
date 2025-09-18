using System.Collections.Generic;
using UnityEngine;

public class UIOtherPartyStatusModel
{
    public string PartyName { get; set; }
    public string PartyStatus { get; set; }
    public string PartyAgenda { get; set; }
    public Dictionary<string, int> PartyUnits { get; set; } = new Dictionary<string, int>();

    public UIOtherPartyStatusModel(string partyName)
    {
        PartyName = partyName;
    }

    public void AddPreservedUnit(string unitType, int count)
    {
        if (PartyUnits.ContainsKey(unitType))
        {
            PartyUnits[unitType] += count;
        }
        else
        {
            PartyUnits[unitType] = count;
        }
    }

    public void UpdateStatus(string status)
    {
        PartyStatus = status;
    }

    public void UpdateAgenda(string agenda)
    {
        PartyAgenda = agenda;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public static class PartyRegistry
{
    public static readonly SPD SPD = SPD.Instance;
    public static readonly Zentrum Zentrum = Zentrum.Instance;
    public static readonly KPD KPD = KPD.Instance;
    public static readonly DNVP DNVP = DNVP.Instance;

    public static readonly List<Party> AllParties = new() { KPD, SPD, Zentrum, DNVP, USPD.Instance, DDP.Instance, DVP.Instance, NSDAP.Instance };
    public static readonly List<MainParty> AllMainParties = new() { KPD, SPD, Zentrum, DNVP };
    public static readonly List<SubParty> AllSubParties = new() { USPD.Instance, DDP.Instance, DVP.Instance, NSDAP.Instance };

    public static Party GetPartyByName(string name)
    {
        switch (name)
        {
            case "SPD": return SPD;
            case "Zentrum": return Zentrum;
            case "Z": return Zentrum;
            case "KPD": return KPD;
            case "DNVP": return DNVP;
            case "USPD": return USPD.Instance;
            case "DDP": return DDP.Instance;
            case "DVP": return DVP.Instance;
            case "NSDAP": return NSDAP.Instance;
            default:
                Debug.LogWarning($"Party '{name}' not found. Returning null.");
                return null;
        }
    }

    
}

[Serializable]
public class Party
{
    public string partyName;
    public Color partyColor;

    public Party(string name, Color color)
    {
        partyName = name;
        partyColor = color;
    }
}

[Serializable]
public class MainParty : Party
{
    // Playable faction
    public string partyGovernmentStatus;
    public string currentPartyAgenda;
    public List<SubParty> heldSubParties { get; set; } = new List<SubParty>();
    public Dictionary<string, int> preservedPartyUnits { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> inSupplyPartyUnits { get; set; } = new Dictionary<string, int>();
    
    public MainParty(string name, Color color) : base(name, color) { }
}

[Serializable]
public class SubParty : Party
{
    // Non-playable faction
    public SubParty(string name, Color color) : base(name, color) { }
}


// Main Parties

public class SPD : MainParty
{
    public static readonly SPD Instance = new();
    public SPD() : base("SPD", Color.red) { }
}

public class Zentrum : MainParty
{
    public static readonly Zentrum Instance = new();
    public Zentrum() : base("Zentrum", Color.blue) { }
}

public class KPD : MainParty
{
    public static readonly KPD Instance = new();
    public KPD() : base("KPD", Color.hotPink) { }
}

public class DNVP : MainParty
{
    public static readonly DNVP Instance = new();
    public DNVP() : base("DNVP", Color.black) { }
}

// Sub Parties

public class USPD : SubParty
{
    public static readonly USPD Instance = new();
    public USPD() : base("USPD", new Color(1.0f, 0.5f, 0.5f)) { } // Light Red
}
public class DDP : SubParty
{
    public static readonly DDP Instance = new();
    public DDP() : base("DDP", Color.yellow) { }
}

public class DVP : SubParty
{
    public static readonly DVP Instance = new();
    public DVP() : base("DVP", new Color(0.5f, 0.5f, 1.0f)) { } // Light Blue
}

public class NSDAP : SubParty
{
    public static readonly NSDAP Instance = new();
    public NSDAP() : base("NSDAP", Color.brown) { }
}
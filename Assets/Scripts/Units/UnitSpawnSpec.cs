using System;
using System.Collections.Generic;

[Serializable]
public class UnitSpawnSpec
{
    public string unitName;      // refers to UnitData.unitName
    public string membership;    // owning party (e.g., SPD)
    public UnitPosition position; // InPool, InReserved, OnBoard
    public string locationId;    // playerId for InReserved, cityName for OnBoard, optional for InPool
    public int count = 1;        // how many instances of this spec
}

[Serializable]
public class UnitSpawnSpecList
{
    public List<UnitSpawnSpec> units = new();
}

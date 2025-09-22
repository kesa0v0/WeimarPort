using System;
using System.Collections.Generic;

[Serializable]
public class UnitSpawnSpecJson
{
    public string id;
    public string unitName;
    public string membership;
    public string position;   // 문자열("InPool", "InReserved", "OnBoard" 또는 숫자)
    public string locationId;
    public int count = 1;
}

[Serializable]
public class UnitSpawnSpecJsonList
{
    public List<UnitSpawnSpecJson> units = new();
}

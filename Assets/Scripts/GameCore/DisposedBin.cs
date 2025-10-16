using System.Collections.Generic;
using UnityEngine;

// 간단한 폐기 보관소 컨테이너. 뷰는 생성하지 않음.
public class DisposedBin : IUnitContainer
{
    public IList<UnitModel> DisposedUnits = new List<UnitModel>();
    

    public void AddUnit(UnitModel unit)
    {
        DisposedUnits.Add(unit);
    }

    public LocationData GetContainerData()
    {
        return new LocationData
        {
            Type = LocationType.DissolvedArea,
            Name = "DisposedBin"
        };
    }

    public List<UnitModel> GetUnits()
    {
        if (DisposedUnits == null) return new List<UnitModel>();
        return new List<UnitModel>(DisposedUnits);
    }

    public void RemoveUnit(UnitModel unit)
    {
        DisposedUnits.Remove(unit);
    }
}

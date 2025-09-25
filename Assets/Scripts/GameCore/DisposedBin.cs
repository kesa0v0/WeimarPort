using System.Collections.Generic;
using UnityEngine;

// 간단한 폐기 보관소 컨테이너. 뷰는 생성하지 않음.
public class DisposedBin : IUnitContainer
{
    private static DisposedBin _instance;
    public static DisposedBin Instance => _instance ??= new DisposedBin();

    public IList<UnitModel> DisposedUnits = new List<UnitModel>();
    
    public IList<UnitModel> ContainedUnits => DisposedUnits;

    public void AddUnit(UnitModel unit)
    {
        DisposedUnits.Add(unit);
    }

    public string GetContainerName()
    {
        return "Disposed Bin";
    }

    public List<UnitModel> GetUnits()
    {
        return new List<UnitModel>(DisposedUnits);
    }

    public void RemoveUnit(UnitModel unit)
    {
        DisposedUnits.Remove(unit);
    }
}

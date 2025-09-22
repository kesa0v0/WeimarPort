using System.Collections.Generic;
using UnityEngine;

// 간단한 폐기 보관소 컨테이너. 뷰는 생성하지 않음.
public class DisposedBin : IUnitContainer
{
    private static DisposedBin _instance;
    public static DisposedBin Instance => _instance ??= new DisposedBin();

    public Dictionary<UnitPresenter, int> ContainedUnits { get; private set; } = new();

    public void AddUnit(UnitPresenter unit)
    {
        if (ContainedUnits.ContainsKey(unit)) ContainedUnits[unit]++;
        else ContainedUnits[unit] = 1;
    }

    public void RemoveUnit(UnitPresenter unit)
    {
        if (!ContainedUnits.ContainsKey(unit)) return;
        ContainedUnits[unit]--;
        if (ContainedUnits[unit] <= 0) ContainedUnits.Remove(unit);
    }
}

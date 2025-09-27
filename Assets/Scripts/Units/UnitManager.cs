using System;
using System.Collections.Generic;
using UnityEngine;
using IngameDebugConsole;

public class UnitManager : MonoBehaviour
{
    #region Singleton & Fields
    public static UnitManager Instance { get; private set; }

    // Data references (assigned in Editor)
    public List<UnitData> unitDataList;

    // Runtime caches
    public Dictionary<string, UnitModel> units = new Dictionary<string, UnitModel>();
    public Dictionary<UnitModel, Unit> unitPresenters = new Dictionary<UnitModel, Unit>();
    public Dictionary<UnitModel, UnitView> unitViews = new Dictionary<UnitModel, UnitView>();
    #endregion

    #region Unity Lifecycle
    public void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
        AddDebugCommands();
    }
    #endregion


    #region Initialization / Scenario Loading

    public void ClearAllUnits()
    {
        foreach (var view in unitViews.Values)
        {
            if (view != null) Destroy(view.gameObject);
        }
        unitViews.Clear();
        unitPresenters.Clear();
        units.Clear();
    }

    #endregion

    #region Public API - Creation

    public UnitData GetUnitDataByName(string unitName)
    {
        return unitDataList.Find(unit => unit.UnitName == unitName);
    }

    #endregion

    #region Debug Listing
    public void ListUnits()
    {
        if (units.Count == 0) { Debug.Log("No units spawned."); return; }
        foreach (var kv in units)
        {
            var m = kv.Value;
            Debug.Log($"{m.InstanceId} | {m.Data.UnitName} | {m.CurrentState} | {m.CurrentLocation.Name} | owner={m.ControllerPartyId}");
        }
    }

    #endregion

    #region Presenter / Lookup
    public Unit GetPresenterById(string unitId)
    {
        if (units.TryGetValue(unitId, out var unit))
            return unitPresenters.ContainsKey(unit) ? unitPresenters[unit] : null;

        return null;
    }

    #endregion

    #region Debug Command Registration
    private void AddDebugCommands()
    {
        DebugLogConsole.AddCommandInstance("debug.listUnits", "Lists all spawned units with IDs.", "ListUnits", this);
    }

    #endregion
}


public interface IUnitContainer
{
    void AddUnit(UnitModel unit);
    void RemoveUnit(UnitModel unit);
    List<UnitModel> GetUnits();
    string GetContainerName();
}

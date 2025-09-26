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
    public Dictionary<string, UnitModel> spawnedUnits = new Dictionary<string, UnitModel>();
    public Dictionary<string, Unit> spawnedPresenter = new Dictionary<string, Unit>();

    // View instances (key = UnitModel.uniqueId)
    public Dictionary<string, UnitView> spawnedUnitViews = new Dictionary<string, UnitView>();
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
        foreach (var view in spawnedUnitViews.Values)
        {
            if (view != null) Destroy(view.gameObject);
        }
        spawnedUnitViews.Clear();
        spawnedPresenter.Clear();
        spawnedUnits.Clear();
    }


    // 동일한 ID가 이미 존재하면 -2, -3...을 붙여 유니크 보장
    private string EnsureUniqueInstanceId(string desired)
    {
        if (!spawnedUnits.ContainsKey(desired)) return desired;
        int suffix = 2;
        string candidate = $"{desired}-{suffix}";
        while (spawnedUnits.ContainsKey(candidate))
        {
            suffix++;
            candidate = $"{desired}-{suffix}";
        }
        Debug.Log($"Duplicate unit id '{desired}' detected. Using '{candidate}' instead.");
        return candidate;
    }


    #endregion

    #region Public API - Creation

    public UnitData GetUnitDataByName(string unitName)
    {
        return unitDataList.Find(unit => unit.unitName == unitName);
    }

    #endregion

    #region Debug Listing
    public void ListUnits()
    {
        if (spawnedUnits.Count == 0) { Debug.Log("No units spawned."); return; }
        foreach (var kv in spawnedUnits)
        {
            var m = kv.Value;
            Debug.Log($"{m.uniqueId} | {m.Data.unitName} | {m.position} | {m.locationId} | owner={m.membership}");
        }
    }

    #endregion

    #region Presenter / Lookup
    public Unit GetPresenterById(string unitId)
    {
        if (spawnedPresenter.TryGetValue(unitId, out var presenter))
            return presenter;

        if (spawnedUnits.TryGetValue(unitId, out UnitModel unit))
        {
            return GetPresenterByModel(unit);
        }
        return null;
    }

    public Unit GetPresenterByModel(UnitModel unitModel)
    {
        if (spawnedPresenter.TryGetValue(unitModel.uniqueId, out var presenter))
            return presenter;

        presenter = new Unit(unitModel, null);
        if (spawnedUnitViews.TryGetValue(unitModel.uniqueId, out UnitView view))
        {
            presenter.BindView(view);
        }
        spawnedPresenter[unitModel.uniqueId] = presenter;
        return presenter;
    }

    #endregion

    #region Debug Command Registration
    private void AddDebugCommands()
    {
        DebugLogConsole.AddCommandInstance("debug.listUnits", "Lists all spawned units with IDs.", "ListUnits", this);
        DebugLogConsole.AddCommandInstance("debug.initUnitsFromJson", "Initializes units from Resources JSON. usage: debug.initUnitsFromJson <ResourcesPath>", "InitializeUnitsFromJson", this);
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

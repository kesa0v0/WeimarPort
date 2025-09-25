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

    public void InitializeUnitsFromSpecs(IEnumerable<UnitSpawnSpec> specs)
    {
        if (specs == null) return;
        foreach (var spec in specs)
        {
            if (spec == null || string.IsNullOrEmpty(spec.unitName)) continue;
            int spawnCount = Mathf.Max(1, spec.count);
            // id가 지정되어 있고 count>1이면, id-1, id-2... 형태로 부여. id 미지정이면 unitName-1.. 로 부여
            for (int i = 0; i < spawnCount; i++)
            {
                string baseId = string.IsNullOrEmpty(spec.id) ? spec.unitName : spec.id;
                string desired = spawnCount == 1 ? baseId : $"{baseId}-{i + 1}";
                string instanceId = EnsureUniqueInstanceId(desired);
                CreateUnitFromSpec(spec, instanceId);
            }
        }
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

    public void InitializeUnitsFromJson(string resourcesPath)
    {
        var ta = Resources.Load<TextAsset>(resourcesPath);
        if (ta == null)
        {
            Debug.LogError($"UnitManager: Could not load TextAsset at Resources/{resourcesPath}");
            return;
        }
        try
        {
            var raw = JsonUtility.FromJson<UnitSpawnSpecJsonList>(ta.text);
            if (raw == null || raw.units == null)
            {
                Debug.LogWarning("UnitManager: Parsed empty unit spec list.");
                return;
            }

            var cooked = new List<UnitSpawnSpec>(raw.units.Count);
            foreach (var r in raw.units)
            {
                if (r == null || string.IsNullOrEmpty(r.unitName)) continue;
                UnitPosition pos;
                if (!Enum.TryParse(r.position, true, out pos))
                {
                    if (int.TryParse(r.position, out int ival) && Enum.IsDefined(typeof(UnitPosition), ival))
                        pos = (UnitPosition)ival;
                    else
                        pos = UnitPosition.Unavailable;
                }
                cooked.Add(new UnitSpawnSpec
                {
                    id = r.id,
                    unitName = r.unitName,
                    membership = r.membership,
                    position = pos,
                    locationId = r.locationId,
                    count = Math.Max(1, r.count)
                });
            }

            ClearAllUnits();
            InitializeUnitsFromSpecs(cooked);
            Debug.Log($"UnitManager: Initialized {cooked.Count} spec lines from JSON.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"UnitManager: Failed to parse unit specs JSON. {ex.Message}");
        }
    }

    #endregion

    #region Public API - Creation

    private Unit CreateUnitFromSpec(UnitSpawnSpec spec, string instanceId = null)
    {
        var data = GetUnitDataByName(spec.unitName);
        if (data == null)
        {
            Debug.LogWarning($"UnitManager: UnitData '{spec.unitName}' not found.");
            return null;
        }
        var model = new UnitModel(data, instanceId, spec.membership, spec.position, spec.locationId);
        spawnedUnits.Add(model.uniqueId, model);
        var presenter = new Unit(model, null);
        spawnedPresenter[model.uniqueId] = presenter;

        switch (model.position)
        {
            case UnitPosition.Unavailable:
                break;
            case UnitPosition.InReserved:
                {
                    break;
                }
            case UnitPosition.OnBoard:
                {
                    break;
                }
        }

        return presenter;
    }

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
    IList<UnitModel> ContainedUnits { get; }
    void AddUnit(UnitModel unit);
    void RemoveUnit(UnitModel unit);
    List<UnitModel> GetUnits();
    string GetContainerName();
}

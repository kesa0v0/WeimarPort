using System;
using System.Collections.Generic;
using UnityEngine;
using IngameDebugConsole;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    public List<UnitData> unitDataList; // 에디터에서 할당
    public Dictionary<string, UnitModel> spawnedUnits = new Dictionary<string, UnitModel>();
    public Dictionary<string, UnitPresenter> spawnedPresenter = new Dictionary<string, UnitPresenter>();

    // 화면에 생성된 '시각적 표현' 목록
    // Key: UnitModel의 uniqueId, Value: 생성된 UnitView 게임오브젝트
    public Dictionary<string, BaseUnitView> spawnedUnitViews = new Dictionary<string, BaseUnitView>();

    public void Awake()
    {
        // 간단한 싱글톤 설정
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        AddDebugCommands();
    }


    // --- Public API ---

    public UnitData GetUnitDataByName(string unitName)
    {
        return unitDataList.Find(unit => unit.unitName == unitName);
    }

    // 새로운 유닛 데이터를 생성하고 관제 목록에 추가
    public UnitModel CreateUnitDataByString(string unitName)
    {
        UnitData unitData = GetUnitDataByName(unitName);
        if (unitData == null)
        {
            Debug.LogWarning($"UnitData with name '{unitName}' not found.");
            return null;
        }
        return CreateUnitData(unitData);
    }
    public UnitModel CreateUnitData(UnitData unitData)
    {
        var newUnit = new UnitModel(unitData);
        
        spawnedUnits.Add(newUnit.uniqueId, newUnit);
        
        // Presenter 즉시 생성 및 캐시 (뷰는 필요할 때 생성)
        var presenter = new UnitPresenter(newUnit, null);
        spawnedPresenter[newUnit.uniqueId] = presenter;

        Debug.Log($"Unit created: {newUnit.Data.name} (ID: {newUnit.uniqueId}) with presenter cached");
        return newUnit;
    }

    // 유닛을 플레이어의 손으로 이동시키는 '명령'
    public void MoveUnitToHandByName(string unitId, string playerId)
    {
        UnitPresenter presenter = GetPresenterById(unitId);
        if (presenter == null)
        {
            Debug.LogWarning($"No presenter found for Unit ID: {unitId}. Tip: use 'debug.listUnits' to see available IDs, or 'debug.moveUnitTypeToHand <unitName> <playerId>' to pick by type.");
            return;
        }
        MainParty player = PartyRegistry.GetPartyByName(playerId) as MainParty;
        if (player == null)
        {
            Debug.LogWarning($"No main party found with ID: {playerId}");
            return;
        }
        MoveUnitToHand(presenter, player);
    }

    // 타입(이름) 기준으로 InPool 상태의 유닛을 찾아 손으로 이동
    public void MoveUnitTypeToHand(string unitName, string playerId)
    {
        MainParty player = PartyRegistry.GetPartyByName(playerId) as MainParty;
        if (player == null)
        {
            Debug.LogWarning($"No main party found with ID: {playerId}");
            return;
        }

        // InPool 상태이면서 해당 타입인 첫 번째 유닛 선택
        foreach (var kv in spawnedUnits)
        {
            var model = kv.Value;
            if (model.Data != null && model.Data.unitName == unitName && model.position == UnitPosition.InPool)
            {
                var presenter = GetPresenterByModel(model);
                if (presenter == null)
                {
                    Debug.LogWarning($"Presenter not found for Unit ID: {model.uniqueId}");
                    return;
                }
                MoveUnitToHand(presenter, player);
                Debug.Log($"Moved {model.uniqueId} to {player.partyName}'s hand.");
                return;
            }
        }
        Debug.LogWarning($"No unit in pool found with type '{unitName}'.");
    }

    // 디버그용: 현재 관리 중인 유닛을 나열
    public void ListUnits()
    {
        if (spawnedUnits.Count == 0)
        {
            Debug.Log("No units spawned.");
            return;
        }
        foreach (var kv in spawnedUnits)
        {
            var m = kv.Value;
            string nm = m.Data != null ? m.Data.unitName : "(null)";
            Debug.Log($"{m.uniqueId} | {nm} | pos={m.position} | loc={m.locationId}");
        }
    }

    public void MoveUnitToHand(UnitPresenter unit, MainParty player)
    {
        if (unit.Model.position != UnitPosition.InPool)
        {
            Debug.LogWarning($"Unit {unit.Model.uniqueId} is not in pool and cannot be moved to hand.");
            return;
        }

        // 1. 데이터(Model)의 상태 변경
        unit.Model.locationId = player.ToString();
        unit.Model.position = UnitPosition.InReserved;

        // 1.5. Presenter에 cache update
        unit.UpdateLocation(player);

        // 2. 시각적 표현(View): 손(UI)으로 갈 때, 뷰가 없으면 UI 뷰 생성
        if (!spawnedUnitViews.ContainsKey(unit.Model.uniqueId))
        {
            var newView = UnitFactory.SpawnUnitView(unit);
            if (newView != null)
            {
                spawnedUnitViews.Add(unit.Model.uniqueId, newView);
            }
        }
    }

    // 유닛을 특정 도시로 이동시키는 '명령'
    public void MoveUnitToCityByName(string unitId, string cityName)
    {
        if (!spawnedUnits
.TryGetValue(unitId, out UnitModel unit)) return;
        UnitPresenter presenter = GetPresenterById(unitId);
        if (presenter == null)
        {
            Debug.LogWarning($"No presenter found for Unit ID: {unitId}");
            return;
        }
        CityPresenter city = CityManager.Instance.GetCity(cityName);
        if (city == null)
        {
            Debug.LogWarning($"No city found with name: {cityName}");
            return;
        }

        MoveUnitToCity(presenter, city);
    }

    public void MoveUnitToCity(UnitPresenter unit, CityPresenter city)
    {
        // 1. 데이터(Model)의 상태를 먼저 변경
        unit.Model.position = UnitPosition.OnBoard;
        unit.Model.locationId = city.model.cityName;

        // 1.5. Presenter에 cache update
        unit.UpdateLocation(city);

        // 2. 시각적 표현(View)을 처리
        // 만약 유닛이 손에 있어서 View가 없었다면, 새로 생성!
        if (!spawnedUnitViews.ContainsKey(unit.Model.uniqueId))
        {
            // UnitFactory를 통해 View를 생성하고 spawnedUnitViews에 등록
            var newView = UnitFactory.SpawnUnitView(unit);
            spawnedUnitViews.Add(unit.Model.uniqueId, newView);
        }

        // 3. View에게 도시로 이동하라고 지시
        spawnedUnitViews[unit.Model.uniqueId].AttachToCity(city);

        // 4. 이벤트 발행: "유닛이 도시로 이동했다!"
        // EventBus.Instance.UnitMovedToCity(unit, cityName);
    }

    public UnitPresenter GetPresenterById(string unitId)
    {
        if (spawnedPresenter.TryGetValue(unitId, out var presenter))
            return presenter;

        if (spawnedUnits.TryGetValue(unitId, out UnitModel unit))
        {
            return GetPresenterByModel(unit);
        }
        return null;
    }

    public UnitPresenter GetPresenterByModel(UnitModel unitModel)
    {
        // 캐시된 Presenter가 있으면 반환
        if (spawnedPresenter.TryGetValue(unitModel.uniqueId, out var presenter))
            return presenter;

        // 없으면 새 Presenter를 만들되, 뷰가 있으면 바인딩
        presenter = new UnitPresenter(unitModel, null);
        if (spawnedUnitViews.TryGetValue(unitModel.uniqueId, out BaseUnitView view))
        {
            presenter.BindView(view);
        }
        spawnedPresenter[unitModel.uniqueId] = presenter;
        return presenter;
    }

    private void AddDebugCommands()
    {
        DebugLogConsole.AddCommandInstance("debug.addUnitToPool", "Adds a unit to the pool. usage: debug.addUnitToPool <unitName>", "CreateUnitDataByString", this);
        DebugLogConsole.AddCommandInstance("debug.moveUnitToHand", "Moves a unit from pool to player's hand. usage: debug.moveUnitToHand <unitId> <playerId>", "MoveUnitToHandByName", this);
        DebugLogConsole.AddCommandInstance("debug.moveUnitToCity", "Moves a unit to a city. usage: debug.moveUnitToCity <unitId> <cityName>", "MoveUnitToCityByName", this);
        DebugLogConsole.AddCommandInstance("debug.moveUnitTypeToHand", "Moves a unit (by type) from pool to player's hand. usage: debug.moveUnitTypeToHand <unitName> <playerId>", "MoveUnitTypeToHand", this);
        DebugLogConsole.AddCommandInstance("debug.listUnits", "Lists all spawned units with IDs.", "ListUnits", this);
    }
}


/// <summary>
/// 게임 내에서 유닛을 포함할 수 있는 모든 객체(도시, 플레이어 핸드 등)가 구현해야 하는 인터페이스입니다.
/// </summary>
public interface IUnitContainer
{
    /// <summary>
    /// 이 컨테이너가 현재 담고 있는 유닛들의 목록입니다. (읽기 전용)
    /// </summary>
    Dictionary<UnitPresenter, int> ContainedUnits { get; }


    /// <summary>
    /// 이 컨테이너에 유닛을 추가합니다.
    /// </summary>
    /// <param name="unit">추가할 유닛입니다.</param>
    void AddUnit(UnitPresenter unit);

    /// <summary>
    /// 이 컨테이너에서 유닛을 제거합니다.
    /// </summary>
    /// <param name="unit">제거할 유닛입니다.</param>
    void RemoveUnit(UnitPresenter unit);
}
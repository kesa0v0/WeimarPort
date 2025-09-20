using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    // 게임에 존재하는 모든 유닛의 '원본 데이터' 목록
    private Dictionary<string, UnitModel> allUnits = new Dictionary<string, UnitModel>();

    // 화면에 생성된 '시각적 표현' 목록
    // Key: UnitModel의 uniqueId, Value: 생성된 UnitView 게임오브젝트
    private Dictionary<string, UnitView> spawnedUnitViews = new Dictionary<string, UnitView>();

    private void Awake()
    {
        // 간단한 싱글톤 설정
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // --- Public API ---

    // 새로운 유닛 데이터를 생성하고 관제 목록에 추가
    public UnitModel CreateUnitData(UnitData unitType)
    {
        var newUnit = new UnitModel(unitType);
        allUnits.Add(newUnit.uniqueId, newUnit);
        Debug.Log($"Unit data created: {unitType} (ID: {newUnit.uniqueId})");
        return newUnit;
    }

    // 유닛을 특정 도시로 이동시키는 '명령'
    public void MoveUnitToCity(string unitId, string cityName)
    {
        if (!allUnits.TryGetValue(unitId, out UnitModel unit)) return;
        UnitPresenter presenter = GetPresenterById(unitId);

        // 1. 데이터(Model)의 상태를 먼저 변경
        unit.currentLocation = UnitLocation.OnBoard;
        unit.locationId = cityName;

        // 1.5. Presenter에 cache update

        presenter.UpdateLocation(CityManager.Instance.GetCity(cityName));

        // 2. 시각적 표현(View)을 처리
        // 만약 유닛이 손에 있어서 View가 없었다면, 새로 생성!
        if (!spawnedUnitViews.ContainsKey(unitId))
        {
            // UnitFactory를 통해 View를 생성하고 spawnedUnitViews에 등록
            // var newView = UnitFactory.SpawnUnitView(unit);
            // spawnedUnitViews.Add(unitId, newView);
        }

        // 3. View에게 도시로 이동하라고 지시
        // CityPresenter city = CityManager.Instance.GetCity(cityName);
        // spawnedUnitViews[unitId].AttachToCity(city.View.transform);

        // 4. 이벤트 발행: "유닛이 도시로 이동했다!"
        // EventBus.Instance.UnitMovedToCity(unit, cityName);
    }

    private UnitPresenter GetPresenterById(string unitId)
    {
        if (allUnits.TryGetValue(unitId, out UnitModel unit))
        {
            return GetPresenterForModel(unit);
        }
        return null;
    }

    // 유닛을 플레이어의 손으로 이동시키는 '명령'
    public void MoveUnitToHand(string unitId, int playerId)
    {
        if (!allUnits.TryGetValue(unitId, out UnitModel unit)) return;

        // 1. 데이터(Model)의 상태 변경
        unit.currentLocation = UnitLocation.InPlayerHand;
        unit.locationId = playerId.ToString();

        // 2. 시각적 표현(View)이 있었다면, 파괴하거나 숨김
        if (spawnedUnitViews.TryGetValue(unitId, out UnitView view))
        {
            Destroy(view.gameObject);
            spawnedUnitViews.Remove(unitId);
        }

        // 3. 이벤트 발행: "유닛이 핸드로 들어왔다!"
        // EventBus.Instance.UnitAddedToHand(unit, playerId);
    }

    public UnitPresenter GetPresenterForModel(UnitModel unitModel)
    {
        if (spawnedUnitViews.TryGetValue(unitModel.uniqueId, out UnitView view))
        {
            return new UnitPresenter(unitModel, view);
        }
        else
        {
            Debug.LogWarning($"No UnitView found for UnitModel ID: {unitModel.uniqueId}");
            return null;
        }
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
    IReadOnlyList<UnitPresenter> ContainedUnits { get; }


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
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    // 게임에 존재하는 모든 유닛의 '원본 데이터' 목록
    private Dictionary<int, UnitModel> allUnits = new Dictionary<int, UnitModel>();

    // 화면에 생성된 '시각적 표현' 목록
    // Key: UnitModel의 uniqueId, Value: 생성된 UnitView 게임오브젝트
    private Dictionary<int, UnitView> spawnedUnitViews = new Dictionary<int, UnitView>(); 

    private void Awake()
    {
        Instance = this;
    }

    // --- Public API ---

    // 새로운 유닛 데이터를 생성하고 관제 목록에 추가
    public UnitModel CreateUnitData(string unitType)
    {
        var newUnit = new UnitModel(unitType);
        allUnits.Add(newUnit.uniqueId, newUnit);
        Debug.Log($"Unit data created: {unitType} (ID: {newUnit.uniqueId})");
        return newUnit;
    }

    // 유닛을 특정 도시로 이동시키는 '명령'
    public void MoveUnitToCity(int unitId, string cityName)
    {
        if (!allUnits.TryGetValue(unitId, out UnitModel unit)) return;

        // 1. 데이터(Model)의 상태를 먼저 변경
        unit.currentLocation = UnitLocation.OnBoard;
        unit.locationId = cityName;

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
    
    // 유닛을 플레이어의 손으로 이동시키는 '명령'
    public void MoveUnitToHand(int unitId, int playerId)
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
}
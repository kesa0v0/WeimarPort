using System;
using System.Collections.Generic;
using UnityEngine;
using IngameDebugConsole;

public class GameManager : MonoBehaviour
{
    public GameState gameState;
    private PlayerActionState currentState = PlayerActionState.None;
    private UnitPresenter selectedUnit; // 현재 선택된 유닛
    private Action<CityPresenter> onCitySelectedCallback; 
    
    public static GameManager Instance { get; private set; }
    


    private void Awake()
    {
        // 간단한 싱글톤 설정
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        gameState = new GameState();

        // 랜덤하게 플레이어 정당 설정 (나중에 UI로 선택 가능하게 변경 예정)
        gameState.playerParty = PartyRegistry.AllMainParties[UnityEngine.Random.Range(0, PartyRegistry.AllMainParties.Count)];

        // 랜덤하게 첫 플레이어 정당 설정 (나중에 UI로 선택 가능하게 변경 예정)
        // 그리고 파티 턴 순서도 설정
        gameState.firstPlayerParty = PartyRegistry.AllMainParties[UnityEngine.Random.Range(0, PartyRegistry.AllMainParties.Count)];
        gameState.partyTurnOrder.AddRange(PartyRegistry.AllMainParties);
        ShuffleList(gameState.partyTurnOrder);

        // 최초 집권연정은 SPD와 Zentrum으로 설정 < 나중에 불러오기나 UI로 선택 가능하게 변경 예정
        var coalition = new List<MainParty>
        {
            PartyRegistry.SPD,
            PartyRegistry.Zentrum
        };
        gameState.government.SetRulingCoalition(coalition);

        UIManager.Instance.partyStatusManager.Initialize(PartyRegistry.AllMainParties);


        CityManager.Instance.CreateCities();
        // 유닛 초기화: 데이터 리스트를 기반으로 초기 배치
        UnitManager.Instance.InitializeUnitsFromJson("scenarios/RepublicOnTheBrink");
        // 정부 패널 초기 렌더
        UIManager.Instance?.governmentPanel?.Redraw();
        AddDebugCommands();
        TestScript();
    }

    void TestScript()
    {

    }

    private void AddDebugCommands()
    {
        DebugLogConsole.AddCommandInstance(
            "debug.setGovernmentCoalition",
            "Sets government coalition. Usage: debug.setGovernmentCoalition <Primary> [Secondary]",
            nameof(CmdSetGovernmentCoalition), this);

        DebugLogConsole.AddCommandInstance(
            "debug.redrawGovernment",
            "Redraws the Government panel UI.",
            nameof(CmdRedrawGovernment), this);
    }

    public void CmdRedrawGovernment()
    {
        UIManager.Instance?.governmentPanel?.Redraw();
    }

    public void CmdSetGovernmentCoalition(string primary, string secondary = null)
    {
        var p1 = PartyRegistry.GetPartyByName(primary) as MainParty;
        MainParty p2 = null;
        if (!string.IsNullOrEmpty(secondary))
            p2 = PartyRegistry.GetPartyByName(secondary) as MainParty;

        if (p1 == null)
        {
            Debug.LogWarning($"Primary party '{primary}' not found or not a MainParty.");
            return;
        }

        if (p2 == null && !string.IsNullOrEmpty(secondary))
        {
            Debug.LogWarning($"Secondary party '{secondary}' not found or not a MainParty. Proceeding with single-party government.");
        }

        gameState.government.SetRulingCoalition(p1, p2);
        // Setters already trigger redraw, but ensure anyway
        UIManager.Instance?.governmentPanel?.Redraw();
        Debug.Log($"Government coalition set to: {p1.partyName}{(p2 != null ? "+" + p2.partyName : "")}");
    }

    /// <summary>
    /// 외부에서 "도시 선택 모드"로 진입을 요청하는 메소드
    /// </summary>
    /// <param name="newState">어떤 목적으로 선택하는지 (행동 모드)</param>
    /// <param name="callback">선택이 완료되었을 때 실행할 행동</param>
    public void EnterCitySelectionMode(PlayerActionState newState, Action<CityPresenter> callback)
    {
        currentState = newState;
        onCitySelectedCallback = callback;
        Debug.Log($"도시 선택 모드 진입: {newState}. 도시를 선택해주세요.");
        // 여기서 "도시를 선택하세요" 같은 UI 텍스트를 띄워주면 좋습니다.
        // UIManager.Instance.ShowActionPrompt("도시를 선택하세요...");

        // 신규 SelectionManager와 병행 사용 가능: 기존 흐름 호환성 유지
        SelectionManager.Instance.RequestSingleCity(city => {
            onCitySelectedCallback?.Invoke(city);
            ResetToActionStateNone();
        });
    }

    
    public void OnUnitClicked(UnitPresenter unit)
    {
        // SelectionManager 우선 처리
        if (SelectionManager.Instance.IsActive && SelectionManager.Instance.CurrentType == SelectionType.Unit)
        {
            if (SelectionManager.Instance.HandleUnitClicked(unit)) return;
        }
        // TODO: 유닛을 선택하는 더 정교한 로직 (예: 내 턴, 내 유닛인지 확인)
        selectedUnit = unit;
        // 선택 효과 표시
        unit.ShowAsSelected(true);
        Debug.Log($"{unit.Model.Data.unitName} (ID: {unit.Model.uniqueId}) selected.");

        EnterCitySelectionMode(
            PlayerActionState.SelectingCityForUnitMove,
            (selectedCity) => {
                UnitManager.Instance.MoveUnitToCity(unit, selectedCity);
            }
    );
}

    /// <summary>
    /// 도시가 클릭되었을 때 View가 호출하는 유일한 공용 메소드
    /// </summary>
    public void OnCityClicked(CityPresenter city)
    {
        // SelectionManager 우선 처리
        if (SelectionManager.Instance.IsActive && SelectionManager.Instance.CurrentType == SelectionType.City)
        {
            if (SelectionManager.Instance.HandleCityClicked(city)) return;
        }
        // 현재 특별한 선택 모드가 아니라면 아무것도 하지 않음
        if (currentState == PlayerActionState.None) return;

        // 저장해 두었던 콜백(행동)이 있다면 실행!
        onCitySelectedCallback?.Invoke(city);
        
        // 행동을 완료했으므로 기본 상태로 돌아감
        ResetToActionStateNone();
    }

    private void ResetToActionStateNone()
    {
        currentState = PlayerActionState.None;
        onCitySelectedCallback = null;
        Debug.Log("기본 상태로 돌아갑니다.");
        // UIManager.Instance.HideActionPrompt();
    }

    public void OnAddSeatButtonClicked()
    {
        // GameManager에게 "의석 추가를 위한 도시 선택 모드로 들어가줘."
        // "그리고 도시가 선택되면, 이 함수(AddSeatToSelectedCity)를 실행해줘." 라고 요청
        GameManager.Instance.EnterCitySelectionMode(
            PlayerActionState.SelectingCityForAddSeat, 
            (selectedCity) => {
                // 이 부분이 람다(lambda) 식으로 작성된 콜백 함수입니다.
                Debug.Log($"{selectedCity.model.cityName}에 의석을 추가합니다.");
                // selectedCity.AddSeat(currentParty, 1);
            }
        );
    }

    public void RequestPartySelection(List<Party> candidates, int count, System.Action<List<Party>> onChosen)
    {
        UIManager.Instance.partyStatusManager.RequestPartySelection(candidates, count, onChosen);
    }


    #region Helper Methods

    // Fisher-Yates shuffle for lists
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
    public List<MainParty> GetCurrentRoundPartyOrder()
    {
        var order = new List<MainParty>();
        int count = gameState.partyTurnOrder.Count;
        for (int i = 0; i < count; i++)
        {
            int index = (gameState.partyTurnOrder.IndexOf(gameState.firstPlayerParty) + i) % count;
            order.Add(gameState.partyTurnOrder[index]);
        }
        return order;
    }

    #endregion
}

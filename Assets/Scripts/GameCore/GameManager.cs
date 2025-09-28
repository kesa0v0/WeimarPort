using System;
using System.Collections.Generic;
using UnityEngine;
using IngameDebugConsole;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public GameState gameState;
    private PlayerActionState currentState = PlayerActionState.IdleState;

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

        PartyManager.Instance.Initialize();
        UIManager.Instance.partyStatusManager.Initialize(gameState.Parties); // 임시로 일단 이렇게 해 놓음. EventBus에서 Initialize(GameData) 호출하도록 변경 예정

        // 랜덤하게 플레이어 정당 설정 (나중에 UI로 선택 가능하게 변경 예정)
        gameState.GameInfo.CurrentPlayerPartyId = GetParty(FactionType.SPD).Data.factionType;

        // 랜덤하게 첫 플레이어 정당 설정 (나중에 UI로 선택 가능하게 변경 예정)
        gameState.GameInfo.RoundStartPlayerPartyId = GetParty(FactionType.SPD).Data.factionType;

        // 최초 집권연정은 SPD와 Zentrum으로 설정 < 나중에 불러오기나 UI로 선택 가능하게 변경 예정
        gameState.government.FormNewGovernment(GetParty(FactionType.SPD), GetParty(FactionType.Z));

        CityManager.Instance.CreateCities();
        // 유닛 초기화: 데이터 리스트를 기반으로 초기 배치
        
        AddDebugCommands();
        TestScript();
    }

    void AddDebugCommands()
    {

    }

    void TestScript()
    {

    }

    public void RequestPartySelection(List<FactionType> candidates, int count, Action<List<FactionType>> onChosen)
    {
        EventBus.Instance.RequestPartySelection(candidates, count, onChosen);
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
    public List<FactionType> GetCurrentRoundPartyOrder()
    {
        var order = new List<FactionType>();
        int count = gameState.partyTurnOrder.Count;
        int startIndex = gameState.partyTurnOrder.FindIndex(p => p.Data.factionType == gameState.GameInfo.RoundStartPlayerPartyId);
        for (int i = 0; i < count; i++)
        {
            int index = (startIndex + i) % count;
            order.Add(gameState.partyTurnOrder[index].Data.factionType);
        }
        return order;
    }

    public PartyModel GetParty(FactionType faction)
    {
        return gameState.Parties.FirstOrDefault(p => p.Data.factionType == faction);
    }

    #endregion
}

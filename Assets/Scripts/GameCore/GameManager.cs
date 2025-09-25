using System;
using System.Collections.Generic;
using UnityEngine;
using IngameDebugConsole;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public GameState gameState;
    private PlayerActionState currentState = PlayerActionState.IdleState;

    private Unit selectedUnit; // 현재 선택된 유닛
    private Action<City> onCitySelectedCallback;

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
        gameState.playerParty = GetParty(FactionType.SPD);

        // 랜덤하게 첫 플레이어 정당 설정 (나중에 UI로 선택 가능하게 변경 예정)
        // 그리고 파티 턴 순서도 설정
        gameState.firstPlayerParty = GetParty(FactionType.SPD);

        // 최초 집권연정은 SPD와 Zentrum으로 설정 < 나중에 불러오기나 UI로 선택 가능하게 변경 예정
        gameState.government.FormNewGovernment(GetParty(FactionType.SPD), GetParty(FactionType.Z));

        UIManager.Instance.partyStatusManager.Initialize(gameState.allParties);


        CityManager.Instance.CreateCities();
        // 유닛 초기화: 데이터 리스트를 기반으로 초기 배치
        UnitManager.Instance.InitializeUnitsFromJson("scenarios/RepublicOnTheBrink");
        // 정부 패널 초기 렌더
        UIManager.Instance?.governmentPanel?.Redraw();
        AddDebugCommands();
        TestScript();
    }

    void AddDebugCommands()
    {
        
    }

    void TestScript()
    {

    }

    public void RequestPartySelection(List<Party> candidates, int count, Action<List<Party>> onChosen)
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
    public List<Party> GetCurrentRoundPartyOrder()
    {
        var order = new List<Party>();
        int count = gameState.partyTurnOrder.Count;
        for (int i = 0; i < count; i++)
        {
            int index = (gameState.partyTurnOrder.IndexOf(gameState.firstPlayerParty) + i) % count;
            order.Add(gameState.partyTurnOrder[index]);
        }
        return order;
    }
    
    public Party GetParty(FactionType faction)
    {
        return gameState.allParties.FirstOrDefault(p => p.Data.factionType == faction);
    }

    #endregion
}

using System;
using System.Collections.Generic;
using UnityEngine;
using IngameDebugConsole;

public class GameManager : MonoBehaviour
{
    public GameState gameState;
    
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

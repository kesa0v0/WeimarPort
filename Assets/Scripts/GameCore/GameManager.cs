using System.Collections.Generic;
using UnityEngine;

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

        // 랜덤하게 첫 플레이어 정당 설정 (나중에 UI로 선택 가능하게 변경 예정)
        // 그리고 파티 턴 순서도 설정
        gameState.firstPlayerParty = PartyRegistry.AllMainParties[Random.Range(0, PartyRegistry.AllMainParties.Count)];
        gameState.partyTurnOrder.AddRange(PartyRegistry.AllMainParties);
        ShuffleList(gameState.partyTurnOrder);

        UIManager.Instance.partyStatusManager.Initialize(PartyRegistry.AllMainParties);


        CityManager.Instance.CreateCities();
        TestScript();
    }

    void TestScript()
    {

    }

    // Fisher-Yates shuffle for lists
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
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
}

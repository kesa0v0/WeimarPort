using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 게임에 참여하는 모든 정당과 정당 기지를 생성, 관리, 조회하는 중앙 관리자입니다.
/// </summary>
public class PartyManager : MonoBehaviour
{
    // --- 싱글톤 구현 ---
    public static PartyManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    [Header("프리팹 참조")]
    [SerializeField] private Transform partyBaseViewParent;

    // 관리용 Dictionary
    private Dictionary<FactionType, PartyModel> partyModelMap = new Dictionary<FactionType, PartyModel>();

    /// <summary>
    /// 모든 정당 Model을 생성하여 초기화합니다.
    /// </summary>
    public void Initialize()
    {
        var partyDatas = Resources.LoadAll<FactionData>("ScriptableObjects/Party").ToList();
        foreach (var data in partyDatas)
        {
            if (GameManager.Instance.gameState.Parties.Any(p => p.Data.factionType == data.factionType)) continue;
            var party = new PartyModel(data);
            GameManager.Instance.gameState.Parties.Add(party);
        }
    }
    
    public PartyModel GetModel(FactionType type)
    {
        partyModelMap.TryGetValue(type, out PartyModel model);
        return model;
    }
}

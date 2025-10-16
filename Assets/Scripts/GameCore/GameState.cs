using System.Collections.Generic;
using UnityEngine;

public enum RoundPhase
{
    Republic,
    Agenda,
    Impulse,
    Politics
}

[System.Serializable]
public class GameState
{
    // 게임의 전반적인 진행 정보 (룰북 p.12)
    public GameInfoModel GameInfo = new GameInfoModel();

    // 모든 정당(플레이어)의 상태 목록
    public List<PartyModel> Parties = new();
    public List<PartyModel> partyTurnOrder = new(); // ?


    // 게임에 존재하는 모든 유닛의 '마스터 목록' (Source of Truth)
    public List<UnitModel> AllUnits = new List<UnitModel>();

    // 게임에 존재하는 모든 마커의 '마스터 목록'
    public List<ThreatMarkerModel> AllMarkers = new List<ThreatMarkerModel>();

    // 타임라인 카드 덱 (룰북 p.13)
    public CardDeckModel TimelineDeck = new CardDeckModel();
    public CardDeckModel TimelineDiscard = new CardDeckModel();

    public Government government = new();
    
    // 폐기된 유닛 보관소
    public DisposedBin DisposedBin = new DisposedBin();
    
    public List<string> DRBoxMarkerInstanceIds = new List<string>();
}

[System.Serializable]
public class GameInfoModel
{
    public int CurrentRound;                // 현재 라운드 (1-6)
    public RoundPhase CurrentPhase;             // 현재 게임 단계 (e.g., "RepublicPhase", "ImpulsePhase")
    public FactionType CurrentPlayerPartyId;     // 현재 턴을 진행 중인 정당의 ID
    public FactionType RoundStartPlayerPartyId;  // 라운드 시작 플레이어
}

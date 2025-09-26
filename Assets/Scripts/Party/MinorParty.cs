using UnityEngine;

// 어떤 종류의 보너스인지 구분하기 위한 enum
public enum MinorPartyBonusType { AddReservePoint, GrantStresemannCard, AllowDDPAction }

[CreateAssetMenu(fileName = "NewMinorParty", menuName = "Weimar/Minor Party Data")]
public class MinorPartyData : ScriptableObject
{
    [Header("기본 정보")]
    public string partyName; // "독일 민주당" (DDP)
    public Sprite logo;

    [Header("제어 규칙")]
    // 이 군소 정당을 제어할 수 있는 두 개의 주요 정당
    public FactionData controllerOptionA;
    public FactionData controllerOptionB;

    [Header("혜택 정보")]
    // 라운드별 추가 의석 수 (index 0 = Round 1)
    // 예: USPD -> [2, 2, 1, 1, 0, 0]
    public int[] seatBonusByRound = new int[6];

    // 이 정당이 제공하는 고유 보너스의 종류
    public MinorPartyBonusType bonusType;
}
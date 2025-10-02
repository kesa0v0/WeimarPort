using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Weimar/Card Data")]
public class CardData : ScriptableObject
{
    [Header("Card Identification")]
    public string CardId;
    public FactionType Affiliation;

    [Header("Display Information")]
    public string CardName;
    [TextArea(3, 5)]
    public string EventText; // 룰북에 적힌 원본 텍스트
    public Sprite CardImage;

    [Header("Game Values")]
    public int LargeCircleValue;
    public int SmallCircleValue;
    public bool RemoveFromGameOnPlay;
    
    [Header("Event Logic Script")]
    [TextArea(10, 20)]
    // 이 카드의 이벤트 효과를 JSON 스크립트 형태로 저장
    public string EventScriptJson;
}



namespace Weimar.CardEffects
{
    /// <summary>
    /// 카드 이벤트 스크립트의 단일 명령어를 나타내는 클래스
    /// </summary>
    [System.Serializable]
    public class CardEffect
    {
        public string comment; // JSON 가독성을 위한 주석
        public string command; // 실행할 명령어 (e.g., ModifyVp, PlayerChoice)
        public Arguments args; // 명령어에 필요한 인자
    }

    /// <summary>
    /// 명령어에 필요한 모든 종류의 인자를 담는 범용 클래스
    /// </summary>
    [System.Serializable]
    public class Arguments
    {
        // 대상 식별용 ID
        public string partyId;
        public string targetPartyId;
        public string instanceId;
        public string dataId;

        // 수량 및 값
        public int count;
        public int value;

        // 위치 정보
        public LocationInfo location;
        
        // 조건부 효과
        public Condition condition;
        public List<CardEffect> onSuccess; // 조건 성공 시 실행할 스크립트
        public List<CardEffect> onFailure; // 조건 실패 시 실행할 스크립트

        // 플레이어 선택지
        public string choicePrompt; // 플레이어에게 보여줄 질문
        public List<ChoiceOption> options;
    }

    /// <summary>
    /// 플레이어 선택지의 각 옵션을 정의하는 클래스
    /// </summary>
    [System.Serializable]
    public class ChoiceOption
    {
        public string buttonText; // UI 버튼에 표시될 텍스트
        public List<CardEffect> effects; // 이 옵션을 선택했을 때 실행될 효과 스크립트
    }

    /// <summary>
    /// 조건부 효과를 위한 조건을 정의하는 클래스
    /// </summary>
    [System.Serializable]
    public class Condition
    {
        public string type; // e.g., "IsInGovernment", "HasThreatMarker"
        public string partyId;
        public string markerId;
        public LocationInfo location;
    }
}


public enum CardPlayOption
{
    Event,
    Debate,
    Actions
}
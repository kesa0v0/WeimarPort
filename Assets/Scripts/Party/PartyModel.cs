using System;
using System.Collections.Generic;
using UnityEngine;

public class PartyModel
{
    public FactionData Data { get; private set; }
    public int victoryPoints;
    public int reservedPoints;


    // 카드 목록 (룰북 p.11). 실제 카드 데이터는 ID로 조회
    public List<string> HandCardIds = new List<string>();
    public CardDeckModel DrawDeck = new CardDeckModel();
    public CardDeckModel DiscardPile = new CardDeckModel();

    public string ChosenAgendaCardId; // 이번 라운드에 선택한 의제 카드 ID (룰북 p.13)

    // 제어 중인 군소 정당 ID 목록 (룰북 p.11, 26)
    public List<MinorPartyData> ControlledMinorParties { get; private set; }


    // public List<Card> hand;
    
    public PartyModel(FactionData data)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
        victoryPoints = 0;
        reservedPoints = 0;
        ControlledMinorParties = new List<MinorPartyData>();
    }
}
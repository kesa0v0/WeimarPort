using System.Collections.Generic;
using UnityEngine;

public enum RoundPhase
{
    Republic,
    Agenda,
    Impulse,
    Politics
}

public class GameState
{
    public Party playerFaction;
    public Government government = new();

    public int currentRound = 1;
    public RoundPhase currentPhase = RoundPhase.Republic;
    public Party firstPlayerParty;
    public List<Party> partyTurnOrder = new();
}

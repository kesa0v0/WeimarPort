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
    public MainParty playerParty;
    public Government government = new();

    public int currentRound = 1;
    public RoundPhase currentPhase = RoundPhase.Republic;
    public MainParty firstPlayerParty;
    public List<MainParty> partyTurnOrder = new();
}

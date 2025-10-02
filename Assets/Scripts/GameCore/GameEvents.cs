using System;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using Weimar.CardEffects;

namespace Event.GameFlow
{
    // 게임 설정이 완료되었음을 알리는 이벤트
    public class GameSetupCompletedEvent : GameEvent
    {
    }

    // 라운드 시작을 알리는 이벤트
    public class RoundStartedEvent : GameEvent
    {
        public int RoundNumber { get; }
        public CardData RoundCardData { get; }
        public RoundStartedEvent(int roundNumber, CardData roundCardData)
        {
            RoundNumber = roundNumber;
            RoundCardData = roundCardData;
        }
    }

    public class PhaseChangedEvent : GameEvent
    {
        public RoundPhase PreviousePhase { get; }
        public RoundPhase CurrentPhase { get; }

        public PhaseChangedEvent(RoundPhase previousPhase, RoundPhase currentPhase)
        {
            PreviousePhase = previousPhase;
            CurrentPhase = currentPhase;
        }
    }

    // 특정 플레이어의 턴이 시작될 때 발생하는 이벤트
    public class TurnStartedEvent : GameEvent
    {
        public FactionType CurrentTurnParty { get; }

        public TurnStartedEvent(FactionType currentTurnParty)
        {
            CurrentTurnParty = currentTurnParty;
        }
    }

    // 게임이 종료되었을 때 발생하는 이벤트
    public class GameOverEvent : GameEvent
    {
        public FactionType WinningParty { get; }
        public string Reason { get; }

        public GameOverEvent(FactionType winningParty, string reason)
        {
            WinningParty = winningParty;
            Reason = reason;
        }
    }
}


namespace Event.PlayerAction
{
    // 플레이어가 손에서 카드를 냈을 때 발생하는 이벤트
    public class CardPlayedEvent : GameEvent
    {
        public FactionType Player { get; }
        public CardData Card { get; }
        public List<CardPlayOption> Option { get; }

        public CardPlayedEvent(FactionType player, CardData card, List<CardPlayOption> option)
        {
            Player = player;
            Card = card;
            Option = option;
        }
    }

    // '토론' 액션으로 이슈 마커를 움직였을 때 발생하는 이벤트
    public class DebateActionEvent : GameEvent
    {
        public FactionType SourcePlayer { get; }
        public Issue Marker1 { get; }
        public int MoveValue1 { get; }
        public Issue Marker2 { get; }
        public int MoveValue2 { get; }
        public int ReservePointsSpent { get; }

        public DebateActionEvent(FactionType sourcePlayer, Issue marker1, int moveValue1, Issue marker2, int moveValue2, int reservePointsSpent)
        {
            SourcePlayer = sourcePlayer;
            Marker1 = marker1;
            MoveValue1 = moveValue1;
            Marker2 = marker2;
            MoveValue2 = moveValue2;
            ReservePointsSpent = reservePointsSpent;
        }
    }

    // 도시에서 '행동'을 수행하기 위해 행동력(AP)을 얻었을 때 발생하는 이벤트
    public class CityActionInitiatedEvent : GameEvent
    {
        public FactionType SourcePlayer { get; }
        public CityModel TargetCity { get; }
        public int ActionPoints { get; }

        public CityActionInitiatedEvent(FactionType sourcePlayer, CityModel targetCity, int actionPoints)
        {
            SourcePlayer = sourcePlayer;
            TargetCity = targetCity;
            ActionPoints = actionPoints;
        }
    }

    // 다른 플레이어의 행동에 '대응'했을 때 발생하는 이벤트
    public class PlayerReactedEvent : GameEvent
    {
        public FactionType ReactingPlayer { get; }
        public GameEvent TriggeringEvent { get; }
        public ReactionType Type { get; }

        public PlayerReactedEvent(FactionType reactingPlayer, GameEvent triggeringEvent, ReactionType type)
        {
            ReactingPlayer = reactingPlayer;
            TriggeringEvent = triggeringEvent;
            Type = type;
        }

        public enum ReactionType // 대응의 종류 (TODO: 나중에 Reaction 만들면 옮기기)
        {
            Politician,
            Card
        }
    }
}


namespace Event.StateChange.Seats
{
    // 도시에 정당 기반이 설치되었음을 알리는 이벤트
    public class PartyBasePlacedEvent : GameEvent
    {
        public FactionType Owner { get; } // Player는 정당 정보를 담은 클래스/enum이라 가정
        public CityModel TargetCity { get; } // City는 도시 정보를 담은 클래스/enum이라 가정

        public PartyBasePlacedEvent(FactionType owner, CityModel targetCity)
        {
            Owner = owner;
            TargetCity = targetCity;
        }
    }

    public class PartyBaseRemovedEvent : GameEvent
    {
        public FactionType Owner { get; }
        public CityModel TargetCity { get; }

        public PartyBaseRemovedEvent(FactionType owner, CityModel targetCity)
        {
            Owner = owner;
            TargetCity = targetCity;
        }
    }

    public class ParliamentSeatGainedEvent : GameEvent
    {
        public FactionType Party { get; }
        public int SeatsGained { get; }

        public ParliamentSeatGainedEvent(FactionType party, int seatsGained)
        {
            Party = party;
            SeatsGained = seatsGained;
        }
    }
    
    public class ParliamentSeatLostEvent : GameEvent
    {
        public FactionType Party { get; }
        public int SeatsLost { get; }

        public ParliamentSeatLostEvent(FactionType party, int seatsLost)
        {
            Party = party;
            SeatsLost = seatsLost;
        }
    }
}

namespace Event.StateChange.Markers
{
    public class ThreatMarkerPlacedEvent : GameEvent
    {
        public ThreatMarkerData Marker { get; }
        public LocationData Location { get; }

        public ThreatMarkerPlacedEvent(ThreatMarkerData marker, LocationData location)
        {
            Marker = marker;
            Location = location;
        }
    }

    public class ThreatMarkerRemovedEvent : GameEvent
    {
        public ThreatMarkerData Marker { get; }
        public LocationData Location { get; }

        public ThreatMarkerRemovedEvent(ThreatMarkerData marker, LocationData location)
        {
            Marker = marker;
            Location = location;
        }
    }

    public class IssueMarkerPlacedEvent : GameEvent
    {
        public Issue Marker { get; }
        public LocationData Location { get; }

        public IssueMarkerPlacedEvent(Issue marker, LocationData location)
        {
            Marker = marker;
            Location = location;
        }
    }

    public class IssueMarkerMovedEvent : GameEvent
    {
        public Issue Marker { get; }
        public LocationData Location { get; }

        public IssueMarkerMovedEvent(Issue marker, LocationData location)
        {
            Marker = marker;
            Location = location;
        }
    }

    public class SocietyMarkerPlacedEvent : GameEvent
    {
        public SocietyMarkerData Marker { get; }
        public LocationData Location { get; }

        public SocietyMarkerPlacedEvent(SocietyMarkerData marker, LocationData location)
        {
            Marker = marker;
            Location = location;
        }

        public class SocietyMarkerData { } // TODO: 실제 SocietyMarkerData 정의로 교체
    }
}

namespace Event.StateChange.Tracks
{
    public class EconomyTrackMovedEvent : GameEvent
    {
        public int NewPosition { get; }
        public EconomyTrackMovedEvent(int newPosition)
        {
            NewPosition = newPosition;
        }
    }

    public class ForeignAffairsTrackMovedEvent : GameEvent
    {
        public int NewPosition { get; }
        public ForeignAffairsTrackMovedEvent(int newPosition)
        {
            NewPosition = newPosition;
        }
    }

    public class NSDAPTrackAdvancedEvent : GameEvent
    {
        public FactionType ResponsibleParty { get; } // NSDAP 트랙을 진전시킨 정당
        public int NewPosition { get; }
        public NSDAPTrackAdvancedEvent(int newPosition)
        {
            NewPosition = newPosition;
        }
    }
}

namespace Event.StateChange.Units
{
    public class UnitMobilizedEvent : GameEvent
    {
        UnitData Unit { get; }
        LocationData From { get; }
        CityModel To { get; }
        public UnitMobilizedEvent(UnitData unit, LocationData from, CityModel to)
        {
            Unit = unit;
            From = from;
            To = to;
        }
    }

    public class UnitDissolvedEvent : GameEvent
    {
        UnitData Unit { get; }
        CityModel Location { get; } // 해산된 도시
        public UnitDissolvedEvent(UnitData unit, CityModel location)
        {
            Unit = unit;
            Location = location;
        }
    }

    public class ReichswehrControlChangedEvent : GameEvent
    {
        public UnitData Unit { get; }
        public FactionType NewControllingParty { get; }
        public ReichswehrControlChangedEvent(UnitData unit, FactionType newControllingParty)
        {
            Unit = unit;
            NewControllingParty = newControllingParty;
        }
    }
}


namespace Event.StateChange.Government
{
    public class GovernmentFormedEvent : GameEvent
    {
        public List<FactionType> CoalitionParties { get; }
        public FactionType LeadingParty { get; }

        public GovernmentFormedEvent(List<FactionType> coalitionParties, FactionType leadingParty)
        {
            CoalitionParties = coalitionParties;
            LeadingParty = leadingParty;
        }
    }

    public class ForeignAffairsNegotiatedEvent : GameEvent
    {
        CardData Card { get; } // TODO: ForeignAffairs class로 교체
        bool Success { get; }
        public ForeignAffairsNegotiatedEvent(CardData card, bool success)
        {
            Card = card;
            Success = success;
        }
    }
}

namespace Event.UI
{
    public class RequestSelectionEvent<T> : GameEvent
    {
        public PlayerSelectionType Instruction { get; }
        public List<T> Items { get; }
        public Guid RequestId { get; }
        public RequestSelectionEvent(PlayerSelectionType instruction, List<T> items)
        {
            Instruction = instruction;
            Items = items;
            RequestId = Guid.NewGuid();
        }
    }

    public class SelectionMadeEvent<T> : GameEvent
    {
        public T SelectedItem { get; }
        public Guid RequestId { get; }

        public SelectionMadeEvent(T selectedItem, Guid requestId)
        {
            SelectedItem = selectedItem;
            RequestId = requestId;
        }
    }
}

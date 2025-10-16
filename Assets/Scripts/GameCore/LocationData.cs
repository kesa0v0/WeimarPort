using System.Numerics;

public enum LocationType
{
    Unavailable,
    Supply,
    City,
    Parliament,
    DR_Box,
    DissolvedArea,
    OpinionTrack,
    SocietyTrack
}

[System.Serializable]
public class LocationData
{
    public LocationType Type;
    public string Name; // City 이름, PartyId (Supply의 경우) 등
    public Vector2 opinionTrackPosition; // Opinion Track 상의 위치 (Type이 OpinionTrack일 경우)
    public int societyTrackPosition; // Society Track 상의 위치 (Type이 SocietyTrack일 경우)
}
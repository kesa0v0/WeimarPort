public enum LocationType
{
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
}
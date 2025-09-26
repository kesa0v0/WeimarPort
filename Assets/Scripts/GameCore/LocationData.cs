public enum LocationType
{
    Supply,
    City,
    Parliament,
    DRBox,
    DissolvedArea,
    OpinionTrack,
    SocietyTrack
}

[System.Serializable]
public struct LocationData
{
    public LocationType Type;
    public string Name; // City 이름, PartyId (Supply의 경우) 등
}
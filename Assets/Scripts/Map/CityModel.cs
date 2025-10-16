using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class CityModel: IUnitContainer
{
    public string cityName;
    public Vector2 position;

    public int seatMaxCount;
    public int currentSeats => PartyBasesCounts.Values.Sum();
    public Dictionary<FactionType, int> PartyBasesCounts { get; private set; }

    // 도시에 존재하는 유닛 및 마커의 '인스턴스 ID' 목록
    public List<string> UnitInstanceIds { get; private set; } = new List<string>();
    public List<string> ThreatMarkerInstanceIds { get; private set; } = new List<string>();

    public CityModel(string name, Vector2 pos, int seatMaxCount)
    {
        cityName = name;
        position = pos;
        this.seatMaxCount = seatMaxCount;
        PartyBasesCounts = new Dictionary<FactionType, int>();
    }

    #region Seat Management

    public void AddSeat(FactionType party)
    {
        if (currentSeats >= seatMaxCount)
        {
            Debug.LogWarning("No more seats available in the city.");
            return;
        }

        if (PartyBasesCounts.ContainsKey(party))
        {
            PartyBasesCounts[party]++;
        }
        else
        {
            PartyBasesCounts[party] = 1;
        }
    }

    public void RemoveSeat(FactionType party)
    {
        if (PartyBasesCounts.ContainsKey(party) && PartyBasesCounts[party] > 0)
        {
            PartyBasesCounts[party]--;
        }
        else
        {
            Debug.LogWarning($"No seats to remove for party {party.HumanName()}.");
        }
    }

    #endregion


    #region Unit Management

    
    /// <summary>
    /// 이 도시에 유닛을 배치합니다.
    /// </summary>
    public void AddUnit(UnitModel unitModel)
    {
        UnitInstanceIds.Add(unitModel.InstanceId);
    }

    /// <summary>
    /// 이 도시에서 유닛을 제거합니다.
    /// </summary>
    public void RemoveUnit(UnitModel unitModel)
    {
        if (UnitInstanceIds.Contains(unitModel.InstanceId))
        {
            UnitInstanceIds.Remove(unitModel.InstanceId);
        }
    }

    public List<UnitModel> GetUnits()
    {
        return UnitInstanceIds
            .Select(id => UnitManager.Instance.GetPresenter(id)?.Model)
            .Where(unit => unit != null)
            .ToList();
    }

    public LocationData GetContainerData() => new LocationData
    {
        Type = LocationType.City,
        Name = cityName
    };


    #endregion
}

using System.Collections.Generic;
using UnityEngine;

public class CityPresenter
{
    private CityModel model;
    public CityModel Model => model;
    private ICityView view;


    public CityPresenter(CityModel model, ICityView view)
    {
        this.model = model;
        this.view = view;

        // Initialize the view with model data
        InitView();
    }

    private void InitView()
    {
        view.SetCityName(model.cityName);
        view.SetPosition(model.position);
        view.SetSeatsByCount(model.seatCount);
        view.UpdateSeatOccupancy(model.seats);
    }

    public void AddSeatToParty(Party party, int count = 1)
    {
        int available = model.seatCount - model.currentSeats;
        int toAdd = Mathf.Min(count, available);

        // 1. 가능한 만큼 먼저 추가
        for (int i = 0; i < toAdd; i++)
            model.AddSeat(party);

        int remaining = count - toAdd;
        if (remaining > 0)
        {
            // 2. 플레이어에게 어떤 당에서 뺄지 선택 요청
            var removableParties = new List<Party>();
            foreach (var kv in model.seats)
            {
                if (kv.Key != party && kv.Value > 0)
                    removableParties.Add(kv.Key);
            }

            if (removableParties.Count > 0)
            {
                // 선택 UI를 띄우고, 선택 결과를 콜백으로 받음
                view.RequestSeatRemovalChoice(removableParties, remaining, (chosenParties) =>
                {
                    // chosenParties: 플레이어가 선택한 당 리스트 (길이 == remaining)
                    foreach (var toRemove in chosenParties)
                    {
                        model.RemoveSeat(toRemove);
                        model.AddSeat(party);
                    }
                    view.UpdateSeatOccupancy(model.seats);
                });
                return; // 콜백에서 UpdateSeatOccupancy 호출
            }
            else
            {
                // 뺄 수 있는 의석이 없음
                Debug.LogWarning("No seats available to remove from other parties.");
            }
        }

        view.UpdateSeatOccupancy(model.seats);
    }

    public void RemoveSeatFromParty(Party party, int count = 1)
    {
        count = Mathf.Min(count, model.currentSeats);
        for (int i = 0; i < count; i++)
            model.RemoveSeat(party);

        view.UpdateSeatOccupancy(model.seats);
    }


    public void HandleAddCitySeatRequested(string targetCityName, Party party, int count)
    {
        // 이 이벤트가 나(이 도시)에게 온 것이 맞는지 확인
        if (targetCityName != model.cityName)
        {
            return; // 내 도시가 아니면 무시
        }

        Debug.Log($"Handling AddCitySeatRequested for {targetCityName}, Party: {party.partyName}, Count: {count}");
        // 이제 내 도시가 맞으니, 실제 의석 추가 로직 실행
        AddSeatToParty(party, count);
    }
    
    public void HandleRemoveCitySeatRequested(string targetCityName, Party party, int count)
    {
        // 이 이벤트가 나(이 도시)에게 온 것이 맞는지 확인
        if (targetCityName != model.cityName)
        {
            return; // 내 도시가 아니면 무시
        }

        Debug.Log($"Handling RemoveCitySeatRequested for {targetCityName}, Party: {party.partyName}, Count: {count}");
        // 이제 내 도시가 맞으니, 실제 의석 제거 로직 실행
        RemoveSeatFromParty(party, count);
    }
}

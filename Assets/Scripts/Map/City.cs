using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using UnityEngine;

public class City : IUnitContainer
{
    public CityModel model { get; }
    public ICityView view;


    public City(CityModel model, ICityView view)
    {
        this.model = model;
        this.view = view;
        InitView();
    }

    private void InitView()
    {
        view.SetCityName(model.cityName);
        view.SetPosition(model.position);
        view.SetSeatsByCount(model.seatMaxCount);
        view.UpdateSeatOccupancy(model.seats);
    }

    #region Seat Management

    public void AddSeatToParty(Party party, int count = 1)
    {
        // 동기적 처리: 한 번에 하나의 선택만 진행되도록 재귀/체이닝
        ProcessAddSeatSequentially(party, count); // Commented out as the method is removed.
    }

    public void RemoveSeatFromParty(Party party, int count = 1)
    {
        count = Mathf.Min(count, model.currentSeats);
        for (int i = 0; i < count; i++)
            model.RemoveSeat(party);

        view.UpdateSeatOccupancy(model.seats);
    }

    /// <summary>
    /// 좌석 추가를 순차적으로 처리하여, 동시 다중 선택 프롬프트가 발생하지 않도록 함.
    /// 또한 제거만 발생한 경우(실제 빈 자리가 생기지 않은 경우)에는 추가를 시도하지 않도록 가드.
    /// </summary>
    private void ProcessAddSeatSequentially(Party targetParty, int remaining)
    {
        if (remaining <= 0)
            return;

        // 자리 여유가 있으면 바로 추가
        if (model.currentSeats < model.seatMaxCount)
        {
            model.AddSeat(targetParty);
            view.UpdateSeatOccupancy(model.seats);
            ProcessAddSeatSequentially(targetParty, remaining - 1);
            return;
        }

        // 가득 찼으면 제거 후보 수집(본인 정당 제외, 1석 이상 보유)
        var removableParties = new List<Party>();
        foreach (var kv in model.seats)
        {
            if (kv.Key != targetParty && kv.Value > 0)
                removableParties.Add(kv.Key);
        }

        if (removableParties.Count == 0)
        {
            Debug.LogWarning("No seats available to remove from other parties.");
            return;
        }

        // 사용자에게 제거 대상을 한 번만 묻고, 완료되면 다음 스텝으로 진행
        GameManager.Instance.RequestPartySelection(removableParties, 1, (chosenParties) =>
        {
            // 현재가 꽉 찬 상태이므로, 이번 스텝에서는 '제거만' 수행
            foreach (var toRemove in chosenParties)
            {
                model.RemoveSeat(toRemove);
            }

            view.UpdateSeatOccupancy(model.seats);
            ProcessAddSeatSequentially(targetParty, remaining - 1);
        });
    }

    #endregion

    #region Unit Management

    public Dictionary<Unit, int> ContainedUnits { get; private set; } = new Dictionary<Unit, int>();

    public void AddUnit(Unit unit)
    {
        if (ContainedUnits.ContainsKey(unit))
            ContainedUnits[unit]++;
        else
            ContainedUnits[unit] = 1;
    }

    public void RemoveUnit(Unit unit)
    {
        if (ContainedUnits.ContainsKey(unit))
        {
            ContainedUnits[unit]--;
            if (ContainedUnits[unit] <= 0)
                ContainedUnits.Remove(unit);
        }
        else
        {
            Debug.LogWarning($"Attempted to remove unit '{unit.Model.uniqueId}' which is not contained in city '{model.cityName}'.");
        }
    }

    #endregion

    #region Highlight
    public void ShowAsCandidate(bool isCandidate)
    {
        (view as CityView)?.ShowAsCandidate(isCandidate);
    }
    #endregion
}

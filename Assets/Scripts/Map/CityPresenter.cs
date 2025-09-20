using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using UnityEngine;

public class CityPresenter : IUnitContainer
{
    public CityModel model { get; }
    private ICityView view;

    public CityPresenter(CityModel model, ICityView view)
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
        ProcessAddSeatSequentially(party, count);
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
        view.RequestSeatRemovalChoice(removableParties, 1, (chosenParties) =>
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

    public IReadOnlyList<UnitPresenter> ContainedUnits
    {
        get
        {
            // Model에 있는 UnitModel 목록을 UnitPresenter 목록으로 변환해서 반환합니다.
            // 이를 위해서는 UnitManager의 도움이 필요합니다.
            return model.UnitContained
                        .Select(unitModel => UnitManager.Instance.GetPresenterForModel(unitModel))
                        .ToList()
                        .AsReadOnly();
        }
    }
    public void AddUnit(UnitPresenter unit)
    {
        model.AddUnit(unit.Model);

        Debug.Log($"{unit.Model.Data.unitName} added to {model.cityName}.");
    }

    public void RemoveUnit(UnitPresenter unit)
    {
        model.RemoveUnit(unit.Model);

        Debug.Log($"{unit.Model.Data.unitName} removed from {model.cityName}.");
    }

    #endregion
}

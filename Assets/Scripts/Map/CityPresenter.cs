using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using UnityEngine;
using UnityEngine.EventSystems;

public class CityPresenter
{
    public CityModel Model { get; private set; }
    private readonly CityView View;


    public CityPresenter(CityModel model, CityView view)
    {
        this.Model = model;
        this.View = view;
        Initialize();
    }

    public void Initialize()
    {
        View.SetCityName(Model.cityName);
        View.SetPosition(Model.position);
        View.SetSeatsByCount(Model.seatMaxCount);
    }

    #region 객체 배치 및 제거
    /// <summary>
    /// 이 도시에 유닛을 배치합니다.
    /// </summary>
    public void AddUnit(UnitPresenter unitPresenter)
    {
        if (!Model.UnitInstanceIds.Contains(unitPresenter.Model.InstanceId))
        {
            Model.UnitInstanceIds.Add(unitPresenter.Model.InstanceId);
            View.UpdateSummaryView(Model);
        }
    }

    /// <summary>
    /// 이 도시에서 유닛을 제거합니다.
    /// </summary>
    public void RemoveUnit(UnitPresenter unitPresenter)
    {
        if (Model.UnitInstanceIds.Contains(unitPresenter.Model.InstanceId))
        {
            Model.UnitInstanceIds.Remove(unitPresenter.Model.InstanceId);
            View.UpdateSummaryView(Model);
        }
    }

    /// <summary>
    /// 이 도시에 위협 마커를 배치합니다. (룰북 p.24 참고)
    /// </summary>
    public void AddThreatMarker(ThreatMarkerPresenter markerPresenter)
    {
        if (!Model.ThreatMarkerInstanceIds.Contains(markerPresenter.Model.InstanceId))
        {
            Model.ThreatMarkerInstanceIds.Add(markerPresenter.Model.InstanceId);
            View.UpdateSummaryView(Model);
        }
    }

    public void AddPartyBase(FactionType party, int count = 1)
    {
        // 동기적 처리: 한 번에 하나의 선택만 진행되도록 재귀/체이닝
        ProcessAddSeatSequentially(party, count); // Commented out as the method is removed.
    }

    public void RemoveSeatFromParty(FactionType party, int count = 1)
    {
        count = Mathf.Min(count, Model.currentSeats);
        for (int i = 0; i < count; i++)
            Model.RemoveSeat(party);

        View.UpdateSeatOccupancy(Model.PartyBasesCounts);
    }

    /// <summary>
    /// 좌석 추가를 순차적으로 처리하여, 동시 다중 선택 프롬프트가 발생하지 않도록 함.
    /// 또한 제거만 발생한 경우(실제 빈 자리가 생기지 않은 경우)에는 추가를 시도하지 않도록 가드.
    /// </summary>
    private void ProcessAddSeatSequentially(FactionType targetParty, int remaining)
    {
        if (remaining <= 0)
            return;

        // 자리 여유가 있으면 바로 추가
        if (Model.currentSeats < Model.seatMaxCount)
        {
            Model.AddSeat(targetParty);
            View.UpdateSeatOccupancy(Model.PartyBasesCounts);
            ProcessAddSeatSequentially(targetParty, remaining - 1);
            return;
        }

        // 가득 찼으면 제거 후보 수집(본인 정당 제외, 1석 이상 보유)
        var removableParties = new List<FactionType>();
        foreach (var kv in Model.PartyBasesCounts)
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
                Model.RemoveSeat(toRemove);
            }

            View.UpdateSeatOccupancy(Model.PartyBasesCounts);
            ProcessAddSeatSequentially(targetParty, remaining - 1);
        });
    }

    #endregion


    #region Highlight
    public void ShowAsCandidate(bool isCandidate)
    {
        (View as CityView)?.ShowAsCandidate(isCandidate);
    }

    #endregion

    #region Tooltip
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.ShowQuickView(Model, Input.mousePosition);
    }

    // 마우스가 UI 영역에서 나갔을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HideQuickView();
    }
    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Event.UI;
using IngameDebugConsole;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CityPresenter: IUnitContainer
{
    public CityModel Model { get; private set; }
    private readonly CityView View;

    private Coroutine showQuickViewCoroutine;
    public float showDelay = 0.3f;

    public Guid requestId;
    public bool isCandidate = false;

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
        View.UpdateSummaryView(Model);
    }

    #region 객체 배치 및 제거
    /// <summary>
    /// 이 도시에 유닛을 배치합니다.
    /// </summary>
    public void AddUnit(UnitModel unitModel)
    {
        if (!Model.UnitInstanceIds.Contains(unitModel.InstanceId))
        {
            Model.UnitInstanceIds.Add(unitModel.InstanceId);
            View.UpdateSummaryView(Model);
        }
    }

    /// <summary>
    /// 이 도시에서 유닛을 제거합니다.
    /// </summary>
    public void RemoveUnit(UnitModel unitModel)
    {
        if (Model.UnitInstanceIds.Contains(unitModel.InstanceId))
        {
            Model.UnitInstanceIds.Remove(unitModel.InstanceId);
            View.UpdateSummaryView(Model);
        }
    }

    public List<UnitModel> GetUnits()
    {
        return Model.UnitInstanceIds
            .Select(id => UnitManager.Instance.GetModel(id))
            .Where(unit => unit != null)
            .ToList();
    }

    public string GetContainerName() => Model.cityName;

    /// <summary>
    /// 이 도시에 위협 마커를 배치합니다. (룰북 p.24 참고)
    /// </summary>
    public void AddThreatMarker(ThreatMarkerPresenter markerPresenter)
    {
        if (!Model.ThreatMarkerInstanceIds.Contains(markerPresenter.Model.InstanceId))
        {
            Model.ThreatMarkerInstanceIds.Add(markerPresenter.Model.InstanceId);
            View.UpdateSummaryView(Model);
            View.AddThreatToCity(markerPresenter.View.transform, markerPresenter.Model);
        }
    }

    /// <summary>
    /// 이 도시에서 위협 마커를 제거합니다.
    /// </summary>
    public void RemoveThreatMarker(ThreatMarkerPresenter markerPresenter)
    {
        if (Model.ThreatMarkerInstanceIds.Contains(markerPresenter.Model.InstanceId))
        {
            Model.ThreatMarkerInstanceIds.Remove(markerPresenter.Model.InstanceId);
            View.UpdateSummaryView(Model);
            View.RemoveThreatFromCity(markerPresenter.Model);
        }
    }

    private List<FactionType> _chosenPartiesForRemoval;
    public IEnumerator AddPartyBase(FactionType party, int count = 1)
    {
        Debug.Log($"도시 '{Model.cityName}'에 '{party}' 정당 기반 추가 시도 (현재 좌석: {Model.currentSeats}/{Model.seatMaxCount})");
        for (int i = 0; i < count; i++)
        {
            // 1. 자리가 비어있는 경우: 즉시 좌석 추가
            if (Model.currentSeats < Model.seatMaxCount)
            {
                Model.AddSeat(party);
                View.UpdateSeatOccupancy(Model.PartyBasesCounts);
                // 한 프레임 대기하여 UI 업데이트 등을 확인 (선택사항)
                yield return null;
                continue; // 다음 루프 실행
            }

            // 2. 자리가 꽉 찬 경우: 다른 정당 좌석 제거 필요
            var removableParties = new List<FactionType>();
            foreach (var kv in Model.PartyBasesCounts)
            {
                if (kv.Key != party && kv.Value > 0)
                {
                    removableParties.Add(kv.Key);
                }
            }

            // 제거할 정당이 없으면 더 이상 진행 불가
            if (removableParties.Count == 0)
            {
                Debug.LogWarning("제거할 다른 정당의 좌석이 없습니다.");
                yield break; // 코루틴 종료
            }

            // 3. 플레이어에게 제거할 정당 선택 요청
            _chosenPartiesForRemoval = null; // 대기 전 변수 초기화

            // 선택 완료 이벤트 구독
            EventBus.Subscribe<SelectionMadeEvent<FactionType>>(OnPartyForRemovalSelected);

            // EventBus를 통해 선택 요청 이벤트 발행
            EventBus.Publish(new RequestSelectionEvent<FactionType>(
                PlayerSelectionType.CardEffect_ChoosePartyBase,
                removableParties
            // 필요하다면 선택 개수(1개) 등의 정보도 이벤트에 포함
            ));

            // 선택이 완료될 때(_chosenPartiesForRemoval이 채워질 때)까지 대기
            yield return new WaitUntil(() => _chosenPartiesForRemoval != null);

            // 구독 해제
            EventBus.Unsubscribe<SelectionMadeEvent<FactionType>>(OnPartyForRemovalSelected);

            // 4. 선택된 정당의 좌석 제거
            foreach (var toRemove in _chosenPartiesForRemoval)
            {
                Model.RemoveSeat(toRemove);
            }

            // 5. 빈 자리가 생겼으므로 목표 정당의 좌석 추가
            Model.AddSeat(party);
            View.UpdateSeatOccupancy(Model.PartyBasesCounts);
        }
    }

    // 선택 완료 이벤트 콜백 메서드
    private void OnPartyForRemovalSelected(SelectionMadeEvent<FactionType> e)
    {
        _chosenPartiesForRemoval = new List<FactionType> { e.SelectedItem };
    }

    public void RemoveSeatFromParty(FactionType party, int count = 1)
    {
        count = Mathf.Min(count, Model.currentSeats);
        for (int i = 0; i < count; i++)
            Model.RemoveSeat(party);

        View.UpdateSeatOccupancy(Model.PartyBasesCounts);
    }

    #endregion


    #region Highlight
    public void ShowAsCandidate(bool isCandidate, Guid requestId)
    {
        this.isCandidate = isCandidate;
        this.requestId = requestId;
        View?.ShowAsCandidate(isCandidate);
    }
    
    public void OnPointerClick()
    {
        if (isCandidate)
        {
            // 선택 이벤트 발행 (RequestId 등 필요시 포함)
            EventBus.Publish(new SelectionMadeEvent<CityModel>(Model, requestId));
        }
    }

    #endregion

    #region Tooltip
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (showQuickViewCoroutine != null) View.StopCoroutine(showQuickViewCoroutine);
        showQuickViewCoroutine = View.StartCoroutine(ShowQuickViewRoutine());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (showQuickViewCoroutine != null) View.StopCoroutine(showQuickViewCoroutine);
        UIManager.Instance.HideQuickView();
    }

    private IEnumerator ShowQuickViewRoutine()
    {
        yield return new WaitForSeconds(showDelay);
        UIManager.Instance.ShowQuickView(Model, View.transform.position); // 도시 위치를 기준으로 퀵뷰 표시
    }
    #endregion
}

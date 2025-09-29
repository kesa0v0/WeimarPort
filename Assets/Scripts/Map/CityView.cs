
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class CityView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cityName;
    [SerializeField] private GameObject cityIndicator;
    [SerializeField] private GameObject baseGameObject;
    [SerializeField] private Transform baseParent;

    [Header("Summary View Elements")]
    public PartySummaryUI kpdSummary; // 각 정당별 요약 UI를 연결할 변수
    public PartySummaryUI spdSummary;
    public PartySummaryUI dnvpSummary;
    public PartySummaryUI governmentSummary;
    private readonly List<PartyBaseView> partyBases = new();

    public Transform UnrestThreatParent; // Unrest/Threat 마커가 배치될 부모 Transform
    public Transform PovertyThreatParent; // Poverty/Threat 마커가 배치될 부모 Transform
    public Transform KPDThreatParent; // KPD Threat 마커가 배치될 부모 Transform
    public Transform DNVPThreatParent; // DNVP Threat 마커가 배치될 부모 Transform

    private Dictionary<FactionType, PartySummaryUI> summaryUIs;


    void Awake()
    {
        summaryUIs = new Dictionary<FactionType, PartySummaryUI>()
        {
            { FactionType.SPD, spdSummary },
            { FactionType.KPD, kpdSummary },
            { FactionType.DNVP, dnvpSummary },
            { FactionType.Government, governmentSummary }
        };
    }

    public void SetCityName(string cityName)
    {
        this.name = cityName;
        this.cityName.text = cityName;
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = new Vector3(position.x, 0, position.y);
    }

    public void SetSeatsByCount(int seatCount)
    {
        // If exists, destroy old seats
        foreach (PartyBaseView child in partyBases)
        {
            Destroy(child.gameObject);
        }
        partyBases.Clear();

        // Create new seats
        for (int i = 0; i < seatCount; i++)
        {
            PartyBaseView seat = Instantiate(baseGameObject, baseParent).GetComponent<PartyBaseView>();
            seat.transform.localPosition = new Vector2((i - (seatCount - 1) / 2.0f) * 0.5f, 0);
            partyBases.Add(seat);
        }

        // 좌석 개수에 따라 인디케이터 크기 자동 조절 (예: 최소 1, seatCount 1당 0.3씩 증가)
        float indicatorWidth = 2f + seatCount * 0.3f;
        SetCityIndicatorSize(indicatorWidth);
    }

    public void SetCityIndicatorSize(float size)
    {
        var currentScale = cityIndicator.transform.localScale;
        cityIndicator.transform.localScale = new Vector3(size, currentScale.y, currentScale.z);
    }

    public void UpdateSeatOccupancy(Dictionary<FactionType, int> occupiedBy)
    {
        int seatIndex = 0;
        foreach (var party in GameManager.Instance.gameState.Parties)
        {
            int count = occupiedBy.TryGetValue(party.Data.factionType, out int c) ? c : 0;
            for (int i = 0; i < count; i++)
            {
                if (seatIndex < partyBases.Count)
                {
                    partyBases[seatIndex].SetColor(party.Data.factionColor);
                    seatIndex++;
                }
                else
                {
                    Debug.LogError("Not enough seats to display all parties.");
                    return;
                }
            }
        }

        // Clear remaining seats
        for (int i = seatIndex; i < partyBases.Count; i++)
        {
            partyBases[i].SetColor(Color.gray); // Default color for unoccupied seats
        }
    }


    #region Object Placement

    public void AddThreatToCity(Transform threatTransform, ThreatMarkerModel threatModel)
    {
        // GameObject 활성화
        threatTransform.gameObject.SetActive(true);

        switch (threatModel.Data.Category)
        {
            case ThreatMarkerData.MarkerCategory.TwoSidedPoverty:
                threatTransform.SetParent(UnrestThreatParent, true);
                break;
            case ThreatMarkerData.MarkerCategory.OneSidedThreat:
                threatTransform.SetParent(PovertyThreatParent, true);
                break;
            case ThreatMarkerData.MarkerCategory.PartySpecificThreat:
                if (threatModel.Data.AssociatedParty.factionType == FactionType.KPD)
                    threatTransform.SetParent(KPDThreatParent, true);
                else if (threatModel.Data.AssociatedParty.factionType == FactionType.DNVP)
                    threatTransform.SetParent(DNVPThreatParent, true);
                else
                    Debug.LogWarning($"Unknown TargetParty for PartySpecificThreat: {threatModel.Data.AssociatedParty}, placing under City root.");
                break;
            default:
                Debug.LogWarning($"Unknown ThreatType: {threatModel.Data.Category}, placing under City root.");
                threatTransform.SetParent(transform, true);
                break;
        }
        threatTransform.localPosition = Vector3.zero; // 부모 기준 위치 초기화
    }

    /// <summary>
    /// 도시에서 객체를 제거하고 부모를 null로 설정합니다.
    /// </summary>
    public void RemoveThreatFromCity(ThreatMarkerModel threatModel)
    {
        var presenter = ThreatManager.Instance.GetPresenter(threatModel.InstanceId);
        if (presenter != null)
        {
            presenter.View.transform.SetParent(null);
            presenter.View.gameObject.SetActive(false); // 비활성화
        }
        else
        {
            Debug.LogWarning($"ThreatMarker with ID {threatModel.InstanceId} not found in CityView.");
        }
    }

    #endregion

    public void ShowAsCandidate(bool isCandidate)
    {
        // 간단 구현: 인디케이터 활성/비활성 토글 또는 색 변경 가능
        if (cityIndicator != null)
            cityIndicator.SetActive(true); // ensure visible
        // 추가로 머터리얼 색을 바꾸는 등의 연출을 여기에 추가 가능
    }
    

    // CityPresenter가 이 함수를 호출하여 전체 UI를 갱신
    public void UpdateSummaryView(CityModel model)
    {
        // 모든 요약 UI를 일단 비활성화
        foreach (var ui in summaryUIs.Values)
        {
            ui.gameObject.SetActive(false);
        }

        // 각 정당별 유닛/기반 수 집계
        var partyUnitCounts = new Dictionary<FactionType, int>();
        var partyHasStrongUnit = new Dictionary<FactionType, bool>();

        // 초기화
        foreach (var faction in summaryUIs.Keys)
        {
            partyUnitCounts[faction] = 0;
            partyHasStrongUnit[faction] = false;
        }

        // 유닛 집계
        foreach (var unit in model.GetUnitsInCity())
        {
            var faction = unit.ControllerPartyId;
            if (partyUnitCounts.ContainsKey(faction))
            {
                partyUnitCounts[faction]++;
                if (unit.Data.Strength == 2) // 예시: 강한 유닛 여부
                    partyHasStrongUnit[faction] = true;
            }
        }

        // UI 업데이트
        foreach (var faction in summaryUIs.Keys)
        {
            summaryUIs[faction].gameObject.SetActive(true);
            summaryUIs[faction].UpdateView(partyUnitCounts[faction], partyHasStrongUnit[faction]);
        }
    }

    /*
    // 간단한 클릭 전달: collider + raycast 또는 EventSystem 환경에서 동작
    private void OnMouseDown()
    {
        var presenter = CityManager.Instance.GetCity(this.name);
        if (presenter != null)
        {
            GameManager.Instance?.OnCityClicked(presenter);
        }
    }
    */
}
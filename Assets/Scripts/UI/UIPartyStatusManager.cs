using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIPartyStatusManager : MonoBehaviour
{
    [SerializeField] private GameObject partyStatusPrefab;
    [SerializeField] private Transform partyStatusParent; // UI에서 배치할 부모 오브젝트

    private Dictionary<Party, UIPartyStatusView> partyViews = new();
    private Dictionary<Party, Party> partyModels = new();

    // 현재 진행 중인 선택 세션을 안정적으로 관리하기 위한 상태
    private Dictionary<Party, System.Action> _activeOriginalOnClicked = new();
    private bool _selectionActive = false;


    public void Initialize(List<Party> parties)
    {
        foreach (var party in parties)
        {
            var viewObj = Instantiate(partyStatusPrefab, partyStatusParent);
            var view = viewObj.GetComponent<UIPartyStatusView>();
            partyViews.Add(party, view);
            partyModels.Add(party, party);

            UpdatePartyStatusView(party);
        }

        UpdatePartyOrderUI(GameManager.Instance.GetCurrentRoundPartyOrder());
    }

    public void UpdatePartyStatusView(Party party)
    {
        if (partyViews.TryGetValue(party, out var view))
        {
            view.SetPartyName(party.Data.factionName, party.Data.factionColor);
            view.SetPartyStatus($"Opposition");
            view.SetPartyAgenda(party.currentPartyAgenda, party.Data.factionColor);

            // 보유한 하위 정당 목록 표시
            var subPartyNames = party.ControlledMinorParties.Count > 0 ? string.Join(", ", party.ControlledMinorParties.ConvertAll(sp => sp.partyName)) : "None";
            view.SetPartySubParties(subPartyNames);

            // 보유 군대 표시
            view.SetInSupplyUnits(party.ContainedUnits);
        }
    }

    public void UpdatePartyOrderUI(List<Party> newOrder)
    {
        for (int i = 0; i < newOrder.Count; i++)
        {
            if (partyViews.TryGetValue(newOrder[i], out var view))
            {
                view.transform.SetSiblingIndex(i);
            }
        }
    }

    public void RequestPartySelection(List<Party> candidates, int count, System.Action<List<Party>> onChosen)
    {
        // 이전에 진행 중이던 선택 세션이 있으면 안전하게 정리
        CancelActiveSelection();

        // candidates 전체 하이라이트
        foreach (var kvp in partyViews)
        {
            kvp.Value.SetHighlight(candidates.Contains(kvp.Key));
        }

        _activeOriginalOnClicked.Clear();
        _selectionActive = true;
        bool selected = false;

        foreach (var party in candidates)
        {
            if (partyViews.TryGetValue(party, out var view))
            {
                _activeOriginalOnClicked[party] = view.OnClicked;
                view.OnClicked = () =>
                {
                    if (selected) return;
                    selected = true;

                    // 모든 하이라이트 해제 및 이벤트 복구를 먼저 수행(새 세션 하이라이트가 바로 적용되도록)
                    CancelActiveSelection();

                    // 콜백 호출 (한 파티만 리스트로)
                    onChosen?.Invoke(new List<Party> { party });
                };
            }
        }
    }

    // 현재 선택 세션을 종료하며, 하이라이트/클릭 이벤트를 원복
    private void CancelActiveSelection()
    {
        if (!_selectionActive)
        {
            // 그래도 하이라이트는 항상 초기화 보장
            foreach (var kv in partyViews)
                kv.Value.SetHighlight(false);
            return;
        }

        // 하이라이트 초기화
        foreach (var kv in partyViews)
            kv.Value.SetHighlight(false);

        // 이벤트 원상복구
        foreach (var kv in _activeOriginalOnClicked)
        {
            if (partyViews.TryGetValue(kv.Key, out var v))
                v.OnClicked = kv.Value;
        }

        _activeOriginalOnClicked.Clear();
        _selectionActive = false;
    }
}
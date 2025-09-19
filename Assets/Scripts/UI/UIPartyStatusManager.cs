using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIPartyStatusManager : MonoBehaviour
{
    public static UIPartyStatusManager instance;
    [SerializeField] private GameObject partyStatusPrefab;
    [SerializeField] private Transform partyStatusParent; // UI에서 배치할 부모 오브젝트

    private Dictionary<Party, UIPartyStatusView> partyViews = new();
    private Dictionary<Party, MainParty> partyModels = new();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void Initialize(List<MainParty> parties)
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

    public void UpdatePartyStatusView(MainParty party)
    {
        if (partyViews.TryGetValue(party, out var view))
        {
            view.SetPartyName(party.partyName);
            view.SetPartyStatus($"Opposition");
            view.SetPartyAgenda(party.currentPartyAgenda);

            // 보유한 하위 정당 목록 표시
            var subPartyNames = party.heldSubParties.Count > 0 ? string.Join(", ", party.heldSubParties.ConvertAll(sp => sp.partyName)) : "None";
            view.SetPartySubParties(subPartyNames);

            // 보유 군대 표시
            view.SetInSupplyUnits(party.preservedPartyUnits);
        }
    }

    public void UpdatePartyOrderUI(List<MainParty> newOrder)
    {
        for (int i = 0; i < newOrder.Count; i++)
        {
            Debug.Log($"Updating UI order: {newOrder[i].partyName} to index {i}");
            if (partyViews.TryGetValue(newOrder[i], out var view))
            {
                view.transform.SetSiblingIndex(i);
            }
        }
    }

    public void RequestPartySelection(List<Party> candidates, int count, System.Action<List<Party>> onChosen)
    {
        // candidates 전체 하이라이트
        foreach (var kvp in partyViews)
        {
            kvp.Value.SetHighlight(candidates.Contains(kvp.Key));
        }

        Dictionary<Party, System.Action> originalOnClicked = new();
        bool selected = false;

        foreach (var party in candidates)
        {
            if (partyViews.TryGetValue(party, out var view))
            {
                originalOnClicked[party] = view.OnClicked;
                view.OnClicked = () =>
                {
                    if (selected) return;
                    selected = true;
                    // 모든 하이라이트 해제
                    foreach (var kv in partyViews)
                    {
                        kv.Value.SetHighlight(false);
                    }
                    // 콜백 호출 (한 파티만 리스트로)
                    onChosen?.Invoke(new List<Party> { party });
                    // 이벤트 원상복구
                    foreach (var p in candidates)
                    {
                        if (partyViews.TryGetValue(p, out var v) && originalOnClicked.ContainsKey(p))
                            v.OnClicked = originalOnClicked[p];
                    }
                };
            }
        }
    }
}
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

            var capturedParty = party;
            view.OnClicked = () => OnPartyStatusClicked(capturedParty);
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

    private void OnPartyStatusClicked(Party party)
    {
        // 파티 상태창 클릭 시 처리 (예: 선택, 상세 정보, 등)
        Debug.Log($"{party.partyName} 상태창 클릭됨");
    }

    // 필요시 위치 이동 등도 여기서 제어
    public void SetPartyStatusPosition(Party party, Vector2 anchoredPos)
    {
        if (partyViews.TryGetValue(party, out var view))
        {
            (view.transform as RectTransform).anchoredPosition = anchoredPos;
        }
    }
}
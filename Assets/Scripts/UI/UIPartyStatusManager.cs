using System.Collections.Generic;
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
    }

    public void UpdatePartyStatusView(Party party)
    {
        if (partyViews.TryGetValue(party, out var view) && partyModels.TryGetValue(party, out var model))
        {
            view.SetPartyName(model.partyName);
            if (model is MainParty mainModel)
            {
                view.SetPartyStatus(mainModel.partyGovernmentStatus);
                view.SetPartyAgenda(mainModel.currentPartyAgenda);
                view.SetPreservedUnits(mainModel.preservedPartyUnits);
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
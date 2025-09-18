using System.Collections.Generic;
using UnityEngine;

public class UIPartyStatusManager : MonoBehaviour
{
    [SerializeField] private GameObject partyStatusPrefab;
    [SerializeField] private Transform partyStatusParent; // UI에서 배치할 부모 오브젝트

    private Dictionary<Party, UIPartyStatusPresenter> presenters = new();

    public void Initialize(List<Party> parties)
    {
        foreach (var party in parties)
        {
            var viewObj = Instantiate(partyStatusPrefab, partyStatusParent);
            var view = viewObj.GetComponent<UIPartyStatusView>();
            var model = new UIPartyStatusModel(party.partyName);
            var presenter = new UIPartyStatusPresenter(model, view);

            presenters.Add(party, presenter);

            // 클릭 이벤트 연결
            var button = viewObj.GetComponent<UnityEngine.UI.Button>();
            if (button != null)
            {
                var capturedParty = party;
                button.onClick.AddListener(() => OnPartyStatusClicked(capturedParty));
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
        if (presenters.TryGetValue(party, out var presenter))
        {
            var view = presenter.view;
            (view.transform as RectTransform).anchoredPosition = anchoredPos;
        }
    }
}
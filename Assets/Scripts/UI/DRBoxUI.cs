using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// DR Box의 상태를 UI에 표시하고 툴팁을 관리합니다.
/// </summary>
public class DRBoxUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Tooltip tooltip;

    private List<string> currentMarkersInDRBox = new List<string>();

    void OnEnable()
    {
        // ThreatManager의 이벤트에 구독
        ThreatManager.OnDRBoxChanged += UpdateDRBoxDisplay;
    }

    void OnDisable()
    {
        // ThreatManager의 이벤트 구독 해제
        ThreatManager.OnDRBoxChanged -= UpdateDRBoxDisplay;
    }

    private void UpdateDRBoxDisplay(List<string> markerInstanceIds)
    {
        currentMarkersInDRBox = markerInstanceIds;
        int count = currentMarkersInDRBox.Count;

        // 룰북 p.12: 7번째 마커가 놓이면 게임 오버
        countText.text = $"{count} / 7";
        countText.color = count >= 6 ? Color.red : Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null && currentMarkersInDRBox.Any())
        {
            // 툴팁에 표시할 텍스트를 생성합니다.
            string tooltipContent = "<b>DR Box Contents:</b>\n";
            
            var markerGroups = currentMarkersInDRBox
                .Select(id => ThreatManager.Instance.GetPresenter(id).Model.Data.ThreatName)
                .GroupBy(name => name)
                .Select(group => $"{group.Key} x{group.Count()}");

            tooltipContent += string.Join("\n", markerGroups);
            
            tooltip.Show(tooltipContent);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            tooltip.Hide();
        }
    }
}

/// <summary>
/// 간단하고 재사용 가능한 툴팁 UI 컴포넌트입니다.
/// </summary>
public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI contentText;
    private RectTransform backgroundRectTransform;

    void Awake()
    {
        backgroundRectTransform = GetComponent<RectTransform>();
        Hide();
    }
    
    void Update()
    {
        // 마우스 커서를 따라 툴팁이 움직이도록 설정
        transform.position = Input.mousePosition;
    }

    public void Show(string content)
    {
        gameObject.SetActive(true);
        contentText.text = content;
        // 텍스트 크기에 맞게 배경 크기 조절 (Content Size Fitter 사용 권장)
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

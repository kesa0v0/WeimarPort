using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public UIPartyStatusManager partyStatusManager;
    public PlayerHandPanel playerHandPanel;
    public DisposedPanel disposedPanel;
    public GovernmentPanel governmentPanel;


    [Header("Tooltip Panels")]
    public GameObject quickViewPanel; // Inspector에서 Quick-View 패널 연결
    public GameObject unitIconPrefab; // 유닛 아이콘 프리팹

    private QuickViewPanel _currentQuickView; // 현재 씬에 생성된 퀵뷰 인스턴스를 저장할 변수


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }



    public void ShowQuickView(CityModel cityModel, Vector3 worldPosition)
    {
        // 퀵뷰가 없다면 프리팹으로부터 새로 생성
        if (_currentQuickView == null)
        {
            GameObject instance = Instantiate(quickViewPanel, transform); // UIManager의 자식으로 생성
            _currentQuickView = instance.GetComponent<QuickViewPanel>();
        }

        // 도시 이름 설정
        _currentQuickView.cityNameText.text = cityModel.cityName;

        // 생성된 인스턴스의 스크립트를 통해 UnitContainer에 접근합니다.
        RectTransform container = _currentQuickView.unitContainer;

        // 1. 기존 유닛 아이콘들 삭제
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // 2. CityModel 데이터로 새로운 유닛 아이콘 생성 및 부모 지정
        foreach (UnitModel unit in cityModel.GetUnitsInCity())
        {
            GameObject iconObj = Instantiate(unitIconPrefab, container);
            // 강도 표시용 텍스트
            var txt = iconObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (txt != null)
            {
                txt.text = unit.Data != null ? unit.Data.Strength.ToString() : "?";
            }
            // 아이콘 이미지 및 소속 정당별 색상
            var img = iconObj.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                // 1. 아이콘 스프라이트
                if (unit.Data != null && unit.Data.Icon != null)
                {
                    img.sprite = unit.Data.Icon;
                }
                else
                {
                    Debug.LogWarning($"Unit Data or Icon is null for unit in city {cityModel.cityName}");
                }
            }
        }

        // 3. 패널 위치 설정 및 활성화 (월드좌표 → 캔버스 로컬좌표 변환)
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main, out localPoint))
            {
                _currentQuickView.GetComponent<RectTransform>().anchoredPosition = localPoint;
            }
        }
        else
        {
            // fallback: 그냥 월드좌표
            _currentQuickView.transform.position = worldPosition;
        }
        _currentQuickView.gameObject.SetActive(true);
    }

    public void HideQuickView()
    {
        if (_currentQuickView != null)
        {
            _currentQuickView.gameObject.SetActive(false);
        }
    }
}

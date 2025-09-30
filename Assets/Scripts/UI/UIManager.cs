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



    public void ShowQuickView(CityModel cityModel, Vector3 position)
    {
        // 퀵뷰가 없다면 프리팹으로부터 새로 생성
        if (_currentQuickView == null)
        {
            GameObject instance = Instantiate(quickViewPanel, transform); // UIManager의 자식으로 생성
            _currentQuickView = instance.GetComponent<QuickViewPanel>();
        }

        // --- 여기가 핵심 ---
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
            // ... 아이콘 데이터 설정 로직 ...
        }

        // 3. 패널 위치 설정 및 활성화
        _currentQuickView.transform.position = position;
        _currentQuickView.gameObject.SetActive(true);
    }

    public void HideQuickView()
    {
        quickViewPanel.SetActive(false);
    }
}

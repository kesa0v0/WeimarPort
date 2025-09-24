using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitIconView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject countBadge;

    // 전체 / 왼쪽 절반 / 오른쪽 절반 중 어떤 영역을 사용할지 선택
    public enum HorizontalPortion
    {
        Full,
        LeftHalf,
        RightHalf
    }

    [Header("Sprite Portion Settings")]
    [SerializeField] private HorizontalPortion horizontalPortion = HorizontalPortion.Full;

    private UnitData unitData;

    public void Setup(UnitData data, HorizontalPortion portion, int count)
    {
        unitData = data;
        horizontalPortion = portion;

        if (data.unitMaterial != null && data.unitMaterial.mainTexture != null)
        {
            // Material에서 메인 텍스처(Texture2D)를 가져옵니다.
            Texture2D unitTex = data.unitMaterial.mainTexture as Texture2D;
            if (unitTex != null)
            {
                // 사용할 부분(Rect) 계산
                Rect rect;
                switch (horizontalPortion)
                {
                    case HorizontalPortion.LeftHalf:
                        rect = new Rect(0, 0, unitTex.width * 0.5f, unitTex.height);
                        break;
                    case HorizontalPortion.RightHalf:
                        rect = new Rect(unitTex.width * 0.5f, 0, unitTex.width * 0.5f, unitTex.height);
                        break;
                    default:
                        rect = new Rect(0, 0, unitTex.width, unitTex.height);
                        break;
                }

                // pivot은 부분 Rect의 가운데로 (부분 Sprite 자체의 중앙)
                Vector2 pivot = new Vector2(0.5f, 0.5f);
                iconImage.sprite = Sprite.Create(unitTex, rect, pivot);
            }
        }
        
        if (count > 1)
        {
            countBadge.SetActive(true);
            countText.text = count.ToString();
        }
        else
        {
            countBadge.SetActive(false);
        }
    }
    
    // 이 아이콘을 클릭했을 때의 로직 (예: 유닛 배치 시작)
    public void OnClick()
    {
        Debug.Log($"Clicked on {unitData.unitName}");
        var local = GameManager.Instance?.gameState?.playerParty;
        if (local == null) return;

        GameManager.Instance.EnterCitySelectionMode(
            PlayerActionState.SelectingCityForUnitMove,
            (selectedCity) =>
            {
                UnitManager.Instance.TryMoveOneUnitFromHandTypeToCity(local, unitData, selectedCity);
            }
        );
    }
}
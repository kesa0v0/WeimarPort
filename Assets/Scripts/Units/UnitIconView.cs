using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitIconView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject countBadge;

    private UnitData unitData;

    public void Setup(UnitData data, int count)
    {
        unitData = data;

        if (data.unitMaterial != null && data.unitMaterial.mainTexture != null)
        {
            // 1. Material에서 메인 텍스처(Texture2D)를 가져옵니다.
            Texture2D unitTex = data.unitMaterial.mainTexture as Texture2D;

            if (unitTex != null)
            {
                // 2. 가져온 Texture2D로 실시간으로 Sprite를 생성합니다.
                Rect rect = new Rect(0, 0, unitTex.width, unitTex.height);
                Vector2 pivot = new Vector2(0.5f, 0.5f); // 중앙을 기준으로
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
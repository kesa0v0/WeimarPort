// Assets/Scripts/UI/PartySummaryUI.cs (수정된 최종 버전)
using UnityEngine;
using TMPro;

public class PartySummaryUI : MonoBehaviour
{
    // [Tooltip("강조 효과를 위해 Glow 설정이 된 TextMeshPro 머티리얼을 할당해야 합니다.")]
    public TextMeshProUGUI countText;

    private Material textMaterialInstance; // 머티리얼의 인스턴스를 저장할 변수
    private float initialGlowPower = 0f;

    void Awake()
    {
        // 원본 머티리얼이 다른 UI에 영향을 주지 않도록 인스턴스를 복제해서 사용합니다.
        textMaterialInstance = new Material(countText.fontMaterial);
        countText.fontMaterial = textMaterialInstance;

        // 초기 Glow Power 값을 저장해 둡니다.
        if (textMaterialInstance.HasProperty("_GlowPower"))
        {
            initialGlowPower = textMaterialInstance.GetFloat("_GlowPower");
        }
    }

    public void UpdateView(int count, bool hasStrongUnit)
    {
        gameObject.SetActive(count > 0);
        if (count > 0)
        {
            countText.text = count.ToString();
            SetHighlight(hasStrongUnit);
        }
    }

    /// <summary>
    /// 텍스트의 빛 효과(Glow)를 켜거나 끕니다.
    /// </summary>
    /// <param name="isHighlighted">빛 효과를 켤지 여부</param>
    private void SetHighlight(bool isHighlighted)
    {
        if (textMaterialInstance == null || !textMaterialInstance.HasProperty("_GlowPower"))
        {
            Debug.LogWarning("이 텍스트의 머티리얼에는 Glow 속성이 없습니다.");
            return;
        }

        // isHighlighted가 true이면 저장해둔 초기 Glow Power 값을, false이면 0을 사용
        float targetGlow = isHighlighted ? initialGlowPower : 0f;
        textMaterialInstance.SetFloat("_GlowPower", targetGlow);
    }
}
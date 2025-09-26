using UnityEngine;

public class PlayerHandPanel : MonoBehaviour
{
    [Header("프리팹과 부모")]
    [SerializeField] private GameObject unitIconPrefab;
    [SerializeField] private Transform unitContainer;  // 아이콘들이 생성될 부모 Transform (Horizontal Layout Group이 붙어있음)

    /// <summary>
    /// 핸드 모델의 데이터를 기반으로 UI 전체를 다시 그립니다.
    /// </summary>
    public void Redraw(IUnitContainer model)
    {
        // 1. 기존에 있던 아이콘들을 모두 삭제합니다.
        foreach (Transform child in unitContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. 모델의 데이터를 기반으로 새 아이콘들을 생성합니다.
        foreach (var entry in model.GetUnits())
        {
            UnitData data = entry.Data;

            GameObject iconObj = Instantiate(unitIconPrefab, unitContainer);
            UnitView iconView = iconObj.GetComponent<UnitView>();
            
        }
    }
}
using UnityEngine;

public class GovernmentPanel : MonoBehaviour
{
    [Header("Prefabs and Parents")]
    [SerializeField] private GameObject unitIconPrefab;
    [SerializeField] private Transform unitContainer;

    public void Redraw()
    {
        var gov = GameManager.Instance?.gameState?.government;
        if (gov == null) return;

        foreach (Transform child in unitContainer)
            Destroy(child.gameObject);

        foreach (var unit in gov.GetUnits())
        {
            var data = unit.Data;
            var iconObj = Instantiate(unitIconPrefab, unitContainer);
            var iconView = iconObj.GetComponent<UnitIconView>();
            iconView.Setup(data, UnitIconView.HorizontalPortion.LeftHalf, 1);
        }
    }
}

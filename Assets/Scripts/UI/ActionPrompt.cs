using TMPro;
using UnityEngine;

public class ActionPrompt : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void Show(string message)
    {
        if (text != null) text.text = message ?? string.Empty;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

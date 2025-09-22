using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public UIPartyStatusManager partyStatusManager;
    public PlayerHandPanel playerHandPanel;
    public DisposedPanel disposedPanel;
    public ActionPrompt actionPrompt;
    public GovernmentPanel governmentPanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void ShowActionPrompt(string message)
    {
        actionPrompt?.Show(message);
    }

    public void HideActionPrompt()
    {
        actionPrompt?.Hide();
    }
}

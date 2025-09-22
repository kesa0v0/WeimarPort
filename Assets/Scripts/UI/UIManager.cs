using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public UIPartyStatusManager partyStatusManager;
    public PlayerHandPanel playerHandPanel;
    public DisposedPanel disposedPanel;
    public GovernmentPanel governmentPanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}

using UnityEngine;

/// <summary>
/// 씬에 있는 실제 위협 마커 GameObject를 나타내는 View입니다.
/// Presenter의 지시에 따라 외형을 변경하고, 사용자 입력을 Presenter에게 전달합니다.
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class ThreatMarkerView : MonoBehaviour
{
    private ThreatMarkerPresenter presenter;
    [SerializeField] private MeshRenderer objectRenderer;

    void Awake()
    {
        if (objectRenderer == null)
        {
            objectRenderer = GetComponent<MeshRenderer>();
        }
    }

    /// <summary>
    /// Presenter가 자신을 View에 등록하기 위해 호출하는 메서드입니다.
    /// </summary>
    public void Initialize(ThreatMarkerPresenter presenter)
    {
        this.presenter = presenter;
    }

    /// <summary>
    /// Presenter의 지시에 따라 머티리얼을 변경합니다.
    /// </summary>
    public void SetMaterial(Material newMaterial)
    {
        if (newMaterial != null && objectRenderer != null)
        {
            objectRenderer.sharedMaterial = newMaterial;
        }
        else
        {
            Debug.LogWarning("SetMaterial 호출 시 newMaterial이 null입니다.");
        }
    }

    /// <summary>
    /// 사용자가 이 오브젝트를 클릭했을 때 호출됩니다.
    /// </summary>
    void OnMouseDown()
    {
        // View는 로직을 처리하지 않고, 즉시 Presenter에게 보고합니다.
        if (presenter != null)
        {
            presenter.OnClicked();
        }
    }
}
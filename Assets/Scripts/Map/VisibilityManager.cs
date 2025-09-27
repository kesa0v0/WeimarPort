using UnityEngine;

/// <summary>
/// GameObject를 비활성화하지 않고 시각적 요소(Renderer, Canvas)만 제어하는 헬퍼 클래스입니다.
/// </summary>
public static class VisibilityManager
{
    /// <summary>
    /// 지정된 부모 오브젝트와 그 모든 자식들의 시각적 요소를 활성화/비활성화합니다.
    /// </summary>
    /// <param name="parent">대상 부모 GameObject</param>
    /// <param name="isVisible">보이게 할지 여부</param>
    public static void SetVisible(GameObject parent, bool isVisible)
    {
        if (parent == null) return;

        // 1. 부모 자신의 Renderer 켜고 끄기
        var parentRenderers = parent.GetComponents<Renderer>();
        foreach (var renderer in parentRenderers)
        {
            renderer.enabled = isVisible;
        }

        // 2. 부모 자신의 Canvas 켜고 끄기
        var parentCanvases = parent.GetComponents<Canvas>();
        foreach (var canvas in parentCanvases)
        {
            canvas.enabled = isVisible;
        }
        
        // 3. 모든 자식들의 Renderer 켜고 끄기
        var childRenderers = parent.GetComponentsInChildren<Renderer>(true); // 비활성화된 자식도 포함
        foreach (var renderer in childRenderers)
        {
            renderer.enabled = isVisible;
        }

        // 4. 모든 자식들의 Canvas 켜고 끄기
        var childCanvases = parent.GetComponentsInChildren<Canvas>(true);
        foreach (var canvas in childCanvases)
        {
            canvas.enabled = isVisible;
        }
    }
}


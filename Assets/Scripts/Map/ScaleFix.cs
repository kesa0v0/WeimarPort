using UnityEngine;

public static class ScaleFix
{
    public static Vector3 FixScale(Transform parent, Vector3 initialScale)
    {
        Vector3 scale = initialScale;
        if (parent != null)
        {
            Vector3 parentScale = parent.lossyScale;

            // 각 축이 0이면 1로 나누도록 처리 (0으로 나누기 방지, Plane 등 평면 부모 대응)
            scale = new Vector3(
                initialScale.x / (parentScale.x != 0 ? parentScale.x : 1),
                initialScale.y / (parentScale.y != 0 ? parentScale.y : 1),
                initialScale.z / (parentScale.z != 0 ? parentScale.z : 1)
            );
        }
        return scale;
    }
}

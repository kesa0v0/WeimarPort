using UnityEngine;

[System.Serializable]
    public class MarkerModel
    {
        public string InstanceId;       // 마커의 고유 인스턴스 ID (e.g., "poverty_1", "issue_economy_1")
        public string DataId;           // 원본 데이터 ID (e.g., "Threat_Poverty", "Issue_Economy")

        // 이슈 마커일 경우, 의견 트랙 위의 좌표 (룰북 p.20)
        public Vector2Int OpinionTrackPosition;
    }

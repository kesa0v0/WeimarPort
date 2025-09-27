using System;
using UnityEngine;

[System.Serializable]
    public class ThreatMarkerModel
    {
        /// <summary>
        /// 이 마커 인스턴스의 고유 식별자입니다.
        /// </summary>
        public string InstanceId;

        /// <summary>
        /// 이 마커의 원본 데이터(SO)를 가리키는 ID입니다.
        /// </summary>
        public string DataId;
        /// <summary>
        /// 이 마커의 현재 위치 데이터입니다.
        /// </summary>
        public LocationData CurrentLocation;
        
        /// <summary>
        /// 마커가 현재 뒤집힌 상태인지(번영 또는 비활성) 나타냅니다.
        /// </summary>
        public bool IsFlipped;

        // 런타임에서만 사용되는 원본 데이터 참조 (저장/로드 시에는 DataId를 사용)
        [NonSerialized]
        public ThreatMarkerData Data;
    }

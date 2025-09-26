
using System.Collections.Generic;

[System.Serializable]
    public class BoardStateModel
    {
        // 트랙 상태
        public TrackModel EconomyTrack = new TrackModel();             // 경제 트랙 (룰북 p.26)
        public TrackModel ForeignAffairsTrack = new TrackModel();    // 외교 트랙 (룰북 p.22)
        public TrackModel NSDAPTrack = new TrackModel();                // NSDAP 트랙 (룰북 p.26)

        // 도시 상태 (Key: 도시 이름 e.g., "Berlin")
        public Dictionary<string, CityModel> Cities = new Dictionary<string, CityModel>();

        // 의회 상태 (룰북 p.22)
        public ParliamentModel Parliament = new ParliamentModel();

        // 각종 공간 상태
        public OpinionTrackModel OpinionTrack = new OpinionTrackModel(); // 이슈가 놓이는 의견 트랙 (룰북 p.20)
        public SocietyTrackModel SocietyTrack = new SocietyTrackModel(); // 사회 마커가 놓이는 트랙 (룰북 p.20)
        public DRBoxModel DRBox = new DRBoxModel();                      // Deutsches Reich 박스 (룰북 p.24)
        public ForeignAffairsAreaModel ForeignAffairsArea = new ForeignAffairsAreaModel(); // 외교 카드 및 깃발 공간 (룰북 p.22)
    }

    
    [System.Serializable]
    public class TrackModel
    {
        public int CurrentPosition;
    }

    [System.Serializable]
    public class OpinionTrackModel
    {
        // 의견 트랙에 올라와 있는 이슈 마커의 인스턴스 ID 목록
        public List<string> IssueMarkerInstanceIds = new List<string>();
    }

    [System.Serializable]
    public class SocietyTrackModel
    {
        // 사회 트랙에 놓인 사회 마커의 데이터 ID 목록 (순서가 중요)
        public List<string> SocietyMarkerDataIds = new List<string>();
    }

    [System.Serializable]
    public class DRBoxModel
    {
        // DR 박스에 있는 위협 마커의 인스턴스 ID 목록
        public List<string> ThreatMarkerInstanceIds = new List<string>();
    }

    [System.Serializable]
    public class ForeignAffairsAreaModel
    {
        // 협상 가능한 외교 카드 ID 목록
        public List<string> AvailableCardIds = new List<string>();
        // 현재 놓인 깃발 (Key: FlagType e.g., "USA_UK", Value: 깃발 개수)
        public Dictionary<string, int> Flags = new Dictionary<string, int>();
    }
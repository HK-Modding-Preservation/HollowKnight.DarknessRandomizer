using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DarknessRandomizer.Data
{
    [JsonConverter(typeof(ClusterNameConverter))]
    public class ClusterName
    {
        private static readonly Dictionary<string, ClusterName> clustersByName = new();
        private static readonly List<ClusterName> clustersById = new();
        private static int NextId = 0;

        public readonly string Name;
        public readonly int Id;

        private ClusterName(string name)
        {
            if (clustersByName.ContainsKey(name))
            {
                throw new ArgumentException($"Duplicate cluster: {name}");
            }

            this.Name = name;
            this.Id = NextId++;
            clustersByName.Add(name, this);
            clustersById.Add(this);
        }

        public static bool TryGetClusterName(string s, out ClusterName cluster) => clustersByName.TryGetValue(s, out cluster);

        public static ClusterName FromId(int id) => clustersById[id];

        public static bool IsScene(string s) => clustersByName.ContainsKey(s);

        public override string ToString() => Name;

        public static int NumSceneNames() => clustersById.Count;

        public override bool Equals(object obj)
        {
            return obj is ClusterName name &&
                   Id == name.Id;
        }

        public override int GetHashCode() => Id;

        // @@@ INSERT_CLUSTER_NAMES START @@@
        public static readonly ClusterName AbyssBirthplace = new("AbyssBirthplace");
        public static readonly ClusterName AbyssClimb = new("AbyssClimb");
        public static readonly ClusterName AbyssCore = new("AbyssCore");
        public static readonly ClusterName AbyssLifebloodCore = new("AbyssLifebloodCore");
        public static readonly ClusterName AbyssShadeCloakWing = new("AbyssShadeCloakWing");
        public static readonly ClusterName AbyssShriekWing = new("AbyssShriekWing");
        public static readonly ClusterName ArchivesMonomon = new("ArchivesMonomon");
        public static readonly ClusterName BasinBrokenVesselWing = new("BasinBrokenVesselWing");
        public static readonly ClusterName BasinCloth = new("BasinCloth");
        public static readonly ClusterName BasinFountain = new("BasinFountain");
        public static readonly ClusterName BasinLostKin = new("BasinLostKin");
        public static readonly ClusterName BasinPalaceWing = new("BasinPalaceWing");
        public static readonly ClusterName BlackEggRadiance = new("BlackEggRadiance");
        public static readonly ClusterName BlueLake = new("BlueLake");
        public static readonly ClusterName CityBridgeToBasin = new("CityBridgeToBasin");
        public static readonly ClusterName CityLurien = new("CityLurien");
        public static readonly ClusterName CliffsBaldurShell = new("CliffsBaldurShell");
        public static readonly ClusterName CliffsJonis = new("CliffsJonis");
        public static readonly ClusterName CliffsMain = new("CliffsMain");
        public static readonly ClusterName CrossroadsAncestralMound = new("CrossroadsAncestralMound");
        public static readonly ClusterName CrossroadsCanyonBridge = new("CrossroadsCanyonBridge");
        public static readonly ClusterName CrossroadsCentralPass = new("CrossroadsCentralPass");
        public static readonly ClusterName CrossroadsElevatorApproach = new("CrossroadsElevatorApproach");
        public static readonly ClusterName CrossroadsEntrance = new("CrossroadsEntrance");
        public static readonly ClusterName CrossroadsFailedChampion = new("CrossroadsFailedChampion");
        public static readonly ClusterName CrossroadsFalseKnight = new("CrossroadsFalseKnight");
        public static readonly ClusterName CrossroadsFungalBridge = new("CrossroadsFungalBridge");
        public static readonly ClusterName CrossroadsGlowingWomb = new("CrossroadsGlowingWomb");
        public static readonly ClusterName CrossroadsGoamJournal = new("CrossroadsGoamJournal");
        public static readonly ClusterName CrossroadsGreenpathBridge = new("CrossroadsGreenpathBridge");
        public static readonly ClusterName CrossroadsGrubfather = new("CrossroadsGrubfather");
        public static readonly ClusterName CrossroadsHotSpring = new("CrossroadsHotSpring");
        public static readonly ClusterName CrossroadsLowerPass = new("CrossroadsLowerPass");
        public static readonly ClusterName CrossroadsMawlek = new("CrossroadsMawlek");
        public static readonly ClusterName CrossroadsMawlekApproach = new("CrossroadsMawlekApproach");
        public static readonly ClusterName CrossroadsMiddleBridge = new("CrossroadsMiddleBridge");
        public static readonly ClusterName CrossroadsOutsideMound = new("CrossroadsOutsideMound");
        public static readonly ClusterName CrossroadsOutsideTemple = new("CrossroadsOutsideTemple");
        public static readonly ClusterName CrossroadsPeaksBridge = new("CrossroadsPeaksBridge");
        public static readonly ClusterName CrossroadsSpikeGrub = new("CrossroadsSpikeGrub");
        public static readonly ClusterName CrossroadsStag = new("CrossroadsStag");
        public static readonly ClusterName CrossroadsStagHub = new("CrossroadsStagHub");
        public static readonly ClusterName CrossroadsTram = new("CrossroadsTram");
        public static readonly ClusterName CrossroadsVesselFragment = new("CrossroadsVesselFragment");
        public static readonly ClusterName CrossroadsWest = new("CrossroadsWest");
        public static readonly ClusterName DeepnestBeastsDen = new("DeepnestBeastsDen");
        public static readonly ClusterName DeepnestDark = new("DeepnestDark");
        public static readonly ClusterName DeepnestDescent = new("DeepnestDescent");
        public static readonly ClusterName DeepnestFailedTram = new("DeepnestFailedTram");
        public static readonly ClusterName DeepnestGalien = new("DeepnestGalien");
        public static readonly ClusterName DeepnestHerrah = new("DeepnestHerrah");
        public static readonly ClusterName DeepnestLightEast = new("DeepnestLightEast");
        public static readonly ClusterName DeepnestLightWest = new("DeepnestLightWest");
        public static readonly ClusterName DeepnestMantisEntrance = new("DeepnestMantisEntrance");
        public static readonly ClusterName DeepnestNosk = new("DeepnestNosk");
        public static readonly ClusterName DeepnestNoskApproach = new("DeepnestNoskApproach");
        public static readonly ClusterName DeepnestStag = new("DeepnestStag");
        public static readonly ClusterName DeepnestTramCorridor = new("DeepnestTramCorridor");
        public static readonly ClusterName DeepnestTramOutliers = new("DeepnestTramOutliers");
        public static readonly ClusterName DeepnestVillage = new("DeepnestVillage");
        public static readonly ClusterName DeepnestWeaversDen = new("DeepnestWeaversDen");
        public static readonly ClusterName DirtmouthGPZ = new("DirtmouthGPZ");
        public static readonly ClusterName EdgeCampBench = new("EdgeCampBench");
        public static readonly ClusterName EdgeColoApproach = new("EdgeColoApproach");
        public static readonly ClusterName EdgeColoCorridor = new("EdgeColoCorridor");
        public static readonly ClusterName EdgeColumn = new("EdgeColumn");
        public static readonly ClusterName EdgeEastUpper = new("EdgeEastUpper");
        public static readonly ClusterName EdgeHornetApproach = new("EdgeHornetApproach");
        public static readonly ClusterName EdgeHornetSentinel = new("EdgeHornetSentinel");
        public static readonly ClusterName EdgeLifeblood = new("EdgeLifeblood");
        public static readonly ClusterName EdgeMarkoth = new("EdgeMarkoth");
        public static readonly ClusterName EdgeOroDive = new("EdgeOroDive");
        public static readonly ClusterName EdgeOroStrip = new("EdgeOroStrip");
        public static readonly ClusterName EdgePaleLurker = new("EdgePaleLurker");
        public static readonly ClusterName EdgeTramAppraoch = new("EdgeTramAppraoch");
        public static readonly ClusterName EdgeTramEntrance = new("EdgeTramEntrance");
        public static readonly ClusterName EdgeUpperEast = new("EdgeUpperEast");
        public static readonly ClusterName FogCanyonArchives = new("FogCanyonArchives");
        public static readonly ClusterName FogCanyonAutoPilot = new("FogCanyonAutoPilot");
        public static readonly ClusterName FogCanyonCharmNotch = new("FogCanyonCharmNotch");
        public static readonly ClusterName FogCanyonEast = new("FogCanyonEast");
        public static readonly ClusterName FogCanyonOutsideArchives = new("FogCanyonOutsideArchives");
        public static readonly ClusterName FogCanyonQGA = new("FogCanyonQGA");
        public static readonly ClusterName FogCanyonRobber = new("FogCanyonRobber");
        public static readonly ClusterName FogCanyonWest = new("FogCanyonWest");
        public static readonly ClusterName FogCanyonWestWing = new("FogCanyonWestWing");
        public static readonly ClusterName FungalClothCorridor = new("FungalClothCorridor");
        public static readonly ClusterName FungalCore = new("FungalCore");
        public static readonly ClusterName FungalCorniferHub = new("FungalCorniferHub");
        public static readonly ClusterName FungalDeepnestFall = new("FungalDeepnestFall");
        public static readonly ClusterName FungalElderHuWing = new("FungalElderHuWing");
        public static readonly ClusterName FungalEntrance = new("FungalEntrance");
        public static readonly ClusterName FungalLowerHub = new("FungalLowerHub");
        public static readonly ClusterName FungalMantisLords = new("FungalMantisLords");
        public static readonly ClusterName FungalMantisRewards = new("FungalMantisRewards");
        public static readonly ClusterName FungalMantisVillage = new("FungalMantisVillage");
        public static readonly ClusterName FungalOgres = new("FungalOgres");
        public static readonly ClusterName FungalPilgrimsWay = new("FungalPilgrimsWay");
        public static readonly ClusterName FungalQueensStation = new("FungalQueensStation");
        public static readonly ClusterName FungalSporeShroom = new("FungalSporeShroom");
        public static readonly ClusterName FungalUpper = new("FungalUpper");
        public static readonly ClusterName GardensCornifer = new("GardensCornifer");
        public static readonly ClusterName GardensDeepnestBridge = new("GardensDeepnestBridge");
        public static readonly ClusterName GardensEntrance = new("GardensEntrance");
        public static readonly ClusterName GardensFrogsWing = new("GardensFrogsWing");
        public static readonly ClusterName GardensLowerPass = new("GardensLowerPass");
        public static readonly ClusterName GardensMainArena = new("GardensMainArena");
        public static readonly ClusterName GardensPetraArena = new("GardensPetraArena");
        public static readonly ClusterName GardensStag = new("GardensStag");
        public static readonly ClusterName GardensStagHub = new("GardensStagHub");
        public static readonly ClusterName GardensTollBench = new("GardensTollBench");
        public static readonly ClusterName GardensTraitorLord = new("GardensTraitorLord");
        public static readonly ClusterName GardensTraitorsGrave = new("GardensTraitorsGrave");
        public static readonly ClusterName GardensUpper = new("GardensUpper");
        public static readonly ClusterName GreenpathHornet = new("GreenpathHornet");
        public static readonly ClusterName GreenpathHunter = new("GreenpathHunter");
        public static readonly ClusterName GreenpathLowerHub = new("GreenpathLowerHub");
        public static readonly ClusterName GreenpathLowerPass = new("GreenpathLowerPass");
        public static readonly ClusterName GreenpathMMC = new("GreenpathMMC");
        public static readonly ClusterName GreenpathSheo = new("GreenpathSheo");
        public static readonly ClusterName GreenpathStag = new("GreenpathStag");
        public static readonly ClusterName GreenpathStoneSanctuary = new("GreenpathStoneSanctuary");
        public static readonly ClusterName GreenpathStoneSanctuaryApproach = new("GreenpathStoneSanctuaryApproach");
        public static readonly ClusterName GreenpathStoneSanctuaryBench = new("GreenpathStoneSanctuaryBench");
        public static readonly ClusterName GreenpathThorns = new("GreenpathThorns");
        public static readonly ClusterName GreenpathToll = new("GreenpathToll");
        public static readonly ClusterName GreenpathUnn = new("GreenpathUnn");
        public static readonly ClusterName GreenpathUnnBridge = new("GreenpathUnnBridge");
        public static readonly ClusterName GreenpathUpperEast = new("GreenpathUpperEast");
        public static readonly ClusterName GreenpathUpperWest = new("GreenpathUpperWest");
        public static readonly ClusterName GreenpathWest = new("GreenpathWest");
        public static readonly ClusterName GroundsDreamNail = new("GroundsDreamNail");
        public static readonly ClusterName GroundsGlade = new("GroundsGlade");
        public static readonly ClusterName GroundsSeer = new("GroundsSeer");
        public static readonly ClusterName GroundsShrineOfBelievers = new("GroundsShrineOfBelievers");
        public static readonly ClusterName GroundsTram = new("GroundsTram");
        public static readonly ClusterName KingsPass = new("KingsPass");
        public static readonly ClusterName PalaceEntrance = new("PalaceEntrance");
        public static readonly ClusterName PalaceFinale = new("PalaceFinale");
        public static readonly ClusterName PalaceHub = new("PalaceHub");
        public static readonly ClusterName PalaceInnerKingsmould = new("PalaceInnerKingsmould");
        public static readonly ClusterName PalacePOPEntrance = new("PalacePOPEntrance");
        public static readonly ClusterName PalacePOPFinal = new("PalacePOPFinal");
        public static readonly ClusterName PalaceThrone = new("PalaceThrone");
        public static readonly ClusterName PalaceWings = new("PalaceWings");
        public static readonly ClusterName PalaceWorkshop = new("PalaceWorkshop");
        public static readonly ClusterName SanctumSoulTyrant = new("SanctumSoulTyrant");
        public static readonly ClusterName WaterwaysDungDefender = new("WaterwaysDungDefender");
        public static readonly ClusterName WaterwaysDungDefenderCave = new("WaterwaysDungDefenderCave");
        public static readonly ClusterName WaterwaysEdgeAcidCorridor = new("WaterwaysEdgeAcidCorridor");
        public static readonly ClusterName WaterwaysFlukemarm = new("WaterwaysFlukemarm");
        public static readonly ClusterName WaterwaysIsmasGrove = new("WaterwaysIsmasGrove");
        public static readonly ClusterName WaterwaysJunkPit = new("WaterwaysJunkPit");
        public static readonly ClusterName WaterwaysMain = new("WaterwaysMain");
        public static readonly ClusterName WaterwaysOutsideFlukemarm = new("WaterwaysOutsideFlukemarm");
        public static readonly ClusterName WaterwaysWestWing = new("WaterwaysWestWing");
        public static readonly ClusterName WaterwaysWhiteDefender = new("WaterwaysWhiteDefender");
        public static readonly ClusterName WestCityElevator = new("WestCityElevator");
        // @@@ INSERT_CLUSTER_NAMES END @@@
    }

    class ClusterNameConverter : JsonConverter<ClusterName>
    {
        public override ClusterName ReadJson(JsonReader reader, Type objectType, ClusterName existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (serializer.Deserialize(reader, typeof(string)) is string name && ClusterName.TryGetClusterName(name, out ClusterName clusterName))
            {
                return clusterName;
            }
            throw new JsonReaderException("Error decoding ClusterName");
        }

        public override void WriteJson(JsonWriter writer, ClusterName value, JsonSerializer serializer) => serializer.Serialize(writer, value.Name);
    }
}

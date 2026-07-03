using System;
using System.ComponentModel.Design;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Delegates;
using StardewValley.Events;
using StardewValley.Extensions;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Shops;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile;
using xTile.Dimensions;
using xTile.Tiles;

namespace GoldenWalnutFramework
{
    internal sealed class ModEntry : Mod
    {
        public static ModEntry? Instance;
        public static MainFunctions? mf;
        public static HandleCommands? cmds;
        public string? customHintForToday = null;
        public int additionalGoldenWalnuts = 0;
        public int GoldenWalnutCap = IslandLocation.TOTAL_WALNUTS;
        public int SpentWalnutsForPUPs = 0;
        public int walnutGroupCountForTranspiler = 0;
        public int vanillaWalnutCount = 0;
        public bool disableWalnutCap = false;
        public bool dayIsResetting = true;
        public bool playJingle = false;
        public bool? seasonalTerrainFeatures = null;
        public HashSet<string> excludedMapsFromSeasonalFeatures = new(StringComparer.OrdinalIgnoreCase);
        public List<ParrotUpgradePerch> Custom_parrotUpgradePerches = [];
        public List<KeyValuePair<string, Vector2>> vanillaBushes = [];
        public HashSet<string> unobtainableWalnuts = [];
        public Dictionary<string, string[]> GameStateQueryWalnutGroups = [];

        public List<string> vanillaWalnutIDs = [
            "Bush_IslandNorth_13_33", "Bush_IslandNorth_5_30", "Buried_IslandNorth_19_39", "Bush_IslandNorth_4_42", "Bush_IslandNorth_45_38", "Bush_IslandNorth_47_40", "IslandLeftPlantRestored", "IslandRightPlantRestored", "IslandBatRestored", "IslandFrogRestored",
            "IslandCenterSkeletonRestored", "IslandSnakeRestored", "Buried_IslandNorth_19_13", "Buried_IslandNorth_57_79", "Buried_IslandNorth_54_21", "Buried_IslandNorth_42_77", "Buried_IslandNorth_62_54", "Buried_IslandNorth_26_81", "Bush_IslandNorth_20_26", "Bush_IslandNorth_9_84",
            "Bush_IslandNorth_56_27", "Bush_IslandSouth_31_5", "TreeNut", "IslandWestCavePuzzle", "SandDuggy", "TigerSlimeNut", "Buried_IslandWest_21_81", "Buried_IslandWest_62_76", "Buried_IslandWest_39_24", "Buried_IslandWest_88_14",
            "Buried_IslandWest_43_74", "Buried_IslandWest_30_75", "MusselStone", "IslandFarming", "Bush_IslandWest_104_3", "Bush_IslandWest_31_24", "Bush_IslandWest_38_56", "Bush_IslandWest_75_29", "Bush_IslandWest_64_30", "Bush_IslandWest_54_18",
            "Bush_IslandWest_25_30", "Bush_IslandWest_15_3", "IslandFishing", "VolcanoNormalChest", "VolcanoRareChest", "VolcanoBarrel", "VolcanoMining", "VolcanoMonsterLoot", "Island_N_BuriedTreasureNut", "Island_W_BuriedTreasureNut",
            "Island_W_BuriedTreasureNut2", "Mermaid", "TreeNutShot", "Buried_IslandSouthEastCave_36_26", "Buried_IslandSouthEast_25_17", "StardropPool", "Bush_Caldera_28_36", "Bush_Caldera_9_34", "Bush_CaptainRoom_2_4", "BananaShrine",
            "Bush_IslandEast_17_37", "Darts", "IslandGourmand1", "IslandGourmand2", "IslandGourmand3", "IslandShrinePuzzle", "Bush_IslandShrine_23_34"
        ]; //unused. Just to look things up

        public HashSet<string> vanillaBushIDs = new(StringComparer.OrdinalIgnoreCase)
        {
            "Bush_IslandNorth_13_33", "Bush_IslandNorth_5_30", "Bush_IslandNorth_4_42", "Bush_IslandNorth_45_38", "Bush_IslandNorth_47_40", "Bush_IslandNorth_20_26", "Bush_IslandNorth_9_84",
            "Bush_IslandNorth_56_27", "Bush_IslandSouth_31_5", "Bush_IslandWest_104_3", "Bush_IslandWest_31_24", "Bush_IslandWest_38_56", "Bush_IslandWest_75_29", "Bush_IslandWest_64_30", "Bush_IslandWest_54_18",
            "Bush_IslandWest_25_30", "Bush_IslandWest_15_3", "Bush_Caldera_28_36", "Bush_Caldera_9_34", "Bush_CaptainRoom_2_4", "Bush_IslandEast_17_37", "Bush_IslandShrine_23_34"
        }; //used. Not just to look things up



        public override void Entry(IModHelper helper)
        {
            Instance = this;
            mf = new();
            cmds = new();
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.DayEnding += GameLoop_DayEnding;
            helper.Events.World.TerrainFeatureListChanged += World_TerrainFeatureListChanged;
            helper.Events.World.ObjectListChanged += World_ObjectListChanged;
            helper.Events.World.NpcListChanged += World_NpcListChanged;
            helper.Events.Player.Warped += Player_Warped;
            helper.Events.Content.AssetRequested += Content_AssetRequested;

            helper.ConsoleCommands.Add("ShowAllWalnutIDs", "Gives you all the walnut IDs that you added to the game", cmds.ShowAllWalnutIDs);
            helper.ConsoleCommands.Add("ShowMailFlags", "Lists all the Mailflags that the host currently has", cmds.ShowMailFlags);
            helper.ConsoleCommands.Add("ShowWalnuts", "Lists all currently collected walnuts", cmds.ShowWalnuts);
            helper.ConsoleCommands.Add("ShowStoneTypeIDs", "Lists the IDs for all the Stone Nodes in the game", cmds.ShowStoneTypeIDs);
            helper.ConsoleCommands.Add("RemoveMailFlag", "Lets you remove a Mailflag. Mainly used for resetting ParrotUpgradePerches. Remember to save the day!", cmds.RemoveMailFlag);
            helper.ConsoleCommands.Add("RemoveWalnut", "Lets you remove a walnut, setting it back to it can be collected again. Remember to save the day!", cmds.RemoveWalnut);

            GameStateQuery.Register("COMPLETED_WALNUTGROUP", cmds.WalnutGroupGSQ);
            GameStateQuery.Register("FOUND_EVERY_WALNUT", cmds.WalnutCountGSQ);

            foreach (var vanillaBush in vanillaBushIDs)
            {
                string[] splitString = vanillaBush.Split('_');
                vanillaBushes.Add(new KeyValuePair<string, Vector2>(splitString[1], new Vector2(int.Parse(splitString[2]), int.Parse(splitString[3]))));
            }
            



            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(Farmer),
                    "foundWalnut"
                ),
                prefix: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.foundWalnut_Prefix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(Game1),
                    "RecountWalnuts"
                ),
                postfix: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.RecountWalnuts_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(IslandHut),
                    "MissingLimitedNutDrops"
                ),
                prefix: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.MissingLimitedNutDrops_Prefix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(IslandHut),
                    "UpdateWhenCurrentLocation"
                ),
                prefix: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.IslandHutUpdateWhenCurrentLocation_Prefix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(GameLocation),
                    "ShowQiCat"
                ),
                transpiler: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.WalnutCapReplaceTranspiler)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(Utility),
                    "percentGameComplete"
                ),
                transpiler: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.WalnutCapReplaceTranspiler)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(WorldChangeEvent),
                    "resetForPlayerEntry"
                ),
                transpiler: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.WalnutCapReplaceTranspiler)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Constructor(
                typeof(IslandNorth)
                ),
                postfix: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.IslandNorthGoldenParrot_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(ParrotUpgradePerch),
                    "CheckAction"
                ),
                postfix: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.CheckActionForGoldenParrot_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(ParrotUpgradePerch),
                    "AnswerQuestion"
                ),
                postfix: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.AnswerQuestionForGoldenParrot_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(ParrotUpgradePerch),
                    "ActivateGoldenParrot"
                ),
                postfix: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.ActivateGoldenParrot_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(IslandFieldOffice),
                    "donatePiece"
                ),
                prefix: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.donatePiece_Prefix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(IslandFieldOffice),
                    "ApplyPlantRestoreLeft"
                ),
                postfix: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.ApplyPlantRestore_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(IslandFieldOffice),
                    "ApplyPlantRestoreRight"
                ),
                postfix: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.ApplyPlantRestore_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(FishingRod),
                    "DoFunction"
                ),
                prefix: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.FishingRodDoFunction_Prefix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(GameLocation),
                    "DayUpdate"
                ),
                postfix: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.GameLocationDayUpdateHarmony_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(GameLocation),
                    "loadMap"
                ),
                postfix: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.loadMap_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(GameLocation),
                    "resetLocalState"
                ),
                postfix: new HarmonyMethod(
                    typeof(BasePatches),
                    nameof(BasePatches.GameLocationResetLocalStateHarmony_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(GameLocation),
                    "UpdateWhenCurrentLocation"
                ),
                postfix: new HarmonyMethod(
                    typeof(BasePatches),
                    nameof(BasePatches.GameLocationUpdateWhenCurrentLocationHarmony_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(GameLocation),
                    "draw"
                ),
                postfix: new HarmonyMethod(
                    typeof(BasePatches),
                    nameof(BasePatches.GameLocationDrawHarmony_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(GameLocation),
                    "answerDialogue"
                ),
                postfix: new HarmonyMethod(
                    typeof(BasePatches),
                    nameof(BasePatches.GameLocationAnswerDialogueHarmony_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(GameLocation),
                    "cleanupBeforePlayerExit"
                ),
                postfix: new HarmonyMethod(
                    typeof(BasePatches),
                    nameof(BasePatches.GameLocationCleanupBeforePlayerExitHarmony_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(GameLocation),
                    "updateEvenIfFarmerIsntHere"
                ),
                postfix: new HarmonyMethod(
                    typeof(BasePatches),
                    nameof(BasePatches.GameLocationUpdateEvenIfFarmerIsntHereHarmony_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(GameLocation),
                    "TransferDataFromSavedLocation"
                ),
                postfix: new HarmonyMethod(
                    typeof(BasePatches),
                    nameof(BasePatches.GameLocationTransferDataFromSavedLocationHarmony_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(GameLocation),
                    "isActionableTile"
                ),
                postfix: new HarmonyMethod(
                    typeof(BasePatches),
                    nameof(BasePatches.GameLocationIsActionableTileHarmony_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(GameLocation),
                    "checkAction"
                ),
                postfix: new HarmonyMethod(
                    typeof(BasePatches),
                    nameof(BasePatches.GameLocationCheckActionHarmony_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(GameLocation),
                    "drawAboveAlwaysFrontLayer"
                ),
                postfix: new HarmonyMethod(
                    typeof(BasePatches),
                    nameof(BasePatches.GameLocationDrawAboveAlwaysFrontLayerHarmony_Postfix)
                )
            );

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(Bush),
                    "shake"
                ),
                prefix: new HarmonyMethod(
                    typeof(BasePatches),
                    nameof(BasePatches.BushShake_Prefix)
                ),
                postfix: new HarmonyMethod(
                    typeof(BasePatches),
                    nameof(BasePatches.BushShake_Postfix)
                )
            );
        }
        private bool launchedFirstCheck;
        private void GameLoop_UpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (launchedFirstCheck) { return; }  
            launchedFirstCheck = true;
            FirstCheck();

            new Harmony(ModManifest.UniqueID).Patch(
                original: AccessTools.Method(
                    typeof(IslandHut),
                    "ShowNutHint"
                ),
                transpiler: new HarmonyMethod(
                    typeof(MainPatches),
                    nameof(MainPatches.ShowNutHint_Transpiler)
                )
            );
        }

        private void FirstCheck()
        {
            var json = Game1.content.Load<BaseJSON>("Mods/GoldenWalnutFramework/Data");
            mf!.RegisterJSONSettings(json.Settings);
            foreach (var perch in json.ParrotUpgradePerches)
            {
                mf!.CheckJSONPerches(perch);
            }
            foreach (var group in json.GoldenWalnuts)
            {
                if (string.IsNullOrWhiteSpace(group.Value.Hint))
                {
                    Monitor.Log($"Walnutgroup {group.Key} is missing a field for the Hint!", LogLevel.Error);
                    continue;
                }
                if (group.Value.Walnuts == null)
                {
                    Monitor.Log($"Walnutgroup {group.Key} is missing a field for Walnuts!", LogLevel.Error);
                    continue;
                }
                if (group.Value.SeparateHint != true)
                {
                    walnutGroupCountForTranspiler++;
                }
                foreach (var walnut in group.Value.Walnuts)
                {
                    mf!.CheckJSONWalnuts(group.Key, walnut);
                }
            }
        }

        private void GameLoop_SaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            Game1.MasterPlayer.mailReceived.OnValueAdded += mf!.MailReceived_OnValueAdded;

            Helper.GameContent.InvalidateCache("TileSheets/bushes");
            Helper.GameContent.InvalidateCache("TerrainFeatures/tree_palm");

            FillMainLists();
            CalculateCap();
            AddQiShopConditions();
            RemoveOldBushes();
            CheckForInvalidPlacedBushes();
            GiveUnobtainableWalnuts();
        }

        public void FillMainLists()
        {
            MainPatches.WalnutGroups.Clear();
            MainPatches.WalnutGroupsSingular.Clear();
            MainPatches.WalnutGroupsHintConditions.Clear();
            MainPatches.SeparateWalnutGroups.Clear();
            MainPatches.SeparateWalnutGroupsSingular.Clear();
            MainPatches.SeparateWalnutGroupsHintConditions.Clear();
            MainPatches.WalnutLocations.Clear();
            MainPatches.CustomPerches.Clear();
            MainPatches.QiShopFlags.Clear();
            MainPatches.CustomWalnuts.Clear();
            excludedMapsFromSeasonalFeatures.Clear();
            Custom_parrotUpgradePerches.Clear();
            unobtainableWalnuts.Clear();
            GameStateQueryWalnutGroups.Clear();
            disableWalnutCap = false;
            customHintForToday = null;

            int walnutGroupCount = 0;
            var json = Game1.content.Load<BaseJSON>("Mods/GoldenWalnutFramework/Data");

            mf!.RegisterJSONSettings(json.Settings);

            foreach (var group in json.GoldenWalnuts)
            {
                if (string.IsNullOrWhiteSpace(group.Value.Hint))
                {
                    Monitor.Log($"Walnutgroup {group.Key} is missing a field for the Hint!", LogLevel.Error);
                    continue;
                }
                if (group.Value.Walnuts == null)
                {
                    Monitor.Log($"Walnutgroup {group.Key} is missing a field for Walnuts!", LogLevel.Error);
                    continue;
                }

                if (group.Value.ShowThisHint == true)
                {
                    if (group.Value.SeparateHint == true)
                    {
                        if (customHintForToday != null)
                        {
                            Monitor.Log("Multiple Walnutgroups have ShowThisHint set to true. You can only use this for one Hint with and one without having 'SeparateHint' = true!", LogLevel.Error);
                        }
                        else
                        {
                            customHintForToday = group.Value.Hint;
                        }
                    }
                    else
                    {
                        IslandHut hut = (IslandHut)Game1.getLocationFromName("IslandHut");
                        if (hut.hintForToday.Value != null)
                        {
                            Monitor.Log("Multiple Walnutgroups have ShowThisHint set to true. You can only use this for one Hint with and one Hint without having 'SeparateHint' = true!", LogLevel.Error);
                        }
                        else
                        {
                            hut.hintForToday.Value = group.Value.Hint;
                        }
                    }
                    Monitor.Log("ShowThisHint is enabled. Before you release the mod, remember to disable it again. If you are a player, the modder messed up. Write him a bug report.", LogLevel.Warn);
                }
                string[] currentWalnutIDs = [];
                foreach (var walnut in group.Value.Walnuts)
                {
                    if (!mf!.CheckJSONWalnuts(group.Key, walnut)) { continue; }
                    if (!walnut.Type!.Equals("Custom", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!Game1.locations.Contains(Game1.getLocationFromName(walnut.Location)))
                        {
                            Monitor.Log($"Group '{group.Key}', Walnut '{walnut.ID}': contains a walnut with the Location: '{walnut.Location}', which couldn't be found in the game's locations", LogLevel.Error);
                            continue;
                        }
                        else
                        {
                            MainPatches.WalnutLocations.Add(walnut.Location!);
                        }
                    }
                    if (MainPatches.CustomWalnuts.Any(storedWalnut => storedWalnut.ID == walnut.ID))
                    {
                        Monitor.Log($"Multiple walnuts with this ID: '{walnut.ID}' have been detected. If you are a player, you can give yourself a walnut for each time that you see this error to compensate for the missing walnuts. To do this, type into the SMAPI Console 'debug item 73 x' and for x the amount of times you get this error. If you are a modder, DO NOT add the same ID twice. If you didn't do that, another mod that you have currently installed also used the same ID. Please just use another ID in that case. (Are you using the {{ModID}} token)?", LogLevel.Error);
                        continue;
                    }
                    if (group.Value.HintConditions != null)
                    {
                        if (walnut.Conditions != null)
                        {
                            walnut.Conditions += $", {group.Value.HintConditions}";
                        }
                        else
                        {
                            walnut.Conditions = group.Value.HintConditions;
                        }
                    }
                    if (walnut.Type.Equals("Bush", StringComparison.OrdinalIgnoreCase)) { mf.SpawnBushes(walnut); }
                    if (walnut.Count == null || walnut.Count == 1 || walnut.Type.Equals("Bush", StringComparison.OrdinalIgnoreCase))
                    {
                        currentWalnutIDs = [.. currentWalnutIDs.AddItem(walnut.ID!)];
                    }
                    else
                    {
                        for (int j = 1; j <= walnut.Count; j++)
                        {
                            currentWalnutIDs = [.. currentWalnutIDs.AddItem($"{walnut.ID}_{j}")];
                        }
                    }
                    MainPatches.CustomWalnuts.Add(walnut);
                }
                if (group.Value.SeparateHint == true)
                {
                    MainPatches.SeparateWalnutGroups.Add(new KeyValuePair<string, string[]>(group.Value.Hint, currentWalnutIDs));
                    MainPatches.SeparateWalnutGroupsSingular.Add(group.Value.Singular ?? "NoSingularAssigned");
                    MainPatches.SeparateWalnutGroupsHintConditions.Add(group.Value.HintConditions ?? "");
                }
                else
                {
                    walnutGroupCount++;
                    MainPatches.WalnutGroups.Add(new KeyValuePair<string, string[]>(group.Value.Hint, currentWalnutIDs));
                    MainPatches.WalnutGroupsSingular.Add(group.Value.Singular ?? "NoSingularAssigned");
                    MainPatches.WalnutGroupsHintConditions.Add(group.Value.HintConditions ?? "");
                }
                GameStateQueryWalnutGroups[group.Key] = currentWalnutIDs;
            }
            foreach (var perch in json.ParrotUpgradePerches)
            {
                if (!mf!.CheckJSONPerches(perch)) { continue; }
                var perchLocation = Game1.getLocationFromName(perch.Location);

                if (perchLocation == null)
                {
                    Monitor.Log($"ParrotUpgradePerch: '{perch.ID}' has the Location: {perch.Location} that could not be found in the game's locations", LogLevel.Error);
                    continue;
                }
                string perchName = perch.ID!;
                if (perch.StoneAnimation == true)
                {
                    perchName += "-Volcano";
                }
                else
                {
                    if (perchName.Contains("Volcano"))
                    {
                        perchName = perchName.Replace("Volcano", "_______");
                    }
                    perchName += "-_______";
                }
                if (perch.StickType != null)
                {
                    TileSheet tileSheet;
                    if (perchLocation.InDesertContext() || perchLocation.InIslandContext() || excludedMapsFromSeasonalFeatures.Contains(perch.Location!, StringComparer.OrdinalIgnoreCase))
                    {
                        tileSheet = new("zzz_GWF/ParrotSticks", perchLocation.Map, "ResoNight.GoldenWalnutFramework/ParrotSticks", new Size(2, 2), new Size(16, 16));
                    }
                    else
                    {
                        tileSheet = new("zzz_GWF/ParrotSticks", perchLocation.Map, $"ResoNight.GoldenWalnutFramework/{Game1.currentSeason}_ParrotSticks", new Size(2, 2), new Size(16, 16));
                    }
                        
                    perchLocation.Map.AddTileSheet(tileSheet);

                    if (perch.StickType.Equals("Wood", StringComparison.OrdinalIgnoreCase))
                    {
                        perchLocation.setMapTile((int)perch.ParrotTile!.X!, (int)perch.ParrotTile.Y!, 2, "Buildings", "zzz_GWF/ParrotSticks");
                        perchLocation.setMapTile((int)perch.ParrotTile!.X!, (int)perch.ParrotTile.Y! - 1, 0, "Front", "zzz_GWF/ParrotSticks");
                    }
                    else if (perch.StickType.Equals("Plant", StringComparison.OrdinalIgnoreCase))
                    {
                        perchLocation.setMapTile((int)perch.ParrotTile!.X!, (int)perch.ParrotTile.Y!, 3, "Buildings", "zzz_GWF/ParrotSticks");
                        perchLocation.setMapTile((int)perch.ParrotTile!.X!, (int)perch.ParrotTile.Y! - 1, 1, "Front", "zzz_GWF/ParrotSticks");
                    }
                }
                if (!Game1.MasterPlayer.mailReceived.Contains(perch.ID))
                {
                    Custom_parrotUpgradePerches.Add(new ParrotUpgradePerch(perchLocation, new Point(perch.ParrotTile!.X!.Value, perch.ParrotTile!.Y!.Value), new Microsoft.Xna.Framework.Rectangle(perch.ParrotArea!.X!.Value, perch.ParrotArea!.Y!.Value, perch.ParrotArea!.Width!.Value, perch.ParrotArea!.Height!.Value), perch.Nuts!.Value, () => { Game1.MasterPlayer.mailReceived.Add(perch.ID); SpentWalnutsForPUPs += (int)perch.Nuts; }, () => false, perchName, perch.Condition ?? ""));
                }
                else
                {
                    Game1.delayedActions.Add(new DelayedAction(10, () => mf.ExecutePerch(perch)));
                }
                if (MainPatches.CustomPerches.Any(storedPerch => storedPerch.ID == perch.ID))
                {
                    Monitor.Log($"Multiple ParrotUpgradePerches with this ID: '{perch.ID}' have been detected. If you are a player, this will cause game breaking errors. You should have at least two mods installed that add those Parrots that sit on a stick. Please write a bug report to either one of those. If you are a modder, DO NOT use the same ID twice. If you didn't do that, you must have another mod installed where someone already uses that ID. Please just use a different ID then (Are you using the {{ModID}} token?)", LogLevel.Error);
                    continue;
                }

                MainPatches.CustomPerches.Add(perch);
            }
            foreach (var entry in json.Settings.SpentWalnuts ?? [])
            {
                string? mailFlag = entry.Key;
                if (string.IsNullOrWhiteSpace(mailFlag))
                {
                    Monitor.Log("You added an empty MailFlag in the SpentWalnuts field!", LogLevel.Error);
                    continue;
                }
                if (MainPatches.CustomPerches.Any(perch => perch.ID == mailFlag))
                {
                    Monitor.Log($"You added the MailFlag '{mailFlag}' in the SpentWalnuts field, but this MailFlag is already one of your ParrotUpgradePerches. They are being automatically added, so this entry will be ignored", LogLevel.Warn);
                    continue;                                    
                }
                if (entry.Value < 0)
                {
                    Monitor.Log("Please do not set a negative amount of Walnuts for your MailFlag in the SpentWalnuts field!", LogLevel.Error);
                    continue;
                }
                MainPatches.QiShopFlags.Add(new KeyValuePair<string, int>(mailFlag, entry.Value));
            }
            foreach (string location in excludedMapsFromSeasonalFeatures)
            {
                if (Game1.getLocationFromName(location) == null)
                {
                    Monitor.Log($"If you are a player, ignore this. You added the location {location} in the disableSeasonalFeaturesForMaps that is not an existing location!", LogLevel.Warn);
                }
            }
            //if (walnutGroupCount != walnutGroupCountForTranspiler)
            //{
            //    Monitor.Log("It seems like you changed the amount of Walnutgroups while the game is loaded. This means that the in-Game command '/recountNuts' will be wrong and the hints at the Parrot in the Islandhut won't be correct. This will be fixed after restarting the game", LogLevel.Warn);
            //}
        }

        private void CalculateCap()
        {
            additionalGoldenWalnuts = 0;
            GoldenWalnutCap = IslandLocation.TOTAL_WALNUTS;
            foreach (var walnut in MainPatches.CustomWalnuts)
            {
                additionalGoldenWalnuts += walnut.Count ?? 1;
            }
            GoldenWalnutCap = IslandLocation.TOTAL_WALNUTS + additionalGoldenWalnuts;
        }

        private void AddQiShopConditions()
        {
            foreach (var entry in Game1.content.Load<Dictionary<string, ShopData>>("Data/Shops")["QiGemShop"].Items)
            {
                if (entry.TradeItemId == "(O)73" && entry.ItemId == "(O)858")
                {
                    var conditionsToAdd = new List<string>();
                    conditionsToAdd.AddRange(MainPatches.CustomPerches.Select(perch => $"PLAYER_HAS_MAIL Host {perch.ID}"));
                    conditionsToAdd.AddRange(MainPatches.QiShopFlags.Select(id => $"PLAYER_HAS_MAIL Host {id.Key}"));
                    var vanillaConditions = entry.Condition.Split(", ").ToHashSet();
                    foreach (string condition in conditionsToAdd)
                    {
                        if (!vanillaConditions.Contains(condition)) //for some weird reason, my changes stay in the file when I load a save in english, then exit and switch the languages to any other languages and load into a save again. I do not understand this at all, why my changes stay only under those EXTREMELY specific circumstances, but yeah, this check fixes those super rare double entries XD
                        {
                            entry.Condition += ", " + condition;
                        }
                    }
                    break;
                }
            }
        }

        public void RemoveOldBushes()
        {
            int deletedCount = 0;
            HashSet<string> customBushes = [];
            foreach (var walnut in MainPatches.CustomWalnuts)
            {
                if (walnut.Type!.Equals("Bush", StringComparison.OrdinalIgnoreCase))
                {
                    customBushes.Add(walnut.ID!);
                }
            }
            foreach (GameLocation l in Game1.locations)
            {
                for (int i = l.largeTerrainFeatures.Count - 1; i >= 0; i--)
                {
                    if (l.largeTerrainFeatures[i] is Bush bush && bush.size.Value == 4 && bush.modData.TryGetValue("ResoNight.GoldenWalnutFramework/CustomID", out var customID))
                    {
                        if (!customBushes.Contains(customID))
                        {
                            l.largeTerrainFeatures.RemoveAt(i);
                            deletedCount++;
                        }
                    }
                }
            }
            if (deletedCount > 0)
            {
                if (deletedCount == 1)
                {
                    Monitor.Log("Removed 1 bush", LogLevel.Info);
                }
                else
                {
                    Monitor.Log($"Removed {deletedCount} bushes", LogLevel.Info);
                }
            }
        }

        public void CheckForInvalidPlacedBushes()
        {
            foreach (var l in Game1.locations)
            {
                foreach (var ltf in l.largeTerrainFeatures)
                {
                    if (ltf is Bush bush && bush.size.Value == 4)
                    {
                        if (!bush.modData.ContainsKey("ResoNight.GoldenWalnutFramework/CustomID"))
                        {
                            if (!vanillaBushes.Any(vanillaBush => vanillaBush.Key == bush.Location.Name && vanillaBush.Value == bush.Tile))
                            {
                                Monitor.Log($"A Bush has been detected that is not a vanilla Bush and that has not been placed using the GoldenWalnutFramework. The Bush is at {bush.Location.Name} with X: {bush.Tile.X} and Y: {bush.Tile.Y}. If you are a player, be aware that this will cause the overall walnutcount to be broken. However, this shouldn't be a problem for you, because you most likely will just have more to collect than normally available. If you are a modder, please DO NOT add Bushes using the Bush tile on the paths.png TileSheet. Please let the GoldenWalnutFramework place the bush by adding a Walnut with Type Bush!", LogLevel.Warn);
                            }
                        }
                    }
                }
            }
        }

        public void GiveUnobtainableWalnuts()
        {
            bool gaveAtLeastOne = false;
            foreach (string id in unobtainableWalnuts)
            {
                if (!Game1.player.team.collectedNutTracker.Contains(id))
                {
                    gaveAtLeastOne = true;
                    Game1.netWorldState.Value.GoldenWalnuts++;
                    Game1.player.team.collectedNutTracker.Add(id);
                }
            }
            if (gaveAtLeastOne)
            {
                Game1.addHUDMessage(new HUDMessage("You have received all the Golden Walnuts that became unobtainable, due to multiple mods adding a Walnut Bush at the same coordinates.", 3, true));
            }
        }

        private void GameLoop_DayStarted(object? sender, DayStartedEventArgs e)
        {
            if (Game1.Date.DayOfMonth == 1)
            {
                Helper.GameContent.InvalidateCache("TileSheets/bushes");
                Helper.GameContent.InvalidateCache("TerrainFeatures/tree_palm");
                foreach (var Perch in MainPatches.CustomPerches)
                {
                    if (Game1.MasterPlayer.mailReceived.Contains(Perch.ID))
                    {
                        Game1.delayedActions.Add(new DelayedAction(10, () => mf!.ExecutePerch(Perch)));
                    }
                }
            }
            Game1.delayedActions.Add(new DelayedAction(10, () => dayIsResetting = false)); //despawning monsters seems to happen in the first tick AFTER the day has started
        }

        private void GameLoop_DayEnding(object? sender, DayEndingEventArgs e)
        {
            customHintForToday = null;
            dayIsResetting = true;
        }

        private void World_TerrainFeatureListChanged(object? sender, TerrainFeatureListChangedEventArgs e)
        {
            if (!Game1.IsMasterGame) { return; }
            if (!e.Added.Any()) { return; }
            if (!Game1.getAllFarmers().Any(farmer => farmer.currentLocation == e.Location)) { return; }
            foreach (var action in e.Added)
            {
                var tile = action.Key;
                if (action.Value is HoeDirt)
                {
                    foreach (var walnut in MainPatches.CustomWalnuts)
                    {
                        if (!walnut.Type!.Equals("Buried", StringComparison.OrdinalIgnoreCase)) { continue; }
                        if (!walnut.Location!.Equals(e.Location.Name, StringComparison.OrdinalIgnoreCase)) { continue; }
                        var nuttracker = Game1.player.team.collectedNutTracker;
                        if (nuttracker.Contains(walnut.ID) || nuttracker.Contains($"{walnut.ID}_{walnut.Count}")) { continue; }
                        if (!GameStateQuery.CheckConditions(walnut.Conditions)) { continue; }
                        if (tile.X == walnut.X && tile.Y == walnut.Y)
                        {
                            if (walnut.Count > 1)
                            {
                                for (int i = 1; i <= walnut.Count; i++)
                                {
                                    Game1.createItemDebris(ItemRegistry.Create("(O)73"), tile * 64f, Game1.random.Next(4), e.Location);
                                    nuttracker.Add($"{walnut.ID}_{i}");
                                }
                            }
                            else
                            {
                                Game1.createItemDebris(ItemRegistry.Create("(O)73"), tile * 64f, Game1.random.Next(4), e.Location);
                                nuttracker.Add(walnut.ID);
                            }
                        }
                    }
                }
            }
        }

        private void World_ObjectListChanged(object? sender, ObjectListChangedEventArgs e)
        {
            if (!Game1.IsMasterGame) { return; }
            if (!e.Removed.Any()) { return; }
            if (!Game1.getAllFarmers().Any(farmer => farmer.currentLocation == e.Location)) { return; }

            foreach (var removedObj in e.Removed)
            {
                var objTile = removedObj.Key;
                if (removedObj.Value.Name != "Stone") { continue; }
                foreach (var walnut in MainPatches.CustomWalnuts)
                {
                    if (!walnut.Type!.Equals("Stone", StringComparison.OrdinalIgnoreCase)) { continue; }
                    if (!walnut.Location!.Equals(e.Location.Name, StringComparison.OrdinalIgnoreCase)) { continue; }
                    if (walnut.StoneTypes != null && !walnut.StoneTypes.Any(type => type!.Equals(removedObj.Value.ItemId, StringComparison.OrdinalIgnoreCase))) { continue; }
                    var nuttracker = Game1.player.team.collectedNutTracker;
                    if (nuttracker.Contains(walnut.ID) || nuttracker.Contains($"{walnut.ID}_{walnut.Count}")) { continue; }
                    if (!GameStateQuery.CheckConditions(walnut.Conditions)) { continue; }
                    bool onCoordinates = objTile.X == walnut.X && objTile.Y == walnut.Y;
                    bool inArea = walnut.Areas?.Any(tile => objTile.X >= tile.X && objTile.X < tile.X + (tile.Width ?? 1) && objTile.Y >= tile.Y && objTile.Y < tile.Y + (tile.Height ?? 1)) == true;
                    if (onCoordinates || inArea || (walnut.X == null && walnut.Areas == null))
                    {
                        if (Game1.random.NextDouble() <= (walnut.Chance ?? 1))
                        {
                            if (walnut.Count > 1)
                            {
                                bool firstTime = true;
                                if (nuttracker.Contains($"{walnut.ID}_1"))
                                {
                                    firstTime = false;
                                }

                                int dropCount = 1;
                                if (!string.IsNullOrWhiteSpace(walnut.DropAtOnce))
                                {
                                    string[] borders = walnut.DropAtOnce.Split('/');
                                    dropCount = Game1.random.Next(int.Parse(borders[0]), int.Parse(borders.Length > 1 ? borders[1] : borders[0]) + 1);
                                }
                                for (int j = 1; j <= dropCount; j++)
                                {
                                    Game1.createItemDebris(ItemRegistry.Create("(O)73"), removedObj.Value.TileLocation * 64f, Game1.random.Next(4), e.Location);
                                    for (int i = 1; i <= walnut.Count; i++)
                                    {
                                        if (!nuttracker.Contains($"{walnut.ID}_{i}"))
                                        {
                                            nuttracker.Add($"{walnut.ID}_{i}");
                                            break;
                                        }
                                    }
                                    
                                    if (nuttracker.Contains($"{walnut.ID}_{walnut.Count}"))
                                    {
                                        if (!firstTime)
                                        {
                                            playJingle = true;
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Game1.createItemDebris(ItemRegistry.Create("(O)73"), removedObj.Value.TileLocation * 64f, Game1.random.Next(4), e.Location);
                                nuttracker.Add(walnut.ID);
                            }
                        }
                    }
                }
            }
        }

        private void World_NpcListChanged(object? sender, NpcListChangedEventArgs e)
        {
            if (!Game1.IsMasterGame) { return; }
            if (dayIsResetting) { return; }
            if (!e.Removed.Any()) { return; }
            if (!Game1.getAllFarmers().Any(farmer => farmer.currentLocation == e.Location)) { return; }

            foreach (var removedNPC in e.Removed)
            {
                if (!removedNPC.IsMonster) { continue; }
                var npcTile = removedNPC.Tile;
                var nuttracker = Game1.MasterPlayer.team.collectedNutTracker;
                foreach (var walnut in MainPatches.CustomWalnuts)
                {
                    if (walnut.Type?.ToLower() != "monsterloot") { continue; }
                    if (!removedNPC.currentLocation.Name.Equals(walnut.Location, StringComparison.OrdinalIgnoreCase)) { continue; }
                    if (nuttracker.Contains(walnut.ID) || nuttracker.Contains($"{walnut.ID}_{walnut.Count}")) { continue; }
                    if (walnut.MonsterTypes != null && !walnut.MonsterTypes.Any(m => m.Equals(removedNPC.Name, StringComparison.OrdinalIgnoreCase) || m.Replace('\\', '/').Equals(removedNPC.Sprite.textureName.Value.Replace('\\', '/'), StringComparison.OrdinalIgnoreCase))) { continue; }
                    if (!GameStateQuery.CheckConditions(walnut.Conditions)) { continue; }
                    bool onCoordinates = npcTile.X == walnut.X && npcTile.Y == walnut.Y;
                    bool inArea = walnut.Areas?.Any(tile => npcTile.X >= tile.X && npcTile.X < tile.X + (tile.Width ?? 1) && npcTile.Y >= tile.Y && npcTile.Y < tile.Y + (tile.Height ?? 1)) == true;
                    if (onCoordinates || inArea || (walnut.X == null && walnut.Areas == null))
                    {
                        if (Game1.random.NextDouble() <= (walnut.Chance ?? 1))
                        {
                            if (walnut.Count > 1)
                            {
                                bool firstTime = true;
                                if (nuttracker.Contains($"{walnut.ID}_1"))
                                {
                                    firstTime = false;
                                }

                                int dropCount = 1;
                                if (!string.IsNullOrWhiteSpace(walnut.DropAtOnce))
                                {
                                    string[] borders = walnut.DropAtOnce.Split('/');
                                    dropCount = Game1.random.Next(int.Parse(borders[0]), int.Parse(borders.Length > 1 ? borders[1] : borders[0]) + 1);
                                }
                                for (int j = 1; j <= dropCount; j++)
                                {
                                    Game1.createItemDebris(ItemRegistry.Create("(O)73"), npcTile * 64f, Game1.random.Next(4), e.Location);
                                    for (int i = 1; i <= walnut.Count; i++)
                                    {
                                        if (!nuttracker.Contains($"{walnut.ID}_{i}"))
                                        {
                                            nuttracker.Add($"{walnut.ID}_{i}");
                                            break;
                                        }
                                    }
                                    if (nuttracker.Contains($"{walnut.ID}_{walnut.Count}"))
                                    {
                                        if (!firstTime)
                                        {
                                            playJingle = true;
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Game1.createItemDebris(ItemRegistry.Create("(O)73"), npcTile * 64f, Game1.random.Next(4), e.Location);
                                nuttracker.Add(walnut.ID);
                            }
                        }
                    }
                }
            }
        }

        private void Player_Warped(object? sender, WarpedEventArgs e)
        {
            if (e.NewLocation.InIslandContext() || e.NewLocation.InDesertContext() || excludedMapsFromSeasonalFeatures.Contains(e.NewLocation.Name))
            {
                if (seasonalTerrainFeatures != false)
                {
                    seasonalTerrainFeatures = false;
                    Helper.GameContent.InvalidateCache("TileSheets/bushes");
                    Helper.GameContent.InvalidateCache("TerrainFeatures/tree_palm");
                }

            }
            else
            {
                if (seasonalTerrainFeatures != true)
                {
                    seasonalTerrainFeatures = true;
                    Helper.GameContent.InvalidateCache("TileSheets/bushes");
                    Helper.GameContent.InvalidateCache("TerrainFeatures/tree_palm");
                }
            }
            foreach (var walnut in MainPatches.CustomWalnuts)
            {
                if (!walnut.Type!.Equals("Bush", StringComparison.OrdinalIgnoreCase)) { continue; }
                if (!walnut.Location!.Equals(e.NewLocation.Name, StringComparison.OrdinalIgnoreCase)) { continue; }
                mf!.SpawnBushes(walnut);
            }
        }

        private void Content_AssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Mods/GoldenWalnutFramework/Data"))
            {
                e.LoadFrom(
                    () => new BaseJSON
                        {
                            GoldenWalnuts = new(),
                            ParrotUpgradePerches = new(),
                            Settings = new()
                        },
                    AssetLoadPriority.Exclusive
                );
            }
            else if (e.Name.IsEquivalentTo("TileSheets/bushes"))
            {
                if (seasonalTerrainFeatures == true)
                {
                    e.Edit(asset =>
                    {
                        var texture = asset.AsImage();
                        var targetArea = new Microsoft.Xna.Framework.Rectangle(0, 320, 64, 32);
                        if (Game1.currentSeason.Equals("spring"))
                        {
                            texture.PatchImage(Helper.ModContent.Load<Texture2D>("assets/TerrainFeatures/spring_bushes.png"), new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 32), targetArea);
                        }
                        else if (Game1.currentSeason.Equals("summer"))
                        {
                            texture.PatchImage(Helper.ModContent.Load<Texture2D>("assets/TerrainFeatures/summer_bushes.png"), new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 32), targetArea);
                        }
                        else if (Game1.currentSeason.Equals("fall"))
                        {
                            texture.PatchImage(Helper.ModContent.Load<Texture2D>("assets/TerrainFeatures/fall_bushes.png"), new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 32), targetArea);
                        }
                        else if (Game1.currentSeason.Equals("winter"))
                        {
                            texture.PatchImage(Helper.ModContent.Load<Texture2D>("assets/TerrainFeatures/winter_bushes.png"), new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 32), targetArea);
                        }
                    });
                }
            }
            else if (e.Name.IsEquivalentTo("TerrainFeatures/tree_palm"))
            {
                if (seasonalTerrainFeatures == true)
                {
                    e.LoadFromModFile<Texture2D>($"assets/TerrainFeatures/{Game1.currentSeason}_tree_palm2.png", AssetLoadPriority.Low);
                }
            }
            else if (e.Name.IsEquivalentTo("ResoNight.GoldenWalnutFramework/ParrotSticks"))
            {
                e.LoadFromModFile<Texture2D>("assets/ParrotSticks/ParrotSticks.png", AssetLoadPriority.Low);
            }
            else if (e.Name.IsEquivalentTo($"ResoNight.GoldenWalnutFramework/{Game1.currentSeason}_ParrotSticks"))
            {
                e.LoadFromModFile<Texture2D>($"assets/ParrotSticks/{Game1.currentSeason}_ParrotSticks.png", AssetLoadPriority.Low);
            }
        }


        public void HandleShowNutHintsForTranspiler(IslandHut __instance, KeyValuePair<string, int>? valid_hint)
        {
            if (valid_hint.HasValue)
            {
                string hint = valid_hint.Value.Key;
                if (valid_hint.Value.Value == 1 && hint.Contains("{0}"))
                {
                    for (int i = 0; i < MainPatches.WalnutGroups.Count; i++)
                    {
                        if (MainPatches.WalnutGroups[i].Key == hint)
                        {
                            if (MainPatches.WalnutGroupsSingular[i] != "NoSingularAssigned")
                            {
                                hint = MainPatches.WalnutGroupsSingular[i];
                            }
                            break;
                        }
                    }
                }

                if (hint != null)
                {
                    __instance.hintDialogues.Add(Game1.content.LoadString("Strings\\Locations:NutHint_Squawk"));
                    if (hint.StartsWith("Strings\\Locations"))
                    {
                        __instance.hintDialogues.Add(Game1.content.LoadString(hint, valid_hint.Value.Value));
                    }
                    else
                    {
                        __instance.hintDialogues.Add(string.Format(hint, valid_hint.Value.Value));
                    }
                }
            }
            

            if (MainPatches.SeparateWalnutGroups != null)
            {
                List<KeyValuePair<string, int>> valid_customHints = [];
                KeyValuePair<string, int> valid_customHint = new();
                var groups = MainPatches.SeparateWalnutGroups;
                for (int i = 0; i < groups.Count; i++)
                {
                    int missingNuts = 0;
                    if (MainPatches.MissingTheseNuts(ref missingNuts, i, true, groups[i].Value))
                    {
                        valid_customHints.Add(new KeyValuePair<string, int>(groups[i].Key, missingNuts));
                    }
                }
                if (valid_customHints.Count == 0) { return; }
                if (customHintForToday == null)
                {
                    Random r = Utility.CreateRandom(Game1.uniqueIDForThisGame, Game1.Date.TotalDays * 642.0);
                    valid_customHint = valid_customHints[r.Next(valid_customHints.Count)];
                    customHintForToday = valid_customHint.Key;
                }
                else
                {
                    valid_customHint = valid_customHints.FirstOrDefault(h => h.Key == customHintForToday);
                }



                if (valid_customHint.Key != null)
                {
                    string customHint = valid_customHint.Key;
                    if (valid_customHint.Value == 1 && customHint.Contains("{0}"))
                    {
                        for (int i = 0; i < MainPatches.SeparateWalnutGroups.Count; i++)
                        {
                            if (MainPatches.SeparateWalnutGroups[i].Key == customHint)
                            {
                                if (MainPatches.SeparateWalnutGroupsSingular[i] != "NoSingularAssigned")
                                {
                                    customHint = MainPatches.SeparateWalnutGroupsSingular[i];
                                }
                                break;
                            }
                        }
                    }
                    __instance.hintShowTime = 2f;
                    __instance.hintDialogues.Add("Custom Walnuts");
                    __instance.hintDialogues.Add(string.Format(customHint, valid_customHint.Value));
                }
                else
                {
                    Random r = Utility.CreateRandom(Game1.uniqueIDForThisGame, Game1.Date.TotalDays * 642.0);
                    valid_customHint = valid_customHints[r.Next(valid_customHints.Count)];
                    string customHint = valid_customHint.Key;
                    customHintForToday = customHint;
                    if (valid_customHint.Value == 1)
                    {
                        for (int i = 0; i < MainPatches.SeparateWalnutGroups.Count; i++)
                        {
                            if (MainPatches.SeparateWalnutGroups[i].Key == customHint)
                            {
                                customHint = MainPatches.SeparateWalnutGroupsSingular[i];
                                break;
                            }
                        }
                    }
                    __instance.hintShowTime = 2f;
                    __instance.hintDialogues.Add("Custom Walnuts");
                    __instance.hintDialogues.Add(string.Format(customHint, valid_customHint.Value));
                }
            }
        }
    }

    public class MainFunctions
    {
        private static ModEntry m => ModEntry.Instance!;

        public void MailReceived_OnValueAdded(string mailFlag)
        {
            foreach (var perch in MainPatches.CustomPerches)
            {
                if (perch.ID == mailFlag)
                {
                    ExecutePerch(perch);
                    break;
                }
            }
        }

        public void RegisterJSONSettings(Settings settings)
        {
            if (settings.DisableWalnutCap == true)
            {
                m.disableWalnutCap = true;
            }
            if (settings.DisableSeasonalFeaturesForMaps != null)
            {
                foreach (string map in settings.DisableSeasonalFeaturesForMaps)
                {
                    m.excludedMapsFromSeasonalFeatures.Add(map!);
                }
            }
        }

        public bool CheckJSONWalnuts(string groupKey, CustomWalnut walnut)
        {
            var id = walnut.ID;
            var type = walnut.Type?.ToLower();
            var l = walnut.Location;
            var x = walnut.X;
            var y = walnut.Y;
            var count = walnut.Count;
            var dropatonce = walnut.DropAtOnce;
            var chance = walnut.Chance;
            var areas = walnut.Areas;
            var monstertypes = walnut.MonsterTypes;
            var stonetypes = walnut.StoneTypes;
            var conditions = walnut.Conditions;


            //double check the entries
            if (string.IsNullOrWhiteSpace(id))
            {
                m.Monitor.Log($"Group '{groupKey}' has a Walnut that does not have an ID assigned to it!", LogLevel.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(type))
            {
                m.Monitor.Log($"Group '{groupKey}', Walnut '{id}':  A Walnut is missing a Type. Existing types are: Bush, Buried, Fishing, Stone, MonsterLoot, Custom", LogLevel.Error);
                return false;
            }
            if (type is not "bush" and not "buried" and not "fishing" and not "stone" and not "monsterloot" and not "custom")
            {
                m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': The Walnut Type '{type}' does not match an existing Type. Existing types are: Bush, Buried, Fishing, Stone, MonsterLoot, Custom", LogLevel.Error);
                return false;
            }

            if (type == "bush")
            {                if (string.IsNullOrWhiteSpace(l) || x == null || y == null)
                {
                    m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut with type Bush needs the following entries: ID, Type, Location, X, Y", LogLevel.Error);
                    return false;
                }
                if (new object?[] { count, dropatonce, chance, areas, monstertypes, stonetypes }.Any(x => x != null))
                {
                    m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut with Type Bush cannot have any of the following entries: Count, DropAtOnce, Chance, Areas, MonsterTypes, StoneTypes. Any of those entries will be ignored", LogLevel.Warn);
                    count = null;
                    dropatonce = null;
                    chance = null;
                    areas = null;
                    monstertypes = stonetypes = null;
                }
            }
            if (type == "buried")
            {
                if (string.IsNullOrWhiteSpace(l) || x == null || y == null)
                {
                    m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut with Type Buried needs the following entries: ID, Type, Location, X, Y", LogLevel.Error);
                    return false;
                }
                if (new object?[] { chance, areas, monstertypes, stonetypes }.Any(x => x != null))
                {
                    m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut with Type Buried cannot have any of the following entries: Chance, Areas, MonsterTypes, StoneTypes. Any of those entries will be ignored", LogLevel.Warn);
                    chance = null;
                    areas = null;
                    monstertypes = stonetypes = null;
                }
                if (dropatonce != null)
                {
                    m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut with Type Buried has a DropAtOnce field assigned to it. Buried walnuts will always drop their count at once, so this field will not have any effect", LogLevel.Warn);
                    dropatonce = null;
                }
            }
            else if (type == "fishing")
            {
                if (string.IsNullOrWhiteSpace(l))
                {
                    m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut with Type Fishing needs the following entries: ID, Type, Location", LogLevel.Error);
                    return false;
                }
                if (new object?[] { dropatonce, monstertypes, stonetypes }.Any(x => x != null))
                {
                    m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut with Type Fishing cannot have any of the following entries: DropAtOnce, MonsterTypes, StoneTypes. Any of those entries will be ignored", LogLevel.Warn);
                    dropatonce = null;
                    monstertypes = stonetypes = null;
                }
            }
            else if (type == "stone")
            {
                if (string.IsNullOrWhiteSpace(l))
                {
                    m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut with Type Stone needs the following entries: ID, Type, Location", LogLevel.Error);
                    return false;
                }
                if (monstertypes != null)
                {
                    m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut with Type Stone cannot have a field for MonsterTypes. Its entry will be ignored", LogLevel.Warn);
                    monstertypes = null;
                }
            }
            else if (type == "monsterloot")
            {
                if (string.IsNullOrWhiteSpace(walnut.Location))
                {
                    m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut with type MonsterLoot needs the following entries: ID, Type, Location", LogLevel.Error);
                    return false;
                }
                if (stonetypes != null)
                {
                    m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut with Type MonsterLoot cannot have the field StoneTypes and its entry will be ignored", LogLevel.Warn);
                    stonetypes = null;
                }
            }
            else if (type == "custom")
            {
                if (new object?[] { l, x, y, areas, chance, monstertypes, conditions }.Any(x => x != null))
                {
                    if (conditions != null)
                    {
                        m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut set to Custom cannot have a conditions field, because you have to make the whole dropping logic yourself. You have to add the conditions in your own code for the walnut", LogLevel.Warn);
                    }
                    else
                    {
                        m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut set to Custom can only have the Type, ID and Count assigned to them. Every other entry will be ignored", LogLevel.Warn);
                    }
                    x = y = null;
                    l = null;
                    chance = null;
                    dropatonce = null;
                    areas = null;
                    monstertypes = null;
                    stonetypes = null;
                    conditions = null;
                }
            }

            //check each entry
            if ((x == null && y != null) || (x != null && y == null))
            {
                m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': If you assign either the X or Y field, you must also assign the other field. Both or none please", LogLevel.Error);
                return false;
            }
            if (count < 1)
            {
                m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut Count must be at least 1! (for count 1, you don't have to add this field at all)", LogLevel.Error);
                return false;
            }
            if (!string.IsNullOrWhiteSpace(dropatonce))
            {
                string[] borders = dropatonce.Split('/');
                if (borders.Length > 2)
                {
                    m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut with the field DropAtOnce has more than one '/'. Please only put 1 number or 2 numbers with a / between them in this field.", LogLevel.Error);
                    return false;
                }
                if (int.Parse(borders.Length > 1 ? borders[1] : borders[0]) < int.Parse(borders[0]))
                {
                    m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut with the field DropAtOnce has the second number lower than the first number. The first number is the lower border and the second number is the upper border.", LogLevel.Error);
                    return false;
                }
                if (int.Parse(borders.Length > 1 ? borders[1] : borders[0]) > count)
                {
                    m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut with the field DropAtOnce has an upper limit (number after '/') higher than the walnut's count. Note that the field for Count is always superior and there will never be dropped more walnuts than the Count.", LogLevel.Warn);
                }
            }
            if (chance is < 0 or > 1)
            {
                m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': The Chance for a walnut must be between 0 and 1. A chance like 25% must therefore be written as 0.25", LogLevel.Error);
                return false;
            }
            if (areas != null)
            {
                foreach (var area in areas)
                {
                    if (area.X == null || area.Y == null)
                    {
                        m.Monitor.Log($"Group '{groupKey}', Walnut '{id}': A Walnut with assigned ExtraTiles must have at least an entry for X and Y for each tile!", LogLevel.Error);
                        return false;
                    }
                }
            }
            return true;
        }

        public void SpawnBushes(CustomWalnut walnut)
        {
            GameLocation l = Game1.getLocationFromName(walnut.Location);
            if (!GameStateQuery.CheckConditions(walnut.Conditions)) { return; }
            Vector2 tilePos = new((int)walnut.X!, (int)walnut.Y!);
            foreach (var ltf in l.largeTerrainFeatures)
            {
                if (ltf is Bush bush && bush.size.Value == 4 && bush.netTilePosition.Equals(tilePos))
                {
                    if (bush.modData.TryGetValue("ResoNight.GoldenWalnutFramework/CustomID", out var customID))
                    {
                        if (customID != walnut.ID)
                        {
                            m.unobtainableWalnuts.Add(walnut.ID!);
                        }
                    }
                    else
                    {
                        foreach (var vanillaBush in m.vanillaBushes)
                        {
                            if (vanillaBush.Key == bush.Location.Name && vanillaBush.Value == bush.Tile)
                            {
                                m.Monitor.Log("You placed a bush where a vanilla Bush already exists.", LogLevel.Error);
                                m.unobtainableWalnuts.Add(walnut.ID!);
                                break;
                            }
                        }
                    }
                    return;
                }
            }
            Bush walnutBush = new(tilePos, 4, l);
            walnutBush.modData["ResoNight.GoldenWalnutFramework/CustomID"] = walnut.ID;
            if (Game1.player.team.collectedNutTracker.Contains(walnut.ID))
            {
                walnutBush.tileSheetOffset.Value = 0;
                walnutBush.setUpSourceRect();
            }
            l.largeTerrainFeatures.Add(walnutBush);
        }


        public bool CheckJSONPerches(CustomParrotUpgradePerch perch)
        {
            if (string.IsNullOrWhiteSpace(perch.ID))
            {
                m.Monitor.Log("A ParrotUpgradePerch has no assigned ID", LogLevel.Error);
                return false;
            }
            string perchID = perch.ID;
            if (perch.FromArea != null && perch.FromFile != null && perch.ToArea == null)
            {
                perch.ToArea = perch.ParrotArea;
            }
            if (string.IsNullOrWhiteSpace(perch.Location) || perch.Nuts == null || perch.ParrotTile == null || perch.ParrotArea == null)
            {
                m.Monitor.Log($"ParrotUpgradePerch: '{perchID}' is missing one of the necessary entries: ID, Location, Nuts, ParrotTile, ParrotArea", LogLevel.Error);
                return false;
            }
            if (perch.Nuts < 1)
            {
                m.Monitor.Log($"ParrotUpgradePerch: '{perchID}', Nuts field must be above Zero!", LogLevel.Error);
                return false;
            }
            if (perch.ParrotTile.X == null || perch.ParrotTile.Y == null)
            {
                m.Monitor.Log($"ParrotUpgradePerch: '{perchID}' must have an X and Y coordinate for the ParrotTile!", LogLevel.Error);
                return false;
            }
            if (perch.ParrotArea.X == null || perch.ParrotArea.Y == null || perch.ParrotArea.Width == null || perch.ParrotArea.Height == null)
            {
                m.Monitor.Log($"ParrotUpgradePerch: '{perchID}' The ParrotArea must have X, Y, Width and Height assigned!", LogLevel.Error);
                return false;
            }
            if (perch.FromArea != null || perch.ToArea != null || perch.FromFile != null)
            {
                if (!(perch.FromArea != null && perch.ToArea != null && perch.FromFile != null))
                {
                    m.Monitor.Log($"ParrotUpgradePerch: '{perchID}' For a Map Overriding ParrotUpgradePerch, it must have the fields FromArea and FromFile assigned! (ToArea will be the ParrotArea if omitted)", LogLevel.Error);
                    return false;
                }
                else if (perch.FromArea.X == null || perch.FromArea.Y == null || perch.FromArea.Width == null || perch.FromArea.Height == null)
                {
                    m.Monitor.Log($"ParrotUpgradePerch: '{perchID}' The FromArea must have X, Y, Width and Height assigned!", LogLevel.Error);
                    return false;
                }
                else if (perch.ToArea.X == null || perch.ToArea.Y == null || perch.ToArea.Width == null || perch.ToArea.Height == null)
                {
                    m.Monitor.Log($"ParrotUpgradePerch: '{perchID}' The ToArea must have X, Y, Width and Height assigned!", LogLevel.Error);
                    return false;
                }
                else if (perch.FromArea.Width != perch.ToArea.Width || perch.FromArea.Height != perch.ToArea.Height)
                {
                    m.Monitor.Log($"ParrotUpgradePerch: '{perchID}' The FromArea and ToArea Width and Height must be the same! (If ToArea is omitted, FromArea and ParrotArea must have the same!)", LogLevel.Error);
                    return false;
                }
            }
            if (perch.DestroyAreas != null)
            {
                foreach (var area in perch.DestroyAreas)
                {
                    if (area.X == null || area.Y == null || area.Layers == null)
                    {
                        m.Monitor.Log($"ParrotUpgradePerch: '{perchID}' Each Area in the DestroyAreas field must have those entries: X, Y, Layers. (Width and Height are optional but recommended)", LogLevel.Error);
                        return false;
                    }
                    foreach (string Layer in area.Layers)
                    {
                        string layer = Layer.ToLower();
                        if (!layer.StartsWith("back") && !layer.StartsWith("buildings") && !layer.StartsWith("paths") && !layer.StartsWith("front") && !layer.StartsWith("alwaysfront"))
                        {
                            m.Monitor.Log($"ParrotUpgradePerch: '{perchID}' An Area in the DestroyAreas field has the layer: '{layer}', which does not exist. Possible layers are: Back, Buildings, Paths, Front, AlwaysFront. May be followed by a number like Buildings-1, Back2, AlwaysFront-4 etc.", LogLevel.Error);
                            return false;
                        }
                    } 
                }
            }
            return true;
        }

        

        public void ExecutePerch(CustomParrotUpgradePerch perch)
        {
            GameLocation location = Game1.getLocationFromName(perch.Location);

            if (perch.DestroyAreas != null)
            {
                foreach (var area in perch.DestroyAreas)
                {
                    for (int X = (int)area.X!; X < area.X + (area.Width ?? 1); X++)
                    {
                        for (int Y = (int)area.Y!; Y < area.Y + (area.Height ?? 1); Y++)
                        {
                            foreach (string Layer in area.Layers!)
                            {
                                string layer = Layer.Replace(" ", "");
                                if (layer.ToLower().StartsWith("alwaysfront"))
                                {
                                    layer = $"AlwaysFront{layer[11..]}";
                                }
                                else
                                {
                                    layer = char.ToUpper(layer[0]) + layer[1..].ToLower();
                                }
                                location.removeMapTile(X, Y, layer);
                            }
                        }
                    }
                }
            }

            if (location is Town town) //this is for an extremely special case. Town has its own map Loader and when using reloadMap(), it will override any map changes for some reason. So for the town, I need to retrigger the overrides and I need to update f.e. the water tiles WITHOUT using reloadMap(). I hope that is the only case like this :/
            {
                if (perch.FromFile != null)
                {
                    var appliedMapOverrides = (HashSet<string>)typeof(GameLocation).GetField("_appliedMapOverrides", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(location)!;
                    appliedMapOverrides.Remove(perch.ID!);
                    location.ApplyMapOverride(m.Helper.GameContent.Load<Map>(perch.FromFile), perch.ID, new Microsoft.Xna.Framework.Rectangle(perch.FromArea!.X!.Value, perch.FromArea!.Y!.Value, perch.FromArea!.Width!.Value, perch.FromArea!.Height!.Value), new Microsoft.Xna.Framework.Rectangle(perch.ToArea!.X!.Value, perch.ToArea!.Y!.Value, perch.ToArea!.Width!.Value, perch.ToArea!.Height!.Value));
                }
                reloadTownMap(town);
                return;
            }

            if (perch.FromFile != null)
            {
                location.ApplyMapOverride(m.Helper.GameContent.Load<Map>(perch.FromFile), perch.ID, new Microsoft.Xna.Framework.Rectangle(perch.FromArea!.X!.Value, perch.FromArea!.Y!.Value, perch.FromArea!.Width!.Value, perch.FromArea!.Height!.Value), new Microsoft.Xna.Framework.Rectangle(perch.ToArea!.X!.Value, perch.ToArea!.Y!.Value, perch.ToArea!.Width!.Value, perch.ToArea!.Height!.Value));
            }
            location.reloadMap();
        }

        public void reloadTownMap(Town town)
        {
            town.SortLayers();
            town.updateSeasonalTileSheets(town.Map);
            town.Map.LoadTileSheets(Game1.mapDisplayDevice);

            town.waterTiles = new WaterTiles(town.Map.Layers[0].LayerWidth, town.Map.Layers[0].LayerHeight);
            for (int x = 0; x < town.Map.Layers[0].LayerWidth; x++)
            {
                for (int y = 0; y < town.Map.Layers[0].LayerHeight; y++)
                {
                    string water_property = town.doesTileHaveProperty(x, y, "Water", "Back");
                    if (water_property != null)
                    {
                        if (water_property == "I")
                        {
                            town.waterTiles.waterTiles[x, y] = new WaterTiles.WaterTileData(is_water: true, is_visible: false);
                        }
                        else
                        {
                            town.waterTiles[x, y] = true;
                        }
                    }
                }
            }
            town.critters = [];
            town.loadLights();
        }
    }

    public class HandleCommands
    {
        private static ModEntry m => ModEntry.Instance!;
        public static MainFunctions mf = ModEntry.mf!;

        public string LogCommandError = "GoldenWalnutFramework Commands can only be used after a save is loaded! Except 'ShowStoneTypeIDs'";

        public void ShowAllWalnutIDs(string command, string[] args)
        {
            if (!Context.IsWorldReady) { m.Monitor.Log(LogCommandError, LogLevel.Error); return; }

            m.Monitor.Log("Here are the IDs of all the Walnuts you and other currently installed mods added:", LogLevel.Info);
            foreach (var walnut in MainPatches.CustomWalnuts)
            {
                if (walnut.Count > 1)
                {
                    for (int i = 1; i <= walnut.Count; i++)
                    {
                        m.Monitor.Log($"{walnut.ID}_{i}", LogLevel.Info);
                    }
                }
                else
                {
                    m.Monitor.Log($"{walnut.ID}", LogLevel.Info);
                }
            }
        }

        public void ShowWalnuts(string command, string[] args)
        {
            if (!Context.IsWorldReady) { m.Monitor.Log(LogCommandError, LogLevel.Error); return; }

            if (Game1.player.team.collectedNutTracker.Count > 0)
            {
                m.Monitor.Log("Here are all the Walnuts that the MasterPlayer currently has, including Vanilla Walnuts:", LogLevel.Info);
                foreach (string walnut in Game1.player.team.collectedNutTracker)
                {
                    m.Monitor.Log($"{walnut}", LogLevel.Info);
                }
            }
            else { m.Monitor.Log("Playerteam does not have any walnuts collected!", LogLevel.Info); }
        }

        public void ShowMailFlags(string command, string[] args)
        {
            if (!Context.IsWorldReady) { m.Monitor.Log(LogCommandError, LogLevel.Error); return; }

            m.Monitor.Log("Here are all the MailFlags listed that the MasterPlayer (Host) currently has:", LogLevel.Info);
            foreach (string mailFlag in Game1.MasterPlayer.mailReceived)
            {
                m.Monitor.Log($"{mailFlag}", LogLevel.Info);
            }
        }

        public void ShowStoneTypeIDs(string command, string[] args)
        {
            m.Monitor.Log("Here are all the vanilla StoneNodes and their IDs", LogLevel.Info);
            m.Monitor.Log("------------------------------", LogLevel.Info);
            m.Monitor.Log("Diamond: 2", LogLevel.Info);
            m.Monitor.Log("Ruby: 4", LogLevel.Info);
            m.Monitor.Log("Jade: 6", LogLevel.Info);
            m.Monitor.Log("Amethyst: 8", LogLevel.Info);
            m.Monitor.Log("Topaz: 10", LogLevel.Info);
            m.Monitor.Log("Emerald: 12", LogLevel.Info);
            m.Monitor.Log("Aquamarine: 14", LogLevel.Info);
            m.Monitor.Log("MusselStone: 25", LogLevel.Info);
            m.Monitor.Log("Stone: 32, 34, 36, 38, 40, 42", LogLevel.Info);
            m.Monitor.Log("GemStone: 44", LogLevel.Info);
            m.Monitor.Log("MysticStone: 46", LogLevel.Info);
            m.Monitor.Log("SnowyStone: 48, 50, 52, 54, 56, 58", LogLevel.Info);
            m.Monitor.Log("GeodeStone: 75", LogLevel.Info);
            m.Monitor.Log("FrozenGeodeStone: 76", LogLevel.Info);
            m.Monitor.Log("MagmaGeodeStone: 77", LogLevel.Info);
            m.Monitor.Log("RadioactiveStone: 95", LogLevel.Info);
            m.Monitor.Log("IronOre: 290", LogLevel.Info);
            m.Monitor.Log("StoneOnly: 343, 450", LogLevel.Info);
            m.Monitor.Log("CoalStone: 668, 670", LogLevel.Info);
            m.Monitor.Log("CopperOre: 751", LogLevel.Info);
            m.Monitor.Log("DarkStone: 760, 762", LogLevel.Info);
            m.Monitor.Log("GoldOre: 764", LogLevel.Info);
            m.Monitor.Log("IridiumOre: 765", LogLevel.Info);
            m.Monitor.Log("FossilStone: 816, 817", LogLevel.Info);
            m.Monitor.Log("ClayStone: 818", LogLevel.Info);
            m.Monitor.Log("OmniGeodeStone: 819", LogLevel.Info);
            m.Monitor.Log("CinderShardStone: 843, 844", LogLevel.Info);
            m.Monitor.Log("StoneVolcano: 845, 846, 847", LogLevel.Info);
            m.Monitor.Log("CopperOreVolcano: 849", LogLevel.Info);
            m.Monitor.Log("IronOreVolcano: 850", LogLevel.Info);
            m.Monitor.Log("CalicoEggStone: 'CalicoEggStone_0', 'CalicoEggStone_1', 'CalicoEggStone_2'", LogLevel.Info);
            m.Monitor.Log("GoldOreVolcano: 'VolcanoGoldNode'", LogLevel.Info);
            m.Monitor.Log("CoalStoneVolcano: 'VolcanoCoalNode0', 'VolcanoCoalNode1", LogLevel.Info);
            m.Monitor.Log("BasicCoalStone: 'BasicCoalNode0', 'BasicCoalNode1'", LogLevel.Info);
            m.Monitor.Log("------------------------------", LogLevel.Info);
            m.Monitor.Log("Here some more explanations. All the entries you see here are all the objects in the Objects.json file that have the field 'Name': 'Stone' (except ID 390, which is the Item stone). This automatically turns them into objects that can be destroyed with a pickaxe. There are IDs that are numbers and IDs that are words. The ones with numbers have the same SpriteIndex on the Maps/springobjects file as their numbers. The ones with words as IDs are on the TileSheets/Objects_2 file. GemStones are the Stones that drop one random gem like a Ruby or a Jade, etc. . SnowyStones occur in the ice layer, DarkStones in the magma layer. StoneOnly are the ones that appear on the farm that will always drop a stone. CoalStone are the ones that appear in each layer in the mine and always drop coal and more stone. BasicCoalStone are the ones that have actual coal in their texture and they only appear in quarries. The volcano has a darker version of them though. I hope the rest is clear. Any added Stones from you will not be listed here.", LogLevel.Info);
        }

        public void RemoveMailFlag(string command, string[] args)
        {
            if (!Context.IsWorldReady) { m.Monitor.Log(LogCommandError, LogLevel.Error); return; }

            if (args.Length > 0)
            {
                if (args.Length > 1) { m.Monitor.Log("After the command, only one string can follow! Everything after that will be ignored", LogLevel.Warn); }
                if (Game1.MasterPlayer.mailReceived.Contains(args[0]))
                {
                    Game1.MasterPlayer.mailReceived.Remove(args[0]);
                    m.Monitor.Log("successfully removed Mailflag. Remember to save the day!", LogLevel.Info);
                }
                else
                {
                    m.Monitor.Log($"The MasterPlayer (Host) does not have a MailFlag with the ID: '{args[0]}'! You can use the command ShowMailFlags to look up all the MailFlags that the Host currently has!", LogLevel.Error);
                }
            }
            else
            {
                m.Monitor.Log("Please add the name of the MailFlag after the Command! You can use the command ShowMailFlags to look up all the MailFlags that the Host currently has!", LogLevel.Error);
            }
        }

        public void RemoveWalnut(string command, string[] args)
        {
            if (!Context.IsWorldReady) { m.Monitor.Log(LogCommandError, LogLevel.Error); return; }

            if (args.Length > 0)
            {
                if (args.Length > 1) { m.Monitor.Log("After the command, only one string can follow! Everything after that will be ignored", LogLevel.Warn); }
                if (Game1.player.team.collectedNutTracker.Contains(args[0]))
                {
                    Game1.player.team.collectedNutTracker.Remove(args[0]);
                    m.Monitor.Log("Successfully removed Walnut", LogLevel.Info);
                }
                else
                {
                    m.Monitor.Log($"The Playerteam has not collected the walnut with the ID: '{args[0]}' yet! To look up all the Walnuts that the team has collected, use the command ShowWalnuts!", LogLevel.Error);
                }
            }
            else
            {
                m.Monitor.Log("Please add the ID of the walnut after the Command! To look up all the Walnuts that the team has collected, use the command ShowWalnuts!", LogLevel.Error);
            }
        }

        public bool WalnutGroupGSQ(string[] query, GameStateQueryContext context)
        {
            if (query.Length < 2)
            {
                m.Monitor.Log("For the GameStateQuery COMPLETED_WALNUTGROUP, you must also type a group after it! In this state, it will always just give true!", LogLevel.Warn);
                return true;
            }
            if (m.GameStateQueryWalnutGroups.TryGetValue(query[1], out var walnutIDs))
            {
                return walnutIDs.All(Game1.player.team.collectedNutTracker.Contains);
            }
            else
            {
                m.Monitor.Log($"For the GameStateQuery COMPLETED_WALNUTGROUP, the group '{query[1]}' could not be found and it will therefore always be true!", LogLevel.Warn);
                return true;
            }
        }

        public bool WalnutCountGSQ(string[] query, GameStateQueryContext context)
        {
            return Game1.netWorldState.Value.GoldenWalnutsFound >= m.GoldenWalnutCap;
        }
    }


    internal static class MainPatches
    {
        public static ModEntry m = ModEntry.Instance!;
        public static MainFunctions mf = ModEntry.mf!;

        public static HashSet<string> WalnutLocations = new(StringComparer.OrdinalIgnoreCase);
        public static List<KeyValuePair<string, string[]>> WalnutGroups = [];
        public static List<string> WalnutGroupsSingular = [];
        public static List<string> WalnutGroupsHintConditions = [];
        public static List<KeyValuePair<string, string[]>> SeparateWalnutGroups = [];
        public static List<string> SeparateWalnutGroupsSingular = [];
        public static List<string> SeparateWalnutGroupsHintConditions = [];
        public static List<KeyValuePair<string, int>> QiShopFlags = [];
        public static List<CustomWalnut> CustomWalnuts = [];
        public static List<CustomParrotUpgradePerch> CustomPerches = [];

        
        public static void foundWalnut_Prefix(int stack)
        {
            if (m.playJingle)
            {
                Game1.delayedActions.Add(new DelayedAction(150, () => Game1.playSound("jingle1")));
                m.playJingle = false;
            }

            if (Game1.netWorldState.Value.GoldenWalnutsFound >= 130 && (m.disableWalnutCap == true || Game1.netWorldState.Value.GoldenWalnutsFound < ModEntry.Instance!.GoldenWalnutCap))
            {
                Game1.netWorldState.Value.GoldenWalnuts += stack;
                Game1.netWorldState.Value.GoldenWalnutsFound += stack;
                Game1.PerformActionWhenPlayerFree(Game1.player.showNutPickup);
            }
        }

        public static void RecountWalnuts_Postfix()
        {
            if (!Game1.IsMasterGame || Game1.getLocationFromName("IslandHut") is not IslandHut hut)
            {
                return;
            }
            int current_nut_count = ModEntry.Instance!.GoldenWalnutCap - hut.ShowNutHint();
            foreach (var group in SeparateWalnutGroups)
            {
                foreach (string id in group.Value)
                {
                    if (!Game1.player.team.collectedNutTracker.Contains(id))
                    {
                        current_nut_count--;
                    }
                }
                
            }
            if (Game1.netWorldState.Value.ActivatedGoldenParrot && !Game1.MasterPlayer.mailReceived.Contains("gotBirdieReward"))
            {
                current_nut_count += 5;
            }
            Game1.netWorldState.Value.GoldenWalnutsFound = current_nut_count;
            foreach (GameLocation location in Game1.locations)
            {
                if (location is not IslandLocation island_location) { continue; }
                foreach (ParrotUpgradePerch perch in island_location.parrotUpgradePerches)
                {
                    if (perch.currentState.Value == ParrotUpgradePerch.UpgradeState.Complete)
                    {
                        current_nut_count -= perch.requiredNuts.Value;
                    }
                }
            }
            if (Game1.netWorldState.Value.ActivatedGoldenParrot)
            {
                current_nut_count--; //Golden Parrot has requiredNuts Value -1
            }
            if (Game1.MasterPlayer.hasOrWillReceiveMail("Island_VolcanoShortcutOut"))
            {
                current_nut_count -= 5;
            }
            if (Game1.MasterPlayer.hasOrWillReceiveMail("Island_VolcanoBridge"))
            {
                current_nut_count -= 5;
            }
            foreach (var perch in CustomPerches)
            {
                if (Game1.MasterPlayer.mailReceived.Contains(perch.ID))
                {
                    current_nut_count -= (int)perch.Nuts!;
                }
            }
            foreach (var flag in QiShopFlags)
            {
                if (Game1.MasterPlayer.mailReceived.Contains(flag.Key))
                {
                    m.Monitor.Log($"{flag.Key}, {flag.Value}", LogLevel.Debug);
                    current_nut_count -= flag.Value;
                }
            }
            Game1.netWorldState.Value.GoldenWalnuts = current_nut_count;
        }

        public static IEnumerable<CodeInstruction> ShowNutHint_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                yield return codes[i];
                if (codes[i].opcode == OpCodes.Stloc_2)
                {
                    for (int index = 0; index < m.walnutGroupCountForTranspiler; index++)
                    {
                        Label skipAdd = generator.DefineLabel();
                        yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                        yield return new CodeInstruction(OpCodes.Stloc_S, (byte)6);
                        yield return new CodeInstruction(OpCodes.Ldloca_S, (byte)6);
                        yield return new CodeInstruction(OpCodes.Ldc_I4, index);
                        yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                        yield return new CodeInstruction(OpCodes.Ldc_I4, index);
                        yield return new CodeInstruction(OpCodes.Call, typeof(MainPatches).GetMethod(nameof(WalnutGroupWalnutsForTranspiler)));
                        yield return new CodeInstruction(OpCodes.Call, typeof(MainPatches).GetMethod(nameof(MissingTheseNuts)));
                        yield return new CodeInstruction(OpCodes.Brfalse_S, skipAdd);
                        yield return new CodeInstruction(OpCodes.Ldloc_0);
                        yield return new CodeInstruction(OpCodes.Ldc_I4, index);
                        yield return new CodeInstruction(OpCodes.Call, typeof(MainPatches).GetMethod(nameof(WalnutGroupHintForTranspiler)));
                        yield return new CodeInstruction(OpCodes.Ldloc_S, (byte)6);
                        yield return new CodeInstruction(OpCodes.Newobj, typeof(KeyValuePair<string, int>).GetConstructor([typeof(string), typeof(int)]));
                        yield return new CodeInstruction(OpCodes.Callvirt, typeof(List<KeyValuePair<string, int>>).GetMethod("Add"));
                        yield return new CodeInstruction(OpCodes.Ldloc_1);
                        yield return new CodeInstruction(OpCodes.Ldloc_S, (byte)6);
                        yield return new CodeInstruction(OpCodes.Add);
                        yield return new CodeInstruction(OpCodes.Stloc_1);
                        yield return new CodeInstruction(OpCodes.Nop) { labels = [skipAdd] };
                        yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                        yield return new CodeInstruction(OpCodes.Stloc_S, (byte)6);
                    }
                }
                else if (codes[i].opcode == OpCodes.Callvirt && ((MethodInfo)codes[i].operand).Name == "Squawk")
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, typeof(ModEntry).GetField("Instance"));
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 5);
                    yield return new CodeInstruction(OpCodes.Callvirt, typeof(ModEntry).GetMethod(nameof(ModEntry.HandleShowNutHintsForTranspiler)));
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 5);
                    yield return new CodeInstruction(OpCodes.Initobj, typeof(KeyValuePair<string, int>?));
                }
            }
        }



        public static string[] WalnutGroupWalnutsForTranspiler(int index)
        {
            return WalnutGroups[index].Value;
        }

        public static string WalnutGroupHintForTranspiler(int index)
        {
            return WalnutGroups[index].Key;
        }

        public static bool MissingTheseNuts(ref int __result, int index, bool separateGroups, params string[] WalnutIDs)
        {
            int num = 0;
            foreach (string id in WalnutIDs)
            {
                if (!Game1.player.team.collectedNutTracker.Contains(id))
                {
                    num++;
                }
            }
            __result += num;
            if (!separateGroups)
            {
                if (!GameStateQuery.CheckConditions(WalnutGroupsHintConditions[index]))
                {
                    return false;
                }
            }
            else
            {
                if (!GameStateQuery.CheckConditions(SeparateWalnutGroupsHintConditions[index]))
                {
                    return false;
                }
            }
            return num > 0;
        }

        public static bool MissingLimitedNutDrops_Prefix(string key, ref bool __result)
        {
            if (Game1.player.team.GetDroppedLimitedNutCount(key) > 99)
            {
                __result = false;
                return false;
            }
            return true;
        }

        public static void IslandHutUpdateWhenCurrentLocation_Prefix(IslandHut __instance, GameTime time)
        {
            if (__instance.hintShowTime > (float)time.ElapsedGameTime.TotalSeconds)
            {
                return;
            }
            if (__instance.hintDialogues.Count is 3 or 5)
            {
                __instance.hintDialogues.RemoveAt(0);
                __instance.hintShowTime = Math.Max(3f, __instance.hintDialogues[0].Length/10);
                __instance.Squawk();
            }
        }

        public static IEnumerable<CodeInstruction> WalnutCapReplaceTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4 && (int)codes[i].operand == 130)
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MainPatches), nameof(GetWalnutCapIntForTranspiler)));
                }
                else if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 130)
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MainPatches), nameof(GetWalnutCapFloatForTranspiler)));
                }
                else
                {
                    yield return codes[i];
                }
            }
        }

        public static int GetWalnutCapIntForTranspiler()
        {
            return ModEntry.Instance!.GoldenWalnutCap;
        }

        public static float GetWalnutCapFloatForTranspiler()
        {
            {
                return ModEntry.Instance!.GoldenWalnutCap;
            }
        }

        public static void IslandNorthGoldenParrot_Postfix(IslandNorth __instance)
        {
            IslandNorth islandNorth = (IslandNorth)Game1.getLocationFromName("IslandNorth");
            if (islandNorth == null) { return; }
            int walnutsFound = Game1.netWorldState.Value.GoldenWalnutsFound;
            if (!Game1.netWorldState.Value.ActivatedGoldenParrot && walnutsFound < ModEntry.Instance?.GoldenWalnutCap && walnutsFound >= 130)
            {
                islandNorth.parrotUpgradePerches.Add(new ParrotUpgradePerch(__instance, new Point(14, 14), new Microsoft.Xna.Framework.Rectangle(2, 2, __instance.Map.Layers[0].LayerWidth - 4, __instance.Map.Layers[0].LayerHeight - 4), -1, delegate
                {
                }, () => false, "GoldenParrot"));
            }
        }

        public static void CheckActionForGoldenParrot_Postfix(ParrotUpgradePerch __instance, Location tile_location)
        {
            if (__instance.IsAtTile(tile_location.X, tile_location.Y) && __instance.IsAvailable())
            {
                string? request_text = null;
                if (__instance.upgradeName.Value == "GoldenParrot")
                {
                    request_text = Game1.content.LoadStringReturnNullIfNotFound("Strings\\1_6_Strings:GoldenParrot");
                }
                GameLocation location = __instance.locationRef.Value;
                if (request_text != null && location != null)
                {
                    if (__instance.upgradeName.Value == "GoldenParrot")
                    {
                        if (Game1.MasterPlayer.hasOrWillReceiveMail("activateGoldenParrotsTonight"))
                        {
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:GoldenParrot_Tonight"));
                            return;
                        }
                        int cost = (ModEntry.Instance!.GoldenWalnutCap - Game1.netWorldState.Value.GoldenWalnutsFound) * 10000;
                        location.createQuestionDialogue(request_text,
                        [
                        new Response("Yes", string.Format(Game1.content.LoadString("Strings\\1_6_Strings:GoldenParrot_Yes"), Utility.getNumberWithCommas(cost))).SetHotKey((Keys)89),
                        new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")).SetHotKey((Keys)27)
                        ], "GoldenParrot");
                    }
                }
            }
        }

        public static bool blockIfForGoldenParrot = true;
        public static void AnswerQuestionForGoldenParrot_Postfix(ParrotUpgradePerch __instance, Response answer, ref bool __result)
        {

            if (Game1.currentLocation.lastQuestionKey != null)
            {
                string question_and_answer = ArgUtility.SplitBySpace(Game1.currentLocation.lastQuestionKey)[0] + "_" + answer.responseKey;
                if (question_and_answer == "UpgradePerch_" + __instance.upgradeName.Value + "_Yes")
                {
                    return;
                }
                if (!(question_and_answer == "GoldenParrotConfirm_Yes"))
                {
                    blockIfForGoldenParrot = true;
                    __result = false;
                    return;
                }
                else
                {
                    if (blockIfForGoldenParrot == false)
                    {
                        Game1.player.Money -= 100000;
                        blockIfForGoldenParrot = true;
                        __result = true;
                        return;
                    }
                    blockIfForGoldenParrot = false;
                    int cost = (ModEntry.Instance!.GoldenWalnutCap - Game1.netWorldState.Value.GoldenWalnutsFound) * 10000;
                    if (Game1.player.Money >= cost)
                    {
                        __instance.locationRef.Value.createQuestionDialogue(Game1.content.LoadString("Strings\\1_6_Strings:GoldenParrot_Sure"),
                        [
                        new Response("Yes", string.Format(Game1.content.LoadString("Strings\\1_6_Strings:GoldenParrot_Yes"), Utility.getNumberWithCommas(cost))).SetHotKey((Keys)89),
                        new Response("No", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")).SetHotKey((Keys)27)
                        ], "GoldenParrotConfirm");
                        __result = true;
                        return;
                    }
                    else
                    {
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
                        blockIfForGoldenParrot = true;
                        __result = true;
                        return;
                    }
                }
            }
            __result = false;
            return;
        }

        public static void ActivateGoldenParrot_Postfix()
        {
            var nuttracker = Game1.player.team.collectedNutTracker;
            int Before = nuttracker.Count;

            foreach (var walnut in CustomWalnuts)
            {
                if (walnut.Count > 1)
                {
                    for (int i = 1; i <= walnut.Count; i++)
                    {
                        nuttracker.Add($"{walnut.ID}_{i}");
                    }
                }
                else
                {
                    nuttracker.Add(walnut.ID);
                }
            }
            int After = nuttracker.Count;
            

            Game1.netWorldState.Value.GoldenWalnuts += After - Before;
            Game1.netWorldState.Value.GoldenWalnutsFound = ModEntry.Instance!.GoldenWalnutCap;
            Game1.MasterPlayer.mailReceived.Add("gotBirdieReward");

            foreach (string l in WalnutLocations)
            {
                GameLocation location = Game1.getLocationFromName(l);
                if (location.largeTerrainFeatures == null) { continue; }
                foreach (LargeTerrainFeature largeTerrainFeature in location.largeTerrainFeatures)
                {
                    if (largeTerrainFeature is Bush bush && bush.size.Equals(4))
                    {
                        bush.tileSheetOffset.Value = 0;
                        bush.setUpSourceRect();
                    }
                }
            }
        }

        public static void donatePiece_Prefix(int which)
        {
            int walnutCount = Game1.netWorldState.Value.GoldenWalnutsFound;
            if (walnutCount < 130) { return; }
            IslandFieldOffice? office = Game1.getLocationFromName("IslandFieldOffice") as IslandFieldOffice;
            if (office == null) { return; }
            office.piecesDonated[which] = true;
            if (!office.centerSkeletonRestored.Value && office.isRangeAllTrue(0, 6))
            {
                office.uncollectedRewards.Add(ItemRegistry.Create("(O)73", 6));
                return;
            }
            if (!office.snakeRestored.Value && office.isRangeAllTrue(6, 9))
            {
                office.uncollectedRewards.Add(ItemRegistry.Create("(O)73", 3));
                return;
            }
            if (!office.batRestored.Value && office.piecesDonated[9])
            {
                office.uncollectedRewards.Add(ItemRegistry.Create("(O)73"));
                return;
            }
            if (!office.frogRestored.Value && office.piecesDonated[10])
            {
                office.uncollectedRewards.Add(ItemRegistry.Create("(O)73"));
                return;
            }
        }


        public static void ApplyPlantRestore_Postfix(IslandFieldOffice __instance)
        {
            if (Game1.netWorldState.Value.GoldenWalnutsFound >= 130)
            {
                Game1.createItemDebris(ItemRegistry.Create("(O)73"), new Vector2(1.5f, 3.3f) * 64f, 1, __instance, 256);
            }
        }

        public static bool FishingRodDoFunction_Prefix(FishingRod __instance, GameLocation location, int x, int y, int power, Farmer who)
        {
            x = (int)(__instance.bobber.X / 64f);
            y = (int)(__instance.bobber.Y / 64f);
            if (!__instance.isNibbling || !__instance.isFishing) { return true; }
            if (!who.IsLocalPlayer) { return true; }
            var nuttracker = who.team.collectedNutTracker;
            foreach (var walnut in CustomWalnuts)
            {
                if (walnut.Type!.Equals("Fishing", StringComparison.OrdinalIgnoreCase)) { continue; }
                if (nuttracker.Contains(walnut.ID) || nuttracker.Contains($"{walnut.ID}_{walnut.Count}")) { continue; }
                if (!location.Name.Equals(walnut.Location, StringComparison.OrdinalIgnoreCase)) { continue; }
                if (!GameStateQuery.CheckConditions(walnut.Conditions)) { continue; }
                bool onCoordinates = x == walnut.X && y == walnut.Y;
                bool inArea = walnut.Areas?.Any(tile => x >= tile.X && x < tile.X + (tile.Width ?? 1) && y >= tile.Y && y < tile.Y + (tile.Height ?? 1)) == true;
                if (onCoordinates || inArea || (walnut.X == null && walnut.Areas == null))
                {
                    if (Game1.random.NextDouble() <= (walnut.Chance ?? 1))
                    {
                        __instance.pullFishFromWater("(O)73", 1, 0, 0, false, false, false, null, false, 1);
                        if (walnut.Count > 1)
                        {
                            for (int i = 1; i <= walnut.Count; i++)
                            {
                                if (!nuttracker.Contains($"{walnut.ID}_{i}"))
                                {
                                    nuttracker.Add($"{walnut.ID}_{i}");
                                    break;
                                }
                            }
                            if (nuttracker.Contains($"{walnut.ID}_{walnut.Count}"))
                            {
                                m.playJingle = true;
                            }
                        }
                        else { nuttracker.Add(walnut.ID); }
                        __instance.isNibbling = false;
                        return false;
                    }
                    else { return true; }
                }
            }
            return true;
        }

        public static void GameLocationDayUpdateHarmony_Postfix(GameLocation __instance) //This makes the mod ResetTerrainFeatures work properly
        {
            foreach (var walnut in CustomWalnuts)
            {
                if (!walnut.Type!.Equals("Bush", StringComparison.OrdinalIgnoreCase)) { continue; }
                if (!walnut.Location!.Equals(__instance.Name, StringComparison.OrdinalIgnoreCase)) { continue; }
                mf.SpawnBushes(walnut);
            }
            foreach (var ltf in __instance.largeTerrainFeatures)
            {
                if (ltf is Bush bush && bush.size.Value == 4 && !bush.modData.ContainsKey("ResoNight/GoldenWalnutFramework/CustomID") && Game1.player.team.collectedNutTracker.Contains($"Bush_{bush.Location.Name}_{bush.netTilePosition.X}_{bush.netTilePosition.Y}"))
                {
                    bush.tileSheetOffset.Value = 0;
                    bush.setUpSourceRect();
                }
            }
        }

        public static void loadMap_Postfix(GameLocation __instance)
        {
            if (__instance.Map == null) { return; }
            if (__instance is Town)
            {
                foreach (var perch in CustomPerches)
                {
                    if (!Game1.player.team.collectedNutTracker.Contains(perch.ID)) { continue; }
                    if (perch.Location!.Equals(__instance.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        Game1.delayedActions.Add(new DelayedAction(10, () => mf.ExecutePerch(perch)));
                    }
                }
            }
        }
    }

    internal static class BasePatches
    {
        public static void GameLocationResetLocalStateHarmony_Postfix(GameLocation __instance)
        {
            foreach (ParrotUpgradePerch perch in ModEntry.Instance!.Custom_parrotUpgradePerches)
            {
                if (perch.locationRef?.Value != __instance) { continue; }
                perch.ResetForPlayerEntry();
            }
        }

        public static void GameLocationUpdateWhenCurrentLocationHarmony_Postfix(GameTime time, GameLocation __instance)
        {
            foreach (ParrotUpgradePerch perch in ModEntry.Instance!.Custom_parrotUpgradePerches)
            {
                if (perch.locationRef?.Value != __instance) { continue; }
                perch.Update(time);
            }
        }

        public static void GameLocationDrawHarmony_Postfix(SpriteBatch b, GameLocation __instance)
        {
            foreach (ParrotUpgradePerch perch in ModEntry.Instance!.Custom_parrotUpgradePerches)
            {
                if (perch.locationRef?.Value != __instance) { continue; }
                perch.Draw(b);
            }
        }


        public static void GameLocationAnswerDialogueHarmony_Postfix(Response answer, GameLocation __instance, ref bool __result)
        {
            foreach (ParrotUpgradePerch perch in ModEntry.Instance!.Custom_parrotUpgradePerches)
            {
                if (perch.locationRef?.Value != __instance) { continue; }
                __result = true;
            }
        }

        public static void GameLocationCleanupBeforePlayerExitHarmony_Postfix(GameLocation __instance)
        {
            foreach (ParrotUpgradePerch perch in ModEntry.Instance!.Custom_parrotUpgradePerches)
            {
                if (perch.locationRef?.Value != __instance) { continue; }
                perch.Cleanup();
            }
        }

        public static void GameLocationUpdateEvenIfFarmerIsntHereHarmony_Postfix(GameTime time, GameLocation __instance, bool ignoreWasUpdatedFlush = false)
        {
            foreach (ParrotUpgradePerch perch in ModEntry.Instance!.Custom_parrotUpgradePerches)
            {
                if (perch.locationRef?.Value != __instance) { continue; }
                perch.UpdateEvenIfFarmerIsntHere(time);
            }
        }

        public static void GameLocationTransferDataFromSavedLocationHarmony_Postfix(GameLocation l)
        {
            foreach (ParrotUpgradePerch perch in ModEntry.Instance!.Custom_parrotUpgradePerches)
            {
                if (perch.locationRef?.Value != l) { continue; }
                perch.UpdateCompletionStatus();
            }
        }

        public static void GameLocationIsActionableTileHarmony_Postfix(int xTile, int yTile, Farmer who, GameLocation __instance, ref bool __result)
        {
            foreach (ParrotUpgradePerch perch in ModEntry.Instance!.Custom_parrotUpgradePerches)
            {
                if (perch.locationRef?.Value != __instance) { continue; }
                if (perch.IsAtTile(xTile, yTile) && perch.IsAvailable(use_cached_value: true) && perch.parrotPresent)
                {
                    __result = true;
                }
            }
        }
        public static void GameLocationCheckActionHarmony_Postfix(Location tileLocation, Microsoft.Xna.Framework.Rectangle viewport, Farmer who, GameLocation __instance, ref bool __result)
        {
            foreach (ParrotUpgradePerch perch in ModEntry.Instance!.Custom_parrotUpgradePerches)
            {
                if (perch.locationRef?.Value != __instance) { continue; }
                if (perch.CheckAction(tileLocation, who))
                {
                    __result = true;
                }
            }
        }

        public static void GameLocationDrawAboveAlwaysFrontLayerHarmony_Postfix(SpriteBatch b, GameLocation __instance)
        {
            foreach (ParrotUpgradePerch perch in ModEntry.Instance!.Custom_parrotUpgradePerches)
            {
                if (perch.locationRef?.Value != __instance) { continue; }
                perch.DrawAboveAlwaysFrontLayer(b);
            }
        }
        public static bool bushHasWalnut = true;
        public static void BushShake_Prefix(Bush __instance)
        {
            if (__instance.tileSheetOffset.Value == 0)
            {
                bushHasWalnut = false;
            }
        }

        public static void BushShake_Postfix(Vector2 tileLocation, Bush __instance)
        {
            if (!bushHasWalnut)
            {
                bushHasWalnut = true;
                return;
            }
            string generatedID = $"Bush_{__instance.Location.Name}_{tileLocation.X}_{tileLocation.Y}";
            if (__instance.modData.TryGetValue("ResoNight.GoldenWalnutFramework/CustomID", out var customID))
            {
                Game1.delayedActions.Add(new DelayedAction(10, () => Game1.player.team.collectedNutTracker.Remove(generatedID)));
                Game1.player.team.collectedNutTracker.Add(customID);
            }
            
        }
    }


    public class BaseJSON
    {
        public List<CustomParrotUpgradePerch> ParrotUpgradePerches { get; set; } = new();
        public Dictionary<string, WalnutGroup> GoldenWalnuts { get; set; } = new();
        public Settings Settings { get; set; } = new();
    }

    public class WalnutGroup
    {
        public string? Hint { get; set; }
        public string? Singular { get; set; }
        public bool? SeparateHint { get; set; }
        public bool? ShowThisHint { get; set; }
        public string? HintConditions { get; set; }
        public List<CustomWalnut>? Walnuts { get; set; }
    }

    
    public class CustomParrotUpgradePerch
    {
        public string? ID { get; set; }
        public string? Location { get; set; }
        public bool? StoneAnimation { get; set; }
        public int? Nuts { get; set; }
        public string? StickType { get; set; }
        public ParrotTile? ParrotTile { get; set; }
        public JSONRect? ParrotArea { get; set; }
        public List<DestroyArea>? DestroyAreas { get; set; }
        public JSONRect? FromArea { get; set; }
        public JSONRect? ToArea { get; set; }
        public string? FromFile { get; set; }
        public string? Condition { get; set; }

    }

    public class DestroyArea
    {
        public int? X { get; set; }
        public int? Y { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public List<string>? Layers { get; set; }
    }


    public class ParrotTile
    {
        public int? X { get; set; }
        public int? Y { get; set; }
    }

    public class JSONRect
    {
        public int? X { get; set; }
        public int? Y { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
    }



    public class CustomWalnut
    {
        public string? ID { get; set; }
        public string? Location { get; set; }
        public string? Type { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
        public int? Count { get; set; }
        public string? DropAtOnce { get; set; }
        public float? Chance { get; set; }
        public List<JSONRect>? Areas { get; set; }
        public List<string>? MonsterTypes { get; set; }
        public List<string>? StoneTypes { get; set; }
        public string? Singular { get; set; }
        public string? Conditions { get; set; }

    }

    public class Settings
    {
        public bool? DisableWalnutCap {  get; set; }
        public List<string>? DisableSeasonalFeaturesForMaps  { get; set; }
        public Dictionary<string, int>? SpentWalnuts { get; set; }

    }
}

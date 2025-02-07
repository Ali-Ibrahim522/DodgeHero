using System.Collections.Generic;
using Global;
using UnityEngine;

namespace LevelSelect {
    public static class LevelDiffChallengeInfo {
        public class ChallengeInfo {
            public string ChallengeHeader;
            public string ChallengeBody;
        }

        public static Dictionary<GameStateManager.GameState, List<ChallengeInfo>> ChallengeInfoMap;

        public static void LoadChallengeInfoMap() {
            ChallengeInfoMap = new Dictionary<GameStateManager.GameState, List<ChallengeInfo>> {
                {
                    GameStateManager.GameState.LevelSelect,
                    new List<ChallengeInfo> {
                        new() {
                            ChallengeHeader = "???",
                            ChallengeBody = "Coming Soon..."
                        }
                    }
                }, {
                    GameStateManager.GameState.ScarecrowEasy,
                    new List<ChallengeInfo> {
                        new() {
                            ChallengeHeader = "ATTACKS",
                            ChallengeBody =
                                $"Scarecrow attacks in the 4 cardinal directions with their sword.\n\nPress the corresponding input for the scarecrow's attack before they complete it.\n\u2191 : [{PlayerDataManager.Instance.GetKeybind("Scarecrow", "Up Attack")}]\n\u2190 : [{PlayerDataManager.Instance.GetKeybind("Scarecrow", "Left Attack")}]\n\u2193 : [{PlayerDataManager.Instance.GetKeybind("Scarecrow", "Down Attack")}]\n\u2192 : [{PlayerDataManager.Instance.GetKeybind("Scarecrow", "Right Attack")}]\nThere is an arrow for each direction that changes colors to guide you...\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.proposed)}>\u25a0</color></size></voffset> when you need to press the corresponding input.\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.hit)}>\u25a0</color></size></voffset> If you have done so successfully.\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.missed)}>\u25a0</color></size></voffset> If you fail to do so before the attack is completed."
                        }
                    }
                }, {
                    GameStateManager.GameState.ScarecrowMedium,
                    new List<ChallengeInfo> {
                        new() {
                            ChallengeHeader = "ATTACKS",
                            ChallengeBody =
                                $"Scarecrow attacks in the 4 cardinal directions with their sword.\n\nPress the corresponding input for the scarecrow's attack before they complete it.\n\u2191 : [{PlayerDataManager.Instance.GetKeybind("Scarecrow", "Up Attack")}]\n\u2190 : [{PlayerDataManager.Instance.GetKeybind("Scarecrow", "Left Attack")}]\n\u2193 : [{PlayerDataManager.Instance.GetKeybind("Scarecrow", "Down Attack")}]\n\u2192 : [{PlayerDataManager.Instance.GetKeybind("Scarecrow", "Right Attack")}]\nThere is an arrow for each direction that changes colors to guide you...\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.proposed)}>\u25a0</color></size></voffset> when you need to press the corresponding input.\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.hit)}>\u25a0</color></size></voffset> If you have done so successfully.\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.missed)}>\u25a0</color></size></voffset> If you fail to do so before the attack is completed."
                        },
                        new() {
                            ChallengeHeader = "TARGETS",
                            ChallengeBody =
                                $"Targets spawn out of the ground from six different positions one at a time.\n\nWhen a target appears, Aim over the target and press [{PlayerDataManager.Instance.GetKeybind("Scarecrow", "Hit Target")}] before the target goes back into the ground.\n\nEach target has a ring that changes colors to guide you...\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.proposed)}>\u25a0</color></size></voffset> when you need to press the corresponding input.\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.hit)}>\u25a0</color></size></voffset> If you have done so successfully over the target.\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.missed)}>\u25a0</color></size></voffset> If you fail to do so before the target disappears or press the M1 input while not aiming on a target."
                        }
                    }
                }, {
                    GameStateManager.GameState.ScarecrowHard,
                    new List<ChallengeInfo> {
                        new() {
                            ChallengeHeader = "ATTACKS",
                            ChallengeBody =
                                $"Scarecrow attacks in the 4 cardinal directions with their sword.\n\nPress the corresponding input for the scarecrow's attack before they complete it.\n\u2191 : [{PlayerDataManager.Instance.GetKeybind("Scarecrow", "Up Attack")}]\n\u2190 : [{PlayerDataManager.Instance.GetKeybind("Scarecrow", "Left Attack")}]\n\u2193 : [{PlayerDataManager.Instance.GetKeybind("Scarecrow", "Down Attack")}]\n\u2192 : [{PlayerDataManager.Instance.GetKeybind("Scarecrow", "Right Attack")}]\nThere is an arrow for each direction that changes colors to guide you...\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.proposed)}>\u25a0</color></size></voffset> when you need to press the corresponding input.\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.hit)}>\u25a0</color></size></voffset> If you have done so successfully.\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.missed)}>\u25a0</color></size></voffset> If you fail to do so before the attack is completed."
                        },
                        new() {
                            ChallengeHeader = "TARGETS",
                            ChallengeBody =
                                $"Targets spawn out of the ground from six different positions one at a time.\n\nWhen a target appears, Aim over the target and press [{PlayerDataManager.Instance.GetKeybind("Scarecrow", "Hit Target")}] before the target goes back into the ground.\n\nEach target has a ring that changes colors to guide you...\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.proposed)}>\u25a0</color></size></voffset> when you need to press the corresponding input.\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.hit)}>\u25a0</color></size></voffset> If you have done so successfully over the target.\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.missed)}>\u25a0</color></size></voffset> If you fail to do so before the target disappears or press the M1 input while not aiming on a target."
                        },
                        new() {
                            ChallengeHeader = "CROWS",
                            ChallengeBody =
                                $"Crows spawn from the left and right sides of the level at 6 different angles.\n\nAs crows swoop in towards the scarecrow, scare them away by pressing [{PlayerDataManager.Instance.GetKeybind("Scarecrow", "Scare Crow")}] when they are in range.\n\nEach crow's outline changes colors to guide you...\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.proposed)}>\u25a0</color></size></voffset> When you need to press the corresponding input.\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.hit)}>\u25a0</color></size></voffset> If you have done so successfully with a crow in range.\n<voffset=\"-5\"><size=160%><color=#{ColorUtility.ToHtmlStringRGBA(PlayerDataManager.Instance.visuals.missed)}>\u25a0</color></size></voffset> If you fail to do so before the crow touches the scarecrow or if you press SPACE with no crows in range."
                        }
                    }
                }
            };
        }
    }
}


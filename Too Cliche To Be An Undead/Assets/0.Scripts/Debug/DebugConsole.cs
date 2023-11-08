using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DebugConsole : MonoBehaviour
{
    #region Modifiers const IDs;
    private const string MODIF_HEALTH_ID = "DC_CHEAT_HEALTH_X";
    private const string MODIF_DAMAGES_ID = "DC_CHEAT_DAMAGES_X";
    private const string MODIF_RANGE_ID = "DC_CHEAT_RANGE_X";
    private const string MODIF_CD_ID = "DC_CHEAT_CD_X";
    private const string MODIF_SPEED_ID = "DC_CHEAT_SPEED_X";
    private const string MODIF_CRIT_ID = "DC_CHEAT_CRIT_X";
    private const string MODIF_DASHCD_ID = "DC_CHEAT_DASH-CD_X";
    private const string MODIF_SKILLCD_ID = "DC_CHEAT_SKILL-CD_X"; 
    #endregion

    private TextEditor editor;

    private bool showConsole;
    private bool showHelp;

    private string input;

    private int currentSelectedSuggestion;

    #region Commands

    private DebugCommand HELP;

    private DebugCommand KILLALL;
    private DebugCommand<int> KILL;

    private DebugCommand SKIP_TUTO;
    private DebugCommand RESTART_IN_TUTO;
    private DebugCommand FORCEOPEN_GYMNASIUM;

    private DebugCommand FORCEWIN;
    private DebugCommand FORCEKILL_BOSS;

    private DebugCommand<int, int> SWITCH_CHARACTER;

    private DebugCommand<int> ADD_KEYCARD;

    private DebugCommand<float, int> HEAL;
    private DebugCommand<float, int> HEAL_CRITICAL;

    private DebugCommand<float, int> DAMAGE;
    private DebugCommand<float, int> DAMAGE_CRITICAL;

    private DebugCommand<int> REMOVE_ALL_MODIFIERS;

    private DebugCommand<float, int> ADDM_HP;
    private DebugCommand<float, int> ADDM_DAMAGES;
    private DebugCommand<float, int> ADDM_ATTRANGE;
    private DebugCommand<float, int> ADDM_ATTCD;
    private DebugCommand<float, int> ADDM_SPEED;
    private DebugCommand<int, int> ADDM_CRIT;
    private DebugCommand<float, int> ADDM_DASHCD;
    private DebugCommand<float, int> ADDM_SKILLCD;

    private DebugCommand GOLD_BAG;
    private DebugCommand PAYDAY;
    private DebugCommand RICH_AF;
    private DebugCommand<int> ADD_MONEY;

    private DebugCommand SHOW_FPS;

    private DebugCommand SHOW_VERSION;
    private DebugCommand HIDE_VERSION;

    private DebugCommand START_BREAKUP;
    private DebugCommand END_BREAKUP;
    private DebugCommand<bool> DEBUG_M_SPAWNERS;
    private DebugCommand<bool> SET_SPAWNS_STATE;

    #endregion

    private Vector2 helpScroll;
    private Vector2 suggestionsScroll;

    private List<string> suggestions = new List<string>();
    private List<Rect> suggestionsRect = new List<Rect>();

    private List<object> commandList = new List<object>();

    public GUIStyle suggestionSkin;
    public GUIStyle greyedText;

    private bool allowGameChange = true;

    private float labelsHeight = 20;

    private GameManager.E_GameState stateBeforeConsole;

    private void Awake()
    {
        CreateSimpleCommands();
        CreateIntCommands();
        CreateFloatCommands();
        CreateBoolCommands();

        commandList = new List<object>()
        {
            HELP,

            KILLALL,
            KILL,

            FORCEWIN,
            FORCEKILL_BOSS,

            SKIP_TUTO,
            RESTART_IN_TUTO,
            FORCEOPEN_GYMNASIUM,

            SWITCH_CHARACTER,

            ADD_KEYCARD,

            HEAL,
            HEAL_CRITICAL,

            DAMAGE,
            DAMAGE_CRITICAL,

            REMOVE_ALL_MODIFIERS,

            ADDM_HP,

            ADDM_DAMAGES,

            ADDM_ATTRANGE,

            ADDM_ATTCD,

            ADDM_SPEED,

            ADDM_CRIT,

            ADDM_DASHCD,

            ADDM_SKILLCD,

            ADD_MONEY,
            GOLD_BAG,
            PAYDAY,
            RICH_AF,

            SHOW_VERSION,
            HIDE_VERSION,

            START_BREAKUP,
            END_BREAKUP,

            SET_SPAWNS_STATE,

            SHOW_FPS,

#if UNITY_EDITOR
            DEBUG_M_SPAWNERS,
#endif
        };
    }

    private void CreateSimpleCommands()
    {
        HELP = new DebugCommand("HELP", "Shows all commands", "HELP", () =>
        {
            showHelp = !showHelp;
        });

        KILL = new DebugCommand<int>("KILL", "Kills the given character index", "KILL <int>", (int targetIdx) =>
        {
            if (!IGPlayersManager.ST_TryGetPlayer(targetIdx, out var player)) return;
            //player.InflinctDamages(player.MaxHP_M, null, isCrit: true);
        });

        KILLALL = new DebugCommand("KILL_ALL", "Kills every players", "KILL_ALL", () =>
        {
            if (!IGPlayersManager.ST_InstanceExists()) return;
            foreach (var item in IGPlayersManager.Instance.PlayersList)
            {
                //item.InflinctDamages(item.MaxHP_M, null);
            }
        });

        FORCEWIN = new DebugCommand("FORCE_WIN", "Forces the win conditions", "FORCE_WIN", () =>
        {
            this.ForceWin();
        });

        FORCEKILL_BOSS = new DebugCommand("FORCEKILL_BOSS", "Instantly kills the boss", "FORCEKILL_BOSS", () =>
        {
            //FindObjectOfType<BossZombie>().InflinctDamages(1000000, null);
        });

        GOLD_BAG = new DebugCommand("GOLD_BAG", "Adds 50 gold", "GOLD_BAG", () =>
        {
            InventoryManager.Instance.AddedMoney(50);
        });

        PAYDAY = new DebugCommand("PAYDAY", "Adds 200 gold", "PAYDAY", () =>
        {
            InventoryManager.Instance.AddedMoney(200);
        });

        RICH_AF = new DebugCommand("RICH_AF", "Adds 5000 gold", "RICH_AF", () =>
        {
            InventoryManager.Instance.AddedMoney(5000);
        });

        SKIP_TUTO = new DebugCommand("SKIP_TUTO", "Skips the tutorial", "SKIP_TUTO", () =>
        {
            if (GameManager.Instance.IsInTutorial == false) return;

            GameObject.FindObjectOfType<Tutorial>().ForceEndTutorial();
        });

        RESTART_IN_TUTO = new DebugCommand("RESTART_IN_TUTO", "Restarts the game, starting in the tutorial", "RESTART_IN_TUTO", () =>
        {
            DataKeeper.Instance.skipTuto = false;
            DataKeeper.Instance.alreadyPlayedTuto = false;
            GameManager.ChangeScene(GameManager.E_ScenesNames.MainScene, true);
        });

        FORCEOPEN_GYMNASIUM = new DebugCommand("FORCEOPEN_GYMNASIUM", "Opens the gymnasium door and plays the cutscene", "FORCEOPEN_GYMNASIUM", () =>
        {
            allowGameChange = false;
            FindObjectOfType<SpineGymnasiumDoor>()?.ForceOpen();
        });

        SHOW_VERSION = new DebugCommand("SHOW_VERSION", "Shows the current version of the game", "SHOW_VERSION", () =>
        {
            GameVerManager.SetShowVersion(true);
        });

        HIDE_VERSION = new DebugCommand("HIDE_VERSION", "Hides the current version of the game", "HIDE_VERSION", () =>
        {
            GameVerManager.SetShowVersion(false);
        });

        START_BREAKUP = new DebugCommand("START_BREAKUP", "Starts a spawns breakup", "START_BREAKUP", () =>
        {
            SpawnersManager.Instance.ForceBreakup();
        });

        END_BREAKUP = new DebugCommand("END_BREAKUP", "Ends the current breakup", "END_BREAKUP", () =>
        {
            SpawnersManager.Instance.EndBreakup();
        });

        SHOW_FPS = new DebugCommand("SHOW_FPS", "Shows or hides the FPS counter", "SHOW_FPS", () =>
        {
            FPSDisplayer.Instance.SetState(!FPSDisplayer.Instance.IsRunning());
        });
    }

    private void CreateBoolCommands()
    {
        DEBUG_M_SPAWNERS = new DebugCommand<bool>("DEBUG_M_SPAWNERS", "Activates the debug mode for spawners manager", "DEBUG_M_SPAWNERS <bool>", (val) =>
        {
            SpawnersManager.Instance.SetDebugMode(val);
        });

        SET_SPAWNS_STATE = new DebugCommand<bool>("SET_SPAWNS_STATE", "Activates or deactivates the zombies spawn", "SET_SPAWNS_STATE <bool>", (val) =>
        {
            SpawnersManager.Instance.AllowSpawns(val);
        });
    }

    private void CreateIntCommands()
    {
        REMOVE_ALL_MODIFIERS = new DebugCommand<int>("REMOVE_ALL_MODIFIERS", "Removes every modifiers of character index", "REMOVE_ALL_MODIFIERS_TARGET", (targetIdx) =>
        {
            if (!IGPlayersManager.ST_TryGetPlayer(targetIdx, out var player)) return;
            //player.RemoveModifiersAll();
        });

        SWITCH_CHARACTER = new DebugCommand<int, int>("SWITCH_CHARACTER", "Switchs to the desired character for target \n 0 = Shirley \n 1 = Whitney \n 2 = Jason \n 3 = Nelson", "SWITCH_CHARACTER <int> <int>", (character, targetIdx) =>
        {
            if (!GameAssets.ST_InstanceExists() || !IGPlayersManager.ST_InstanceExists()) return;
            if (!IGPlayersManager.ST_TryGetPlayer(targetIdx, out PlayerCharacter targetPlayer)) return;
            GameManager.E_CharactersNames desiredCharacter = GameManager.E_CharactersNames.Shirley;

            if (0 < character && character < 4) desiredCharacter = (GameManager.E_CharactersNames)character;

            SO_CharactersComponents newPCC = GameAssets.Instance.CharactersComponentsHolder.GetComponents(desiredCharacter);
            targetPlayer.SwitchCharacter(newPCC);
        });

        KILL = new DebugCommand<int>("KILL", "Kills the given player index", "KILL <int>", (val) =>
        {
            PlayerCharacter pc = DataKeeper.Instance.playersDataKeep[val].playerInput.GetComponentInParent<PlayerCharacter>();
            //pc.InflinctDamages(pc.MaxHP_M, null);
        });

        ADD_MONEY = new DebugCommand<int>("ADD_MONEY", "Adds <int> money", "ADD_MONEY <int>", (val) =>
        {
            InventoryManager.Instance.AddedMoney(val);
        });

        ADD_KEYCARD = new DebugCommand<int>("ADD_KEYCARD", "Adds <int> keycards", "ADD_KEYCARD <int>", (val) =>
        {
            GameManager.Instance.AcquiredCards += val;
        });
    }

    private void CreateFloatCommands()
    {
        HEAL = new DebugCommand<float, int>("HEAL", "Heals the targeted character", "HEAL <float> <int>(default : 0)", (amount, targetIdx) =>
        {
            if (!IGPlayersManager.ST_TryGetPlayer(targetIdx, out PlayerCharacter pc)) return;
            //pc.Heal(amount);
        }, T2defaultValue: 0);

        HEAL_CRITICAL = new DebugCommand<float, int>("HEAL_CRITICAL", "Heals the targeted character", "HEAL_CRITICAL <float> <int>(default : 0)", (amount, targetIdx) =>
        {
            if (!IGPlayersManager.ST_TryGetPlayer(targetIdx, out PlayerCharacter pc)) return;
            //pc.Heal(amount, true);
        }, T2defaultValue: 0);

        DAMAGE = new DebugCommand<float, int>("DAMAGE", "Damages the targeted character", "DAMAGE <float> <int>(default : 0)", (amount, targetIdx) =>
        {
            if (!IGPlayersManager.ST_TryGetPlayer(targetIdx, out PlayerCharacter pc)) return;
            //pc.InflinctDamages(amount, null);
        }, T2defaultValue: 0);

        DAMAGE_CRITICAL = new DebugCommand<float, int>("DAMAGE_CRITICAL", "Criticaly damages the targeted character", "DAMAGE_CRITICAL <float> <int>(default : 0)", (amount, targetIdx) =>
        {
            if (!IGPlayersManager.ST_TryGetPlayer(targetIdx, out PlayerCharacter pc)) return;
            //pc.InflinctDamages(amount, null, true);
        }, T2defaultValue: 0);

        ADDM_HP = new DebugCommand<float, int>("ADDM_HP", "Adds a HP modifier of <float> to <int>", "ADDM_HP <float> <int>(default : 0)", (amount, targetIdx) =>
        {
            if (!IGPlayersManager.ST_TryGetPlayer(targetIdx, out PlayerCharacter pc)) return;
            //pc.AddModifier(MODIF_HEALTH_ID, amount, StatsModifier.E_StatType.MaxHP);
        }, T2defaultValue: 0);

        ADDM_DAMAGES = new DebugCommand<float, int>("ADDM_DAMAGES", "Adds a damages modifier of <float> to <int>", "ADDM_DAMAGES <float> <int>(default : 0)", (amount, targetIdx) =>
        {
            if (!IGPlayersManager.ST_TryGetPlayer(targetIdx, out PlayerCharacter pc)) return;
            //pc.AddModifier(MODIF_DAMAGES_ID, amount, StatsModifier.E_StatType.Damages);
        }, T2defaultValue: 0);

        ADDM_ATTRANGE = new DebugCommand<float, int>("ADDM_ATTRANGE", "Adds a attack range modifier of <float> to <int>", "ADDM_ATTRANGE <float> <int>(default : 0)", (amount, targetIdx) =>
        {
            if (!IGPlayersManager.ST_TryGetPlayer(targetIdx, out PlayerCharacter pc)) return;
            //pc.AddModifier(MODIF_RANGE_ID, amount, StatsModifier.E_StatType.AttackRange);
        }, T2defaultValue: 0);

        ADDM_ATTCD = new DebugCommand<float, int>("ADDM_ATTCD", "Adds a attack cooldown modifier of <float> to <int>", "ADDM_ATTCD <float> <int>(default : 0)", (amount, targetIdx) =>
        {
            if (!IGPlayersManager.ST_TryGetPlayer(targetIdx, out PlayerCharacter pc)) return;
            //pc.AddModifier(MODIF_CD_ID, amount, StatsModifier.E_StatType.Attack_CD);
        }, T2defaultValue: 0);

        ADDM_SPEED = new DebugCommand<float, int>("ADDM_SPEED", "Adds a speed modifier of <float> to <int>", "ADDM_SPEED <float> <int>(default : 0)", (amount, targetIdx) =>
        {
            if (!IGPlayersManager.ST_TryGetPlayer(targetIdx, out PlayerCharacter pc)) return;
            //pc.AddModifier(MODIF_SPEED_ID, amount, StatsModifier.E_StatType.Speed);
        }, T2defaultValue: 0);

        ADDM_CRIT = new DebugCommand<int, int>("ADDM_CRIT", "Adds crit chances <int>% to <int>", "ADDM_CRIT <int> <int>(default : 0)", (amount, targetIdx) =>
        {
            if (!IGPlayersManager.ST_TryGetPlayer(targetIdx, out PlayerCharacter pc)) return;
            //pc.AddModifier(MODIF_CRIT_ID, amount, StatsModifier.E_StatType.CritChances);
        }, T2defaultValue: 0);

        ADDM_DASHCD = new DebugCommand<float, int>("ADDM_DASHCD", "Adds a dash cooldown modifier of <float> to <int>", "ADDM_DASHCD <float> <int>(default : 0)", (amount, targetIdx) =>
        {
            if (!IGPlayersManager.ST_TryGetPlayer(targetIdx, out PlayerCharacter pc)) return;
            //pc.AddModifier(MODIF_DASHCD_ID, amount, StatsModifier.E_StatType.DASH_CD);
        }, T2defaultValue: 0);

        ADDM_SKILLCD = new DebugCommand<float, int>("ADDM_SKILLCD", "Adds a skill cooldown modifier of <float> to <int>", "ADDM_SKILLCD <float> <int>(default : 0)", (amount, targetIdx) =>
        {
            if (!IGPlayersManager.ST_TryGetPlayer(targetIdx, out PlayerCharacter pc)) return;
            //pc.AddModifier(MODIF_SKILLCD_ID, amount, StatsModifier.E_StatType.SKILL_CD);
        }, T2defaultValue: 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Quote))
            OnToggleConsole();
        if (Input.GetKeyDown(KeyCode.Return))
            OnReturn();
    }

    /// <summary>
    /// Toggles or untoggles console when <seealso cref="KeyCode.Quote"/> button is pressed
    /// </summary>
    public void OnToggleConsole()
    {
        showConsole = !showConsole;

        if (showConsole)
        {
            stateBeforeConsole = GameManager.Instance.GameState;

            allowGameChange = stateBeforeConsole == GameManager.E_GameState.InGame;

            // Toggles the console, reset the field and blocks the game
            if (allowGameChange)
                GameManager.Instance.GameState = GameManager.E_GameState.Restricted;

            ResetField();
        }
        else
        {
            if (allowGameChange && stateBeforeConsole != GameManager.E_GameState.None) GameManager.Instance.GameState = stateBeforeConsole;
        }
    }

    public void OnReturn()
    {
        // if the command field is displayed, check the input and reset
        if (showConsole)
        {
            HandleInput();
            ResetField();

            if (allowGameChange)
                 GameManager.Instance.GameState = GameManager.E_GameState.InGame;

            allowGameChange = false;
            stateBeforeConsole = GameManager.E_GameState.None;

            showConsole = false;
        }
    }

    private void ResetField()
    {
        allowGameChange = true;
        showHelp = false;
        input = "";
        suggestionsScroll = Vector2.zero;
        currentSelectedSuggestion = 0;
        GUI.ScrollTo(new Rect(0,0,0,0));
    }

    private void HandleInput()
    {
        // Get the proprieties
        string[] proprieties = input.Split(' ');

        DebugCommandBase command;
        foreach (var item in commandList)
        {
            command = item as DebugCommandBase;

            // check if the input is the current checked command
            if (proprieties[0].Equals(command.CommandID))
            {
                // check the command type
                if (command as DebugCommand != null && proprieties.Length == 1)
                {
                    (command as DebugCommand).Invoke();
                    input = "";
                    return;
                }

                else if (command as DebugCommand<bool> != null && proprieties.Length == 2)
                {
                    DebugCommand<bool> cmd = command as DebugCommand<bool>;
                    if (!TryParseBool(proprieties[1], cmd.AllowDefaultValue, cmd.DefaultValue, out bool result)) return;

                    cmd.Invoke(result);
                    input = "";
                    return;
                }

                else if (command as DebugCommand<int> != null && proprieties.Length == 2)
                {
                    DebugCommand<int> cmd = command as DebugCommand<int>;
                    if (!TryParseInt(proprieties[1], cmd.AllowDefaultValue, cmd.DefaultValue, out int result)) return;

                    cmd.Invoke(result);
                    input = "";
                    return;
                }

                else if (command as DebugCommand<float, int> != null && proprieties.Length >= 2)
                {
                    DebugCommand<float, int> cmd = command as DebugCommand<float, int>;
                    if (!TryParseFloat(proprieties[1], cmd.AllowT1DefaultValue, cmd.T1DefaultValue, out float t1result)) return;
                    if (!TryParseInt(proprieties.Length >= 3 ? proprieties[2] : "null", cmd.AllowT2DefaultValue, cmd.T2DefaultValue, out int t2result)) return;
                    cmd.Invoke(t1result, t2result);
                    input = "";
                    return;
                }

                else if (command as DebugCommand<int, int> != null && proprieties.Length >= 2)
                {
                    DebugCommand<int, int> cmd = command as DebugCommand<int, int>;
                    if (!TryParseInt(proprieties[1], cmd.AllowT1DefaultValue, cmd.T1DefaultValue, out int t1result)) return;
                    if (!TryParseInt(proprieties.Length >= 3 ? proprieties[2] : "null", cmd.AllowT2DefaultValue, cmd.T2DefaultValue, out int t2result)) return;
                    cmd.Invoke(t1result, t2result);
                    input = "";
                    return;
                }
            }
        }

        input = "";
    }

    private bool TryParseFloat(string propriety, bool allowDefaultValue, float defaultValue, out float result)
    {
        result = -1;
        if (float.TryParse(propriety, out result)) return true;
        if (allowDefaultValue)
        {
            result = defaultValue;
            return true;
        }

        this.Log("Could not parse " + propriety, CustomLogger.E_LogType.Error);
        return false;
    }
    private bool TryParseInt(string propriety, bool allowDefaultValue, int defaultValue, out int result)
    {
        result = -1;
        if (int.TryParse(propriety, out result)) return true;
        if (allowDefaultValue)
        {
            result = defaultValue;
            return true;
        }
        this.Log("Could not parse " + propriety, CustomLogger.E_LogType.Error);
        return false;
    }
    private bool TryParseBool(string propriety, bool allowDefaultValue, bool defaultValue, out bool result)
    {
        result = false;
        if (propriety.Equals("<bool>"))
        {
            result = true;
            return true;
        }
        if (propriety.Equals("1") || propriety.Equals("TRUE"))
        {
            result = true;
            return true;
        }
        if (allowDefaultValue)
        {
            result = defaultValue;
            return true;
        }

        this.Log("Could not parse " + propriety, CustomLogger.E_LogType.Error);
        return false;
    }

    private void OnGUI()
    {
        if (!showConsole) return;

        Color baseColor = GUI.backgroundColor;

        float y = 0f;

        // Draw the help box field if needed
        Rect helpBoxRect = new Rect(0, y, Screen.width, 100);
        if (showHelp) DrawHelpBox(ref y, ref helpBoxRect);

        // draw the input field
        Rect inputBoxRect = new Rect(0, y, Screen.width, labelsHeight + 10);
        DrawInputBox(ref y, ref inputBoxRect);

        // will be useful if we need to manually set the selected text
        editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);

        if (input == "" || input == null) return;

        // handles the latest input from the user
        HandleLatestInput();

        DrawSuggestionsBox(y, baseColor);
    }

    private void DrawHelpBox(ref float y, ref Rect helpBoxRect)
    {
        // draw the help box
        GUI.Box(helpBoxRect, "");
        Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * commandList.Count);
        helpScroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), helpScroll, viewport);

        // show every command with their description and format
        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase command = commandList[i] as DebugCommandBase;

            string label = $"{command.CommandFormat} - {command.CommandDescription}";

            Rect labelRect = new Rect(5, labelsHeight * i, viewport.width - 100, labelsHeight);

            GUI.Label(labelRect, label);
        }

        GUI.EndScrollView();

        y += 100;
    }

    private void DrawInputBox(ref float y, ref Rect inputBoxRect)
    {
        GUI.Box(inputBoxRect, "");

        GUI.backgroundColor = new Color(0, 0, 0, 0);
        GUI.SetNextControlName("InputField");
        Rect inputRect = new Rect(10f, y + 5f, Screen.width - labelsHeight, labelsHeight);
        input = GUI.TextField(inputRect, input);
        GUI.FocusControl("InputField");

        y += inputBoxRect.height;
    }

    private void DrawSuggestionsBox(float y, Color baseColor)
    {
        GUI.backgroundColor = baseColor;

        Rect suggestionsBoxRect = new Rect(0, y, Screen.width, 60);
        GUI.Box(suggestionsBoxRect, "");

        suggestions = new List<string>();
        suggestionsRect = new List<Rect>();

        // iterates through every commands
        for (int i = 0; i < commandList.Count; i++)
        {
            // if the current input starts with the i command ID, add it to the suggestions
            DebugCommandBase command = commandList[i] as DebugCommandBase;
            if (command == null)
            {
                this.Log($"Command n°{i} ({command}) was not build.", CustomLogger.E_LogType.Error); 
                continue;
            }
            if (command.CommandID.StartsWith(input) == false) continue;

            suggestions.Add(command.CommandFormat);

            Vector2 suggestionPos = new Vector2(
                x: 5,
                y: (y + suggestionsRect.Count) + labelsHeight * (i + 1)
                );

            Rect suggestionRect = new Rect(
                x: suggestionPos.x,
                y: suggestionPos.y,
                width: Screen.width - 130,
                height: labelsHeight
                );

            suggestionsRect.Add(suggestionRect);
        }

        Rect suggestionsViewport = new Rect(0, y, Screen.width - 30, labelsHeight * suggestions.Count);
        Rect suggestionScrollRect = new Rect(0, y + 5f, Screen.width, 50);
        suggestionsScroll = GUI.BeginScrollView(suggestionScrollRect, suggestionsScroll, suggestionsViewport);

        for (int i = 0; i < suggestions.Count; i++)
        {
            string label = suggestions[i];

            Vector2 suggestionPos = new Vector2(
                x: 5,
                y: y + labelsHeight * i
                );

            Rect labelRect = new Rect(suggestionPos.x, suggestionPos.y, Screen.width - 130, labelsHeight);

            if (currentSelectedSuggestion == i)
                GUI.Label(labelRect, label, suggestionSkin);
            else
                GUI.Label(labelRect, label);
        }
        ChooseSuggestion(y);

        GUI.EndScrollView();
    }

    private void HandleLatestInput()
    {
        // automaticaly set the input to upper case
        if (char.IsLower(input[input.Length - 1]))
            input = input.ToUpper();

        // close the console if the player presses the button
        if (input == "²") OnToggleConsole();

        // Check if the player pressed Quote or Return
        if (Event.current.isKey)
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.Return:
                    OnReturn();
                    break;

                case KeyCode.Quote:
                    OnToggleConsole();
                    break;

                case KeyCode.Escape:
                    OnToggleConsole();
                    break;

                case KeyCode.Backspace:
                    currentSelectedSuggestion = 0;
                    suggestionsScroll = Vector2.zero;
                    break;
            }
        }
    }

    private void ChooseSuggestion(float height)
    {

        if (Event.current.isKey && Event.current.keyCode == KeyCode.DownArrow)
        {
            currentSelectedSuggestion += 1;
            if (currentSelectedSuggestion > suggestions.Count - 1)
            {
                currentSelectedSuggestion = 0;
                suggestionsScroll.y = 0;
            }
            else suggestionsScroll.y += labelsHeight;
        }

        if (Event.current.isKey && Event.current.keyCode == KeyCode.UpArrow)
        {
            currentSelectedSuggestion -= 1;
            if (currentSelectedSuggestion < 0)
            {
                currentSelectedSuggestion = suggestions.Count - 1;
                suggestionsScroll.y = (suggestions.Count - 1) * labelsHeight;
            }
            else suggestionsScroll.y -= labelsHeight;
            //GUI.ScrollTo(suggestionsRect[currentSelectedSuggestion]);
        }


        if (Event.current.isKey && Event.current.keyCode == KeyCode.Tab && currentSelectedSuggestion < suggestions.Count)
        {
            input = suggestions[currentSelectedSuggestion];
            
            editor.text = input;
   
            int textIdx = input.Length - 1;
            int idxOfLastOpeningSymbol = 0;
            while (textIdx >= 0)
            {
                if (input[textIdx] == '<') idxOfLastOpeningSymbol = textIdx;
                textIdx -= 1;
            }

            editor.cursorIndex = idxOfLastOpeningSymbol;
            editor.selectIndex = input.Length;
        }
    }
}

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

    private DebugCommand KILLSELF;
    private DebugCommand KILLALL;
    private DebugCommand<int> KILL;

    private DebugCommand SKIP_TUTO;
    private DebugCommand RESTART_IN_TUTO;
    private DebugCommand FORCEOPEN_GYMNASIUM;

    private DebugCommand FORCEWIN;
    private DebugCommand FORCEKILL_BOSS;

    private DebugCommand<int> SWITCH_CHARACTER;

    private DebugCommand<int> ADD_KEYCARD;

    private DebugCommand<float> HEAL_SELF;
    private DebugCommand<float, bool> HEAL_SELF_C;

    private DebugCommand<float> DAMAGE_SELF;
    private DebugCommand<float, bool> DAMAGE_SELF_C;

    private DebugCommand REMOVE_ALL_MODIFIERS;

    private DebugCommand<float> ADDM_SELF_HP;
    private DebugCommand<float, float> ADDM_SELF_HP_T;
    private DebugCommand<float> ADDM_SELF_DAMAGES;
    private DebugCommand<float, float> ADDM_SELF_DAMAGES_T;
    private DebugCommand<float> ADDM_SELF_ATTRANGE;
    private DebugCommand<float, float> ADDM_SELF_ATTRANGE_T;
    private DebugCommand<float> ADDM_SELF_ATTCD;
    private DebugCommand<float, float> ADDM_SELF_ATTCD_T;
    private DebugCommand<float> ADDM_SELF_SPEED;
    private DebugCommand<float, float> ADDM_SELF_SPEED_T;
    private DebugCommand<int> ADDM_SELF_CRIT;
    private DebugCommand<int, float> ADDM_SELF_CRIT_T;
    private DebugCommand<float> ADDM_SELF_DASHCD;
    private DebugCommand<float, float> ADDM_SELF_DASHCD_T;
    private DebugCommand<float> ADDM_SELF_SKILLCD;
    private DebugCommand<float, float> ADDM_SELF_SKILLCD_T;

    private DebugCommand GOLD_BAG;
    private DebugCommand PAYDAY;
    private DebugCommand RICH_AF;
    private DebugCommand<int> ADD_MONEY;

    private DebugCommand SHOW_FPS;

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

            KILLSELF,
            KILLALL,
            KILL,

            FORCEWIN,
            FORCEKILL_BOSS,

            SKIP_TUTO,
            RESTART_IN_TUTO,
            FORCEOPEN_GYMNASIUM,

            SWITCH_CHARACTER,

            ADD_KEYCARD,

            HEAL_SELF,
            HEAL_SELF_C,

            DAMAGE_SELF,
            DAMAGE_SELF_C,

            REMOVE_ALL_MODIFIERS,

            ADDM_SELF_HP,
            ADDM_SELF_HP_T,

            ADDM_SELF_DAMAGES,
            ADDM_SELF_DAMAGES_T,

            ADDM_SELF_ATTRANGE,
            ADDM_SELF_ATTRANGE_T,

            ADDM_SELF_ATTCD,
            ADDM_SELF_ATTCD_T,

            ADDM_SELF_SPEED,
            ADDM_SELF_SPEED_T,

            ADDM_SELF_CRIT,
            ADDM_SELF_CRIT_T,

            ADDM_SELF_DASHCD,
            ADDM_SELF_DASHCD_T,

            ADDM_SELF_SKILLCD,
            ADDM_SELF_SKILLCD_T,

            ADD_MONEY,
            GOLD_BAG,
            PAYDAY,
            RICH_AF,

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

        KILLSELF = new DebugCommand("KILL_SELF", "Kills the currently controlled character", "KILL_SELF", () =>
        {
            GameManager.Player1Ref.OnTakeDamages(GameManager.Player1Ref.MaxHP_M, null);
        });

        KILLALL = new DebugCommand("KILL_ALL", "Kills every players", "KILL_ALL", () =>
        {
            foreach (var item in DataKeeper.Instance.playersDataKeep)
            {
                PlayerCharacter pc = item.playerInput.GetComponentInParent<PlayerCharacter>();
                pc.OnTakeDamages(pc.MaxHP_M, null);
            }
        });

        FORCEWIN = new DebugCommand("FORCE_WIN", "Forces the win conditions", "FORCE_WIN", () =>
        {
            allowGameChange = false;
            GameManager.Instance.GameState = GameManager.E_GameState.Win;
        });

        FORCEKILL_BOSS = new DebugCommand("FORCEKILL_BOSS", "Instantly kills the boss", "FORCEKILL_BOSS", () =>
        {
            FindObjectOfType<BossZombie>().OnTakeDamages(1000000, null);
        });

        REMOVE_ALL_MODIFIERS = new DebugCommand("REMOVE_ALL_MODIFIERS", "Removes every modifiers of self", "REMOVE_ALL_MODIFIERS", () =>
        {
            GameManager.Player1Ref.RemoveModifiersAll();
        });

        GOLD_BAG = new DebugCommand("GOLD_BAG", "Adds 50 gold", "GOLD_BAG", () =>
        {
            PlayerCharacter.AddMoney(50);
        });

        PAYDAY = new DebugCommand("PAYDAY", "Adds 200 gold", "PAYDAY", () =>
        {
            PlayerCharacter.AddMoney(200);
        });

        RICH_AF = new DebugCommand("RICH_AF", "Adds 5000 gold", "RICH_AF", () =>
        {
            PlayerCharacter.AddMoney(5000);
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
            FPSDisplayer displayer = FindObjectOfType<FPSDisplayer>();
            displayer.SetState(!displayer.IsRunning());
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
        SWITCH_CHARACTER = new DebugCommand<int>("SWITCH_CHARACTER", "Switchs to the desired character \n 0 = Shirley \n 1 = Whitney \n 2 = Jason \n 3 = Nelson", "SWITCH_CHARACTER <int>", (val) =>
        {
            GameManager.E_CharactersNames desiredCharacter = GameManager.E_CharactersNames.Shirley;

            if (0 < val && val < 4) desiredCharacter = (GameManager.E_CharactersNames)val;

            PlayersManager.PlayerCharacterComponents newPCC = PlayersManager.Instance.GetCharacterComponents(desiredCharacter);

            foreach (var item in GameManager.Instance.playersByName)
            {
                item.playerScript.SwitchCharacter(newPCC);
            }
        });

        KILL = new DebugCommand<int>("KILL", "Kills the given player index", "KILL <int>", (val) =>
        {
            PlayerCharacter pc = DataKeeper.Instance.playersDataKeep[val].playerInput.GetComponentInParent<PlayerCharacter>();
            pc.OnTakeDamages(pc.MaxHP_M, null);
        });

        ADDM_SELF_CRIT = new DebugCommand<int>("ADDM_SELF_CRIT", "Adds a crit chances modifier of <int>% to self", "ADDM_SELF_CRIT <float>", (val) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_CRIT_ID, val, StatsModifier.E_StatType.CritChances);
        });

        ADDM_SELF_CRIT_T = new DebugCommand<int, float>("ADDM_SELF_CRIT", "Adds a crit chances modifier of <int>% to self for <float>s", "ADDM_SELF_CRIT <int> <float>", (val_1, val_2) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_CRIT_ID, val_1, val_2, StatsModifier.E_StatType.CritChances);
        });

        ADD_MONEY = new DebugCommand<int>("ADD_MONEY", "Adds <int> money", "ADD_MONEY <int>", (val) =>
        {
            PlayerCharacter.AddMoney(val);
        });

        ADD_KEYCARD = new DebugCommand<int>("ADD_KEYCARD", "Adds <int> keycards", "ADD_KEYCARD <int>", (val) =>
        {
            GameManager.Instance.AcquiredCards += val;
        });
    }

    private void CreateFloatCommands()
    {
        HEAL_SELF = new DebugCommand<float>("HEAL_SELF", "Heals the currently played character", "HEAL_SELF <float>", (val) =>
        {
            GameManager.Player1Ref.OnHeal(val);
        });

        HEAL_SELF_C = new DebugCommand<float, bool>("HEAL_SELF", "Heals the currently played character", "HEAL_SELF <float> <bool>", (val_1, val_2) =>
        {
            GameManager.Player1Ref.OnHeal(val_1, val_2);
        });

        DAMAGE_SELF = new DebugCommand<float>("DAMAGE_SELF", "Damages the currently played character", "DAMAGE_SELF <float>", (val) =>
        {
            GameManager.Player1Ref.OnTakeDamages(val, null);
        });

        DAMAGE_SELF_C = new DebugCommand<float, bool>("DAMAGE_SELF", "Damages the currently played character", "DAMAGE_SELF <float> <bool>", (val_1, val_2) =>
        {
            GameManager.Player1Ref.OnTakeDamages(val_1, null, val_2);
        });

        ADDM_SELF_HP = new DebugCommand<float>("ADDM_SELF_HP", "Adds a HP modifier of <float> to self", "ADDM_SELF_HP <float>", (val) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_HEALTH_ID, val, StatsModifier.E_StatType.MaxHP);
        });

        ADDM_SELF_HP_T = new DebugCommand<float, float>("ADDM_SELF_HP", "Adds a HP modifier of <float> to self for <float>s", "ADDM_SELF_HP <float> <float>", (val_1, val_2) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_HEALTH_ID, val_1, val_2, StatsModifier.E_StatType.MaxHP);
        });

        ADDM_SELF_DAMAGES = new DebugCommand<float>("ADDM_SELF_DAMAGES", "Adds a damages modifier of <float> to self", "ADDM_SELF_DAMAGES <float>", (val) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_DAMAGES_ID, val, StatsModifier.E_StatType.Damages);
        });

        ADDM_SELF_DAMAGES_T = new DebugCommand<float, float>("ADDM_SELF_DAMAGES", "Adds a damages modifier of <float> to self for <float>s", "ADDM_SELF_DAMAGES <float> <float>", (val_1, val_2) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_DAMAGES_ID, val_1, val_2, StatsModifier.E_StatType.Damages);
        });

        ADDM_SELF_ATTRANGE = new DebugCommand<float>("ADDM_SELF_ATTRANGE", "Adds a attack range modifier of <float> to self", "ADDM_SELF_ATTRANGE <float>", (val) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_RANGE_ID, val, StatsModifier.E_StatType.AttackRange);
        });

        ADDM_SELF_ATTRANGE_T = new DebugCommand<float, float>("ADDM_SELF_ATTRANGE", "Adds a attack range modifier of <float> to self for <float>s", "ADDM_SELF_ATTRANGE <float> <float>", (val_1, val_2) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_RANGE_ID, val_1, val_2, StatsModifier.E_StatType.AttackRange);
        });

        ADDM_SELF_ATTCD = new DebugCommand<float>("ADDM_SELF_ATTCD", "Adds a attack cooldown modifier of <float> to self", "ADDM_SELF_ATTCD <float>", (val) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_CD_ID, val, StatsModifier.E_StatType.Attack_CD);
        });

        ADDM_SELF_ATTCD_T = new DebugCommand<float, float>("ADDM_SELF_ATTCD", "Adds a attack cooldown modifier of <float> to self for <float>s", "ADDM_SELF_ATTCD <float> <float>", (val_1, val_2) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_CD_ID, val_1, val_2, StatsModifier.E_StatType.Attack_CD);
        });

        ADDM_SELF_SPEED = new DebugCommand<float>("ADDM_SELF_SPEED", "Adds a speed modifier of <float> to self", "ADDM_SELF_SPEED <float>", (val) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_SPEED_ID, val, StatsModifier.E_StatType.Speed);
        });

        ADDM_SELF_SPEED_T = new DebugCommand<float, float>("ADDM_SELF_SPEED", "Adds a speed modifier of <float> to self for <float>s", "ADDM_SELF_SPEED <float> <float>", (val_1, val_2) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_SPEED_ID, val_1, val_2, StatsModifier.E_StatType.Speed);
        });

        ADDM_SELF_DASHCD = new DebugCommand<float>("ADDM_SELF_DASHCD", "Adds a dash cooldown modifier of <float> to self", "ADDM_SELF_DASHCD <float>", (val) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_DASHCD_ID, val, StatsModifier.E_StatType.DASH_CD);
        });

        ADDM_SELF_DASHCD_T = new DebugCommand<float, float>("ADDM_SELF_DASHCD_T", "Adds a dash cooldown modifier of <float> to self for <float>s", "ADDM_SELF_DASHCD_T <float> <float>", (val_1, val_2) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_DASHCD_ID, val_1, val_2, StatsModifier.E_StatType.DASH_CD);
        });

        ADDM_SELF_SKILLCD = new DebugCommand<float>("ADDM_SELF_SKILLCD", "Adds a skill cooldown modifier of <float> to self", "ADDM_SELF_SKILLCD <float>", (val) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_SKILLCD_ID, val, StatsModifier.E_StatType.SKILL_CD);
        });

        ADDM_SELF_SKILLCD_T = new DebugCommand<float, float>("ADDM_SELF_SKILLCD_T", "Adds a speed modifier of <float> to self for <float>s", "ADDM_SELF_SKILLCD_T <float> <float>", (val_1, val_2) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_SKILLCD_ID, val_1, val_2, StatsModifier.E_StatType.SKILL_CD);
        });
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
                    (command as DebugCommand<bool>).Invoke(ParseBool(proprieties[1]));
                    input = "";
                    return;
                }

                else if (command as DebugCommand<int> != null && proprieties.Length == 2)
                {
                    (command as DebugCommand<int>).Invoke(int.Parse(proprieties[1]));
                    input = "";
                    return;
                }

                else if (command as DebugCommand<float, bool> != null && proprieties.Length == 3)
                {
                    (command as DebugCommand<float, bool>).Invoke(float.Parse(proprieties[1]), ParseBool(proprieties[2]));
                    input = "";
                    return;
                }

                else if (command as DebugCommand<float, float> != null && proprieties.Length == 3)
                {
                    (command as DebugCommand<float, float>).Invoke(float.Parse(proprieties[1]), float.Parse(proprieties[2]));
                    input = "";
                    return;
                }

                else if (command as DebugCommand<float> != null && proprieties.Length == 2)
                {
                    (command as DebugCommand<float>).Invoke(float.Parse(proprieties[1]));
                    input = "";
                    return;
                }
            }
        }

        input = "";
    }

    private bool ParseBool(string propriety)
    {
        if (propriety.Equals("<bool>")) return true;
        if (propriety.Equals("1") || propriety.Equals("TRUE")) return true;
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
            while (textIdx >= 0 && input[textIdx] != '<')
            {
                textIdx -= 1;
            }

            editor.cursorIndex = textIdx;
            editor.selectIndex = input.Length;
        }
    }
}

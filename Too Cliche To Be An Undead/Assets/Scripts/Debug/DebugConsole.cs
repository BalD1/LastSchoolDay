using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class DebugConsole : MonoBehaviour
{
    private const string MODIF_HEALTH_ID = "DC_CHEAT_HEALTH_X";
    private const string MODIF_DAMAGES_ID = "DC_CHEAT_DAMAGES_X";
    private const string MODIF_RANGE_ID = "DC_CHEAT_RANGE_X";
    private const string MODIF_CD_ID = "DC_CHEAT_CD_X";
    private const string MODIF_SPEED_ID = "DC_CHEAT_SPEED_X";
    private const string MODIF_CRIT_ID = "DC_CHEAT_CRIT_X";

    private bool showConsole;
    private bool showHelp;

    private string input;

    private int currentSelectedSuggestion;

    private DebugCommand HELP;

    private DebugCommand KILLSELF;
    private DebugCommand KILLALL;
    private DebugCommand<int> KILL;

    private DebugCommand FORCEWIN;

    private DebugCommand<float> HEAL_SELF;
    private DebugCommand<float, bool> HEAL_SELF_C;

    private DebugCommand<float> DAMAGE_SELF;
    private DebugCommand<float, bool> DAMAGE_SELF_C;

    private DebugCommand REMOVE_ALL_MODIFIERS;

    private DebugCommand<float> ADDM_SELF_HP;
    private DebugCommand<float, float> ADDM_SELF_HP_T;
    private DebugCommand<float> ADDM_SELF_DAMAGES;
    private DebugCommand<float,float> ADDM_SELF_DAMAGES_T;
    private DebugCommand<float> ADDM_SELF_ATTRANGE;
    private DebugCommand<float,float> ADDM_SELF_ATTRANGE_T;
    private DebugCommand<float> ADDM_SELF_ATTCD;
    private DebugCommand<float,float> ADDM_SELF_ATTCD_T;
    private DebugCommand<float> ADDM_SELF_SPEED;
    private DebugCommand<float,float> ADDM_SELF_SPEED_T;
    private DebugCommand<int> ADDM_SELF_CRIT;
    private DebugCommand<int, float> ADDM_SELF_CRIT_T;

    private DebugCommand<int> TEST_INT;

    private Vector2 helpScroll;
    private Vector2 suggestionsScroll;

    private List<string> suggestions = new List<string>();
    private List<Rect> suggestionsRect = new List<Rect>();

    private List<object> commandList = new List<object>();

    public GUIStyle suggestionSkin;
    public GUIStyle greyedText;

    private void Awake()
    {
        // Create commands

        #region Simple commands

        HELP = new DebugCommand("HELP", "Shows all commands", "HELP", () =>
        {
            showHelp = !showHelp;
        });

        KILLSELF = new DebugCommand("KILL_SELF", "Kills the currently controlled character", "KILL_SELF", () =>
        {
            GameManager.Player1Ref.OnTakeDamages(GameManager.Player1Ref.GetStats.MaxHP(GameManager.Player1Ref.StatsModifiers));
        });

        KILLALL = new DebugCommand("KILL_ALL", "Kills every players", "KILL_ALL", () =>
        {
            foreach (var item in DataKeeper.Instance.playersDataKeep)
            {
                PlayerCharacter pc = item.playerInput.GetComponentInParent<PlayerCharacter>();
                pc.OnTakeDamages(pc.GetStats.MaxHP(pc.StatsModifiers));
            }
        });

        FORCEWIN = new DebugCommand("FORCE_WIN", "Forces the win conditions", "FORCE_WIN", () =>
        {
            GameManager.Instance.GameState = GameManager.E_GameState.Win;
        });

        REMOVE_ALL_MODIFIERS = new DebugCommand("REMOVE_ALL_MODIFIERS", "Removes every modifiers of self", "REMOVE_ALL_MODIFIERS", () =>
        {
            GameManager.Player1Ref.StatsModifiers.Clear();
        });

        #endregion

        #region Int commands

        KILL = new DebugCommand<int>("KILL", "Kills the given player index", "KILL <int>", (val) =>
        {
            PlayerCharacter pc = DataKeeper.Instance.playersDataKeep[val].playerInput.GetComponentInParent<PlayerCharacter>();
            pc.OnTakeDamages(pc.GetStats.MaxHP(pc.StatsModifiers));
        });

        ADDM_SELF_CRIT = new DebugCommand<int>("ADDM_SELF_CRIT", "Adds a crit chances modifier of <int>% to self", "ADDM_SELF_CRIT <float>", (val) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_CRIT_ID, val, StatsModifier.E_StatType.CritChances);
        });

        ADDM_SELF_CRIT_T = new DebugCommand<int, float>("ADDM_SELF_CRIT", "Adds a crit chances modifier of <int>% to self for <float>s", "ADDM_SELF_CRIT <int> <float>", (val_1, val_2) =>
        {
            GameManager.Player1Ref.AddModifier(MODIF_CRIT_ID, val_1, val_2, StatsModifier.E_StatType.CritChances);
        });

        #endregion

        #region Float commands

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
            GameManager.Player1Ref.OnTakeDamages(val);
        });

        DAMAGE_SELF_C = new DebugCommand<float, bool>("DAMAGE_SELF", "Damages the currently played character", "DAMAGE_SELF <float> <bool>", (val_1, val_2) =>
        {
            GameManager.Player1Ref.OnTakeDamages(val_1, val_2);
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

        #endregion

        commandList = new List<object>()
        {
            HELP,
            KILLSELF,
            KILLALL,
            KILL,

            FORCEWIN,

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
    };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Quote))
            OnToggleConsole();
        if (Input.GetKeyDown(KeyCode.Return))
            OnReturn();
    }

    public void OnToggleConsole()
    {
        showConsole = !showConsole;

        if (showConsole)
        {
            ResetField();
            GameManager.Player1Ref.SetInGameControlsState(false);
        }
        else
        {
            GameManager.Player1Ref.SetInGameControlsState(true);
        }
    }

    public void OnReturn()
    {
        // if the command field is displayed, check the input and reset
        if (showConsole)
        {
            HandleInput();
            input = "";
            showConsole = false;
            showHelp = false;
        }
    }

    /*
    public void OnToggleConsole(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        showConsole = !showConsole;

        if (showConsole)
        {
            ResetField();
            GameManager.PlayerRef.SetInGameControlsState(false);
        }
        else
        {
            GameManager.PlayerRef.SetInGameControlsState(true);
        }
    }

    public void OnReturn(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // if the command field is displayed, check the input and reset
        if (showConsole)
        {
            HandleInput();
            input = "";
        }
    }
    */

    private void ResetField()
    {
        showHelp = false;
        input = "";
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
        bool res = false;

        if (propriety.Equals("1") || propriety.Equals("TRUE")) res = true;

        return res;
    }

    private void OnGUI()
    {
        if (!showConsole) return;

        Color baseColor = GUI.backgroundColor;

        float y = 0f;

        Rect helpBoxRect = new Rect(0, y, Screen.width, 100);
        if (showHelp)
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

                Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);

                GUI.Label(labelRect, label);
            }

            GUI.EndScrollView();

            y += 100;
        }

        // draw the input field
        Rect inputBoxRect = new Rect(0, y, Screen.width, 30);
        GUI.Box(inputBoxRect, "");

        GUI.backgroundColor = new Color(0, 0, 0, 0);
        GUI.SetNextControlName("InputField");
        Rect inputRect = new Rect(10f, y + 5f, Screen.width - 20f, 20f);
        input = GUI.TextField(inputRect, input);
        GUI.FocusControl("InputField");

        if (input == "" || input == null) return;

        if (char.IsLower(input[input.Length - 1]))
            input = input.ToUpper();

        
        if (input == "²")
        {
            showConsole = false;
            ResetField();
            GameManager.Player1Ref.SetInGameControlsState(true);
        }

        if (Event.current.isKey)
        {
            if (Event.current.keyCode == KeyCode.Return)
                HandleInput();
            if (Event.current.keyCode == KeyCode.Quote)
            {
                showConsole = false;
                ResetField();
                GameManager.Player1Ref.SetInGameControlsState(true);
            }
        }
        

        // draw the box
        GUI.backgroundColor = baseColor;

        float height = y + inputBoxRect.height;

        Rect suggestionsBoxRect = new Rect(0, height, Screen.width, 60);
        GUI.Box(suggestionsBoxRect, "");

        Rect suggestionsViewport = new Rect(0, height, Screen.width - 30, 20 * commandList.Count);
        suggestionsScroll = GUI.BeginScrollView(new Rect(0, height + 5f, Screen.width, 50), suggestionsScroll, suggestionsViewport);

        suggestions = new List<string>();
        suggestionsRect = new List<Rect>();

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase command = commandList[i] as DebugCommandBase;
            if (command.CommandID.StartsWith(input) == false) continue;

            suggestions.Add(command.CommandFormat);
            suggestionsRect.Add(new Rect(5, (y + suggestionsRect.Count) + 20 * (i + 1), suggestionsViewport.width - 100, 20));
        }

        for (int i = 0; i < suggestions.Count; i++)
        {
            string label = suggestions[i];

            Rect labelRect = new Rect(5, y + 20 * (i + 2), suggestionsViewport.width - 100, 20);

            if (currentSelectedSuggestion == i)
                GUI.Label(labelRect, label, suggestionSkin);
            else
                GUI.Label(labelRect, label);
        }
        ChoseSuggestion(height);

        GUI.EndScrollView();
    }

    private void ChoseSuggestion(float height)
    {
        if (Event.current.isKey && Event.current.keyCode == KeyCode.DownArrow)
        {
            currentSelectedSuggestion += 1;
            if (currentSelectedSuggestion > suggestions.Count - 1) currentSelectedSuggestion = 0;
            GUI.ScrollTo(suggestionsRect[currentSelectedSuggestion]);
        }

        if (Event.current.isKey && Event.current.keyCode == KeyCode.UpArrow)
        {
            currentSelectedSuggestion -= 1;
            if (currentSelectedSuggestion < 0) currentSelectedSuggestion = suggestions.Count - 1;
            GUI.ScrollTo(suggestionsRect[currentSelectedSuggestion]);
        }


        if (Event.current.isKey && Event.current.keyCode == KeyCode.Tab && currentSelectedSuggestion < suggestions.Count)
            input = suggestions[currentSelectedSuggestion];
    }
}

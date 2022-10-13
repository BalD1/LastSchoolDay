using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugConsole : MonoBehaviour
{
    private bool showConsole;
    private bool showHelp;

    private string input;

    private int currentSelectedSuggestion;

    private DebugCommand HELP;

    private DebugCommand KILLSELF;

    private DebugCommand<float> HEAL_SELF;
    private DebugCommand<float, bool> HEAL_SELF_C;

    private DebugCommand<float> DAMAGE_SELF;
    private DebugCommand<float, bool> DAMAGE_SELF_C;

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

        // SIMPLE COMMANDS

        HELP = new DebugCommand("HELP", "Shows all commands", "HELP", () =>
        {
            showHelp = !showHelp;
        });

        KILLSELF = new DebugCommand("KILL_SELF", "Kills the currently controlled character", "KILL_SELF", () =>
        {
            GameManager.PlayerRef.OnTakeDamages(GameManager.PlayerRef.GetStats.MaxHP);
        });

        // INT COMMANDS



        // FLOAT COMMANDS

        HEAL_SELF = new DebugCommand<float>("HEAL_SELF", "Heals the currently played character", "HEAL_SELF <float>", (val) =>
        {
            GameManager.PlayerRef.OnHeal(val);
        });

        HEAL_SELF_C = new DebugCommand<float, bool>("HEAL_SELF", "Heals the currently played character", "HEAL_SELF <float> <bool>", (val_1, val_2) =>
        {
            GameManager.PlayerRef.OnHeal(val_1, val_2);
        });

        DAMAGE_SELF = new DebugCommand<float>("DAMAGE_SELF", "Damages the currently played character", "DAMAGE_SELF <float>", (val) =>
        {
            GameManager.PlayerRef.OnTakeDamages(val);
        });

        DAMAGE_SELF_C = new DebugCommand<float, bool>("DAMAGE_SELF", "Damages the currently played character", "DAMAGE_SELF <float> <bool>", (val_1, val_2) =>
        {
            GameManager.PlayerRef.OnTakeDamages(val_1, val_2);
        });

        commandList = new List<object>()
        {
            HELP,
            KILLSELF,
            HEAL_SELF,
            HEAL_SELF_C,
            DAMAGE_SELF,
            DAMAGE_SELF_C,
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
            GameManager.PlayerRef.SetInGameControlsState(false);
        }
        else
        {
            GameManager.PlayerRef.SetInGameControlsState(true);
        }
    }

    public void OnReturn()
    {
        // if the command field is displayed, check the input and reset
        Debug.Log(showConsole);
        if (showConsole)
        {
            HandleInput();
            input = "";
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
    */
    /*
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
                    return;
                }

                else if (command as DebugCommand<int> != null && proprieties.Length == 2)
                {
                    (command as DebugCommand<int>).Invoke(int.Parse(proprieties[1]));
                    return;
                }

                else if (command as DebugCommand<float, bool> != null && proprieties.Length == 3)
                {
                    (command as DebugCommand<float, bool>).Invoke(float.Parse(proprieties[1]), ParseBool(proprieties[2]));
                    return;
                }

                else if (command as DebugCommand<float> != null && proprieties.Length == 2)
                {
                    (command as DebugCommand<float>).Invoke(float.Parse(proprieties[1]));
                    return;
                }
            }
        }
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

        if (Event.current.isKey)
        {
            if (Event.current.keyCode == KeyCode.Return)
                HandleInput();
            if (Event.current.keyCode == KeyCode.BackQuote)
            {
                showConsole = false;
                ResetField();
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

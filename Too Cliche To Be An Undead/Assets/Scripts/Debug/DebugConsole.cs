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
    private DebugCommand TEST_1;
    private DebugCommand TEST_2;
    private DebugCommand TEST_3;
    private DebugCommand TEST_4;
    private DebugCommand TEST_5;
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
            Debug.Log("Test");
        });

        TEST_1 = new DebugCommand("TEST_1", "Shows all commands", "TEST_1", () =>
        {
            Debug.Log("Test 1");
        });

        TEST_2 = new DebugCommand("TEST_2", "Shows all commands", "TEST_2", () =>
        {
            Debug.Log("Test 2");
        });

        TEST_3 = new DebugCommand("TEST_3", "Shows all commands", "TEST_3", () =>
        {
            Debug.Log("Test 3");
        });

        TEST_4 = new DebugCommand("TEST_4", "Shows all commands", "TEST_4", () =>
        {
            Debug.Log("Test 4");
        });

        TEST_5 = new DebugCommand("TEST_5", "Shows all commands", "TEST_5", () =>
        {
            Debug.Log("Test 5");
        });

        // INT COMMANDS


        TEST_INT = new DebugCommand<int>("TEST_INT", "Test int DESCR", "TEST_INT <int>", (val) =>
        {
            Debug.Log("Test int" + val);
        });

        commandList = new List<object>()
        {
            HELP,
            KILLSELF,
            TEST_INT,
            TEST_1,
            TEST_2,
            TEST_3,
            TEST_4,
            TEST_5,
        };
    }

    public void OnToggleConsole(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        showConsole = !showConsole;

        if (showConsole)
        {
            ResetField();
            GameManager.Instance.PlayerRef.SetInGameControlsState(false);
        }
        else
        {
            GameManager.Instance.PlayerRef.SetInGameControlsState(true);
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
            if (input.Contains(command.CommandID))
            {
                // check the command type
                if (command as DebugCommand != null)
                    (command as DebugCommand).Invoke();
                else if (command as DebugCommand<int> != null)
                    (command as DebugCommand<int>).Invoke(int.Parse(proprieties[1]));
            }
        }
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

        if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
            HandleInput();

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

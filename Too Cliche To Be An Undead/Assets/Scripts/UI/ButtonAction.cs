using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAction : MonoBehaviour
{
    public void ChangeGameState(ButtonArgs_GameState buttonArgs) => GameManager.Instance.GameState = buttonArgs.GetArgs;

    public void ChangeScene(ButtonArgs_Scene buttonArgs) => GameManager.ChangeScene(buttonArgs.GetArgs);

    public void Quit() => Application.Quit();
}

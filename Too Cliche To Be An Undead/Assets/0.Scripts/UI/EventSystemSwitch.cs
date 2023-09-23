using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class EventSystemSwitch : MonoBehaviour
{
    private static EventSystemSwitch instance;
    public static EventSystemSwitch Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] private InputSystemUIInputModule baseUIModule;
}

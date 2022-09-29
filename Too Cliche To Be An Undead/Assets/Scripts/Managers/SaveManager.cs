using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BalDUtilities.Misc;

public class SaveManager : MonoBehaviour
{
    private static SaveManager instance;
    public static SaveManager Instance
    {
        get
        {
            if (instance == null) Debug.LogError("SaveManager instance was not found.");
            return instance;
        }
    }

    public enum E_SaveKeys
    {
        // floats

        F_MasterVolume,
        F_MusicVolume,
        F_SFXVolume,

        // ints

        // bools

        // strings
    }

    private void Awake()
    {
        instance = this;
        InitiateKeys();
    }

    private void InitiateKeys()
    {
        if (!PlayerPrefs.HasKey(EnumsExtension.EnumToString(E_SaveKeys.F_MasterVolume)))
            SetSavedKey(E_SaveKeys.F_MasterVolume, .5f);

        if (!PlayerPrefs.HasKey(EnumsExtension.EnumToString(E_SaveKeys.F_MusicVolume)))
            SetSavedKey(E_SaveKeys.F_MusicVolume, .5f);

        if (!PlayerPrefs.HasKey(EnumsExtension.EnumToString(E_SaveKeys.F_SFXVolume)))
            SetSavedKey(E_SaveKeys.F_SFXVolume, .5f);
    }

    /*
     * SETTERS
     * Les SetSavedKey sauvegardent la valeur donnée avec "value" avec une clé "key"
     */

    public static void SetSavedKey(E_SaveKeys key, int value) => PlayerPrefs.SetInt(EnumsExtension.EnumToString(key), value);
    public static void SetSavedKey(E_SaveKeys key, bool value)
    {
        int ival = value ? 1 : 0;
        PlayerPrefs.SetInt(EnumsExtension.EnumToString(key), ival);
    }
    public static void SetSavedKey(E_SaveKeys key, float value) => PlayerPrefs.SetFloat(EnumsExtension.EnumToString(key), value);
    public static void SetSavedKey(E_SaveKeys key, string value) => PlayerPrefs.SetString(EnumsExtension.EnumToString(key), value);

    /*
    * GETTERS
    * Les GetSavedKey retournent la valeur sauvegardée avec la clé "key"
    */

    public static int GetSavedIntKey(E_SaveKeys key) => PlayerPrefs.GetInt(EnumsExtension.EnumToString(key));
    public static bool GetSavedBoolKey(E_SaveKeys key) => PlayerPrefs.GetInt(EnumsExtension.EnumToString(key)) == 1 ? true : false;
    public static float GetSavedFloatKey(E_SaveKeys key) => PlayerPrefs.GetFloat(EnumsExtension.EnumToString(key));
    public static string GetSavedStringKey(E_SaveKeys key) => PlayerPrefs.GetString(EnumsExtension.EnumToString(key));

    public static void DeleteKey(E_SaveKeys key) => PlayerPrefs.DeleteKey(EnumsExtension.EnumToString(key));
}

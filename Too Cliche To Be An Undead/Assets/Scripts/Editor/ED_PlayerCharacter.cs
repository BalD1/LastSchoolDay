using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BalDUtilities.EditorUtils;
using BalDUtilities.Misc;
using static UnityEditor.Progress;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;

[CustomEditor(typeof(PlayerCharacter))]
public class ED_PlayerCharacter : Editor
{
    private PlayerCharacter targetScript;

    private bool showDefaultInspector;
    private bool showComponents = true;
    private bool showState = true;
    private bool showAudio = true;
    private bool showStats = true;
    private bool showModifiers = false;
    private bool showMisc = true;

    private bool showSrptStats;
    private bool showScrptAudio;
    private bool showTickDamages;
    private bool showHurtAudio;
    private bool showDeathAudio;
    private bool showAttackAudio;

    private float damagesAmount = 50;
    private bool critDamages;
    private float healAmount = 50;
    private bool critHeal;

    private enum E_PlayerStates
    {
        Idle,
        Moving,
    }
    private E_PlayerStates stateToForce;

    private void OnEnable()
    {
        targetScript = (PlayerCharacter)target;
    }

    public override void OnInspectorGUI()
    {
        showDefaultInspector = EditorGUILayout.Toggle("Show Default Inspector", showDefaultInspector);

        if (showDefaultInspector)
        {
            ReadOnlyDraws.EditorScriptDraw(typeof(ED_PlayerCharacter), this);
            base.OnInspectorGUI();
            return;
        }

        ReadOnlyDraws.EditorScriptDraw(typeof(ED_PlayerCharacter), this);
        ReadOnlyDraws.ScriptDraw(typeof(PlayerCharacter), targetScript, true);

        DrawComponents();
        DrawState();
        DrawAudio();
        DrawStats();
        DrawMisc();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawComponents()
    {
        EditorGUILayout.Space(5);
        if (GUILayout.Button("Components", ButtonToLabelStyle())) showComponents = !showComponents;
        if (!showComponents) return;

        EditorGUILayout.BeginVertical("GroupBox");

        SerializedProperty rb = serializedObject.FindProperty("rb");
        EditorGUILayout.PropertyField(rb);

        SerializedProperty sprite = serializedObject.FindProperty("sprite");
        EditorGUILayout.PropertyField(sprite);

        SerializedProperty hitMaterial = serializedObject.FindProperty("hitMaterial");
        EditorGUILayout.PropertyField(hitMaterial);

        SerializedProperty flashOnHitTime = serializedObject.FindProperty("flashOnHitTime");
        EditorGUILayout.PropertyField(flashOnHitTime);

        SerializedProperty animator = serializedObject.FindProperty("animator");
        EditorGUILayout.PropertyField(animator);

        SerializedProperty weapon = serializedObject.FindProperty("weapon");
        EditorGUILayout.PropertyField(weapon);

        SerializedProperty skillHolder = serializedObject.FindProperty("skillHolder");
        EditorGUILayout.PropertyField(skillHolder);

        SerializedProperty healthPopupOffset = serializedObject.FindProperty("healthPopupOffset");
        EditorGUILayout.PropertyField(healthPopupOffset);

        SerializedProperty hpBar = serializedObject.FindProperty("hpBar");
        EditorGUILayout.PropertyField(hpBar);

        SerializedProperty hpText = serializedObject.FindProperty("hpText");
        EditorGUILayout.PropertyField(hpText);

        SerializedProperty skillIcon = serializedObject.FindProperty("skillIcon");
        EditorGUILayout.PropertyField(skillIcon);

        SerializedProperty inputs = serializedObject.FindProperty("inputs");
        EditorGUILayout.PropertyField(inputs);

        EditorGUILayout.EndVertical();
    }

    private void DrawState()
    {
        EditorGUILayout.Space(5);

        if (GUILayout.Button("State", ButtonToLabelStyle())) showState = !showState;
        if (!showState) return;

        EditorGUILayout.BeginVertical("GroupBox");

        SerializedProperty stateManager = serializedObject.FindProperty("stateManager");
        EditorGUILayout.PropertyField(stateManager);

        EditorGUILayout.LabelField("Current State", targetScript.StateManager.ToString());


        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Force new state", GUILayout.MaxWidth(150)))
        {
            FSM_Player_Manager playerManager = targetScript.StateManager;
            switch (stateToForce)
            {
                case E_PlayerStates.Idle:
                    playerManager.SwitchState(playerManager.idleState);
                    break;

                case E_PlayerStates.Moving:
                    playerManager.SwitchState(playerManager.movingState);
                    break;
            }
        }
        stateToForce = (E_PlayerStates)EditorGUILayout.EnumPopup(stateToForce, GUILayout.MaxWidth(100));

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void DrawAudio()
    {
        EditorGUILayout.Space(5);
        if (GUILayout.Button("Audio", ButtonToLabelStyle())) showAudio = !showAudio;
        if (!showAudio) return;

        EditorGUILayout.BeginVertical("GroupBox");

        SerializedProperty source = serializedObject.FindProperty("source");
        EditorGUILayout.PropertyField(source);

        EditorGUILayout.BeginHorizontal();
        SerializedProperty audioClips = serializedObject.FindProperty("audioClips");
        EditorGUILayout.PropertyField(audioClips);
        if (GUILayout.Button("Edit")) PopUpAssetInspector.Create(targetScript.GetAudioClips);
        EditorGUILayout.EndHorizontal();

        GUIStyle style = new GUIStyle(EditorStyles.foldout);
        style.fixedWidth = 0;
        style.hover.textColor = Color.blue;
        EditorGUI.indentLevel++;
        showScrptAudio = EditorGUILayout.Foldout(showScrptAudio, "", style);
        EditorGUI.indentLevel--;

        if (showScrptAudio)
        {
            SCRPT_EntityAudio playerAudio = targetScript.GetAudioClips;

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical("GroupBox");

            GUIStyle labelStyle = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontStyle = FontStyle.Bold;

            GUI.enabled = false;
            EditorGUILayout.LabelField("Attack Clips");
            EditorGUI.indentLevel++;
            for (int i = 0; i < playerAudio.AttackClips.Length; i++)
            {
                EditorGUILayout.ObjectField("Clip " + i, playerAudio.AttackClips[i], typeof(AudioClip), false);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("Hurt Clips");
            EditorGUI.indentLevel++;
            for (int i = 0; i < playerAudio.HurtClips.Length; i++)
            {
                EditorGUILayout.ObjectField("Clip " + i, playerAudio.HurtClips[i], typeof(AudioClip), false);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("Death Clips");
            EditorGUI.indentLevel++;
            for (int i = 0; i < playerAudio.DeathClips.Length; i++)
            {
                EditorGUILayout.ObjectField("Clip " + i, playerAudio.DeathClips[i], typeof(AudioClip), false);
            }
            GUI.enabled = true;
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawStats()
    {
        EditorGUILayout.Space(5);

        if (GUILayout.Button("Stats", ButtonToLabelStyle())) showStats = !showStats;
        if (!showStats) return;

        EditorGUILayout.BeginVertical("GroupBox");

        EditorGUILayout.BeginHorizontal();
        SerializedProperty stats = serializedObject.FindProperty("stats");
        EditorGUILayout.PropertyField(stats);
        if (GUILayout.Button("Edit")) PopUpAssetInspector.Create(targetScript.GetStats);
        EditorGUILayout.EndHorizontal();

        GUI.enabled = false;

        EditorGUILayout.LabelField("Current HP : " + targetScript.CurrentHP + " / " + targetScript.GetStats.MaxHP(targetScript.StatsModifiers) + "(" + targetScript.CurrentHP / targetScript.GetStats.MaxHP(targetScript.StatsModifiers) * 100 + "%)");

        GUIStyle style = new GUIStyle(EditorStyles.foldout);
        style.fixedWidth = 0;
        style.hover.textColor = Color.blue;
        EditorGUI.indentLevel++;
        showSrptStats = EditorGUILayout.Foldout(showSrptStats, "", style);
        EditorGUI.indentLevel--;

        if (showSrptStats)
        {
            SCRPT_EntityStats playerStats = targetScript.GetStats;

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical("GroupBox");

            EditorGUILayout.TextField("Entity Type", playerStats.EntityType);
            EditorGUILayout.FloatField("Max HP", playerStats.MaxHP(targetScript.StatsModifiers));
            EditorGUILayout.FloatField("Base Damages", playerStats.BaseDamages(targetScript.StatsModifiers));
            EditorGUILayout.FloatField("Attack Range", playerStats.AttackRange(targetScript.StatsModifiers));
            EditorGUILayout.FloatField("Attack Cooldown", playerStats.Attack_COOLDOWN(targetScript.StatsModifiers));
            EditorGUILayout.FloatField("Invincibility Cooldown", playerStats.Invincibility_COOLDOWN);
            EditorGUILayout.FloatField("Speed", playerStats.Speed(targetScript.StatsModifiers));
            EditorGUILayout.IntField("Crit Chances", playerStats.CritChances(targetScript.StatsModifiers));
            EditorGUILayout.FloatField("Weight", playerStats.Weight);
            EditorGUILayout.TextField("Team", EnumsExtension.EnumToString(playerStats.Team));

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }

        GUI.enabled = true;

        EditorGUI.indentLevel++;
        showModifiers = EditorGUILayout.Foldout(showModifiers, "Modifiers");
        if (showModifiers && targetScript.StatsModifiers.Count > 0)
        {
            EditorGUILayout.BeginVertical("GroupBox");

            EditorGUI.indentLevel++;
            StatsModifier stM;
            for (int i = 0; i < targetScript.StatsModifiers.Count; i++)
            {
                stM = targetScript.StatsModifiers[i];

                stM.showInEditor = EditorGUILayout.Foldout(stM.showInEditor, $"Element {i} : {stM.IDName}");
                if (stM.showInEditor)
                {
                    GUI.enabled = false;

                    EditorGUILayout.EnumPopup("Type", stM.StatType);
                    if (stM.MaxDuration > 0) EditorGUILayout.TextField("Duration ", $"{stM.Timer} / {stM.MaxDuration}");
                    else EditorGUILayout.TextField("Duration ", "Infinite");
                    EditorGUILayout.FloatField("Modifier", stM.Modifier);

                    GUI.enabled = true;
                }
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
        }

        showTickDamages = EditorGUILayout.Foldout(showTickDamages, "Tick Damages");
        if (showTickDamages && targetScript.AppliedTickDamages.Count > 0)
        {
            EditorGUILayout.BeginVertical("GroupBox");

            EditorGUI.indentLevel++;
            TickDamages tD;
            for (int i = 0; i < targetScript.AppliedTickDamages.Count; i++)
            {
                tD = targetScript.AppliedTickDamages[i];

                tD.showInEditor = EditorGUILayout.Foldout(tD.showInEditor, $"Element {i} : {tD.ID}");
                if (tD.showInEditor)
                {
                    GUI.enabled = false;

                    EditorGUILayout.FloatField("Damages", tD.Damages);
                    EditorGUILayout.FloatField("Time Between Damages", tD.TimeBetweenDamages);
                    EditorGUILayout.TextField("Duration ", $"{tD.Lifetime_TIMER} / {tD.Lifetime_TIMER}");

                    GUI.enabled = true;
                }
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
        }
        EditorGUI.indentLevel--;

        SerializedProperty level = serializedObject.FindProperty("level");
        EditorGUILayout.PropertyField(level);

        EditorGUILayout.LabelField("Dash", EditorStyles.boldLabel);

        SerializedProperty playerDash = serializedObject.FindProperty("playerDash");
        EditorGUILayout.PropertyField(playerDash);

        SerializedProperty dyingState_DURATION = serializedObject.FindProperty("dyingState_DURATION");
        EditorGUILayout.PropertyField(dyingState_DURATION);

        SerializedProperty reviveHealPercentage = serializedObject.FindProperty("reviveHealPercentage");
        EditorGUILayout.PropertyField(reviveHealPercentage);

        EditorGUILayout.EndVertical();
    }

    private void DrawMisc()
    {
        EditorGUILayout.Space(5);

        if (GUILayout.Button("Misc", ButtonToLabelStyle())) showMisc = !showMisc;
        if (!showMisc) return;

        EditorGUILayout.BeginVertical("GroupBox");

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Damage", GUILayout.MaxWidth(70)))
            targetScript.OnTakeDamages(damagesAmount, critDamages);
        damagesAmount = EditorGUILayout.FloatField(damagesAmount, GUILayout.MaxWidth(200));
        critDamages = EditorGUILayout.Toggle(critDamages);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Heal", GUILayout.MaxWidth(70)))
            targetScript.OnHeal(healAmount, critHeal);
        healAmount = EditorGUILayout.FloatField(healAmount, GUILayout.MaxWidth(200));
        critHeal = EditorGUILayout.Toggle(critHeal);

        EditorGUILayout.EndHorizontal();

        SerializedProperty debugMode = serializedObject.FindProperty("debugMode");
        EditorGUILayout.PropertyField(debugMode);

        GUI.enabled = false;
        EditorGUILayout.Space(5);
        EditorGUILayout.Vector2Field("Input Velocity", targetScript.Velocity);
        EditorGUILayout.Vector2Field("RB Velocity", targetScript.GetRb.velocity);
        GUI.enabled = true;

        SerializedProperty money = serializedObject.FindProperty("money");
        EditorGUILayout.PropertyField(money);

        SerializedProperty playerIndex = serializedObject.FindProperty("playerIndex");
        EditorGUILayout.PropertyField(playerIndex);

        GUI.enabled = false;
        EditorGUI.indentLevel++;

        SerializedProperty attackers = serializedObject.FindProperty("attackers");
        EditorGUILayout.PropertyField(attackers);

        EditorGUI.indentLevel--;
        GUI.enabled = true;

        EditorGUILayout.EndVertical();
    }

    private GUIStyle ButtonToLabelStyle()
    {
        var s = new GUIStyle();
        var b = s.border;
        b.left = 0;
        b.top = 0;
        b.right = 0;
        b.bottom = 0;
        s.fontStyle = FontStyle.Bold;
        s.normal.textColor = Color.white;
        return s;
    }
}

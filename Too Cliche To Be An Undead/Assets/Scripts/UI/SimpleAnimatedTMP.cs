using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleAnimatedTMP : MonoBehaviour
{
    private enum E_AnimationType
    {
        CharacterWave,
        WordWave,
        Reveal,
    }
    [SerializeField] private E_AnimationType animationType;

    [SerializeField] private TextMeshProUGUI tmp;

    [Header("Reveal")]

    [SerializeField] private float timeBetweenReveals = .1f;

    [SerializeField] private int totalVisibleCharacters;
    [SerializeField] private int startIndex;

    [Header("Wave")]

    [SerializeField] private float timeBetweenWaves = .1f;

    [SerializeField] private float sinValue = 3.3f;
    [SerializeField] private float cosValue = 2.5f;

    private Mesh mesh;

    private Vector3[] vertices;
    private List<int> wordIndexes;
    private List<int> wordLengths;

    public void StartAnim()
    {
        StopAllCoroutines();

        switch (animationType)
        {
            case E_AnimationType.CharacterWave:
                StartCoroutine(CharacterWaveAnim());
                break;

            case E_AnimationType.WordWave:
                StartCoroutine(WordWaveAnim());
                break;

            case E_AnimationType.Reveal:
                StartCoroutine(RevealAnim());
                break;
        }
    }

    private IEnumerator RevealAnim()
    {
        int counter = startIndex;

        while(true)
        {
            tmp.maxVisibleCharacters = counter;

            if (counter >= totalVisibleCharacters)
                counter = startIndex;
            else
                counter += 1;

            yield return new WaitForSecondsRealtime(timeBetweenReveals);
        }
    }

    private IEnumerator WordWaveAnim()
    {
        wordIndexes = new List<int> { 0 };
        wordLengths = new List<int>();

        string s = tmp.text;
        for (int index = s.IndexOf(' '); index > -1; index = s.IndexOf(' ', index + 1))
        {
            wordLengths.Add(index - wordIndexes[wordIndexes.Count - 1]);
            wordIndexes.Add(index + 1);
        }
        wordLengths.Add(s.Length - wordIndexes[wordIndexes.Count - 1]);

        tmp.ForceMeshUpdate();
        mesh = tmp.mesh;
        vertices = mesh.vertices;

        while (true)
        {
            for (int w = 0; w < wordIndexes.Count; w++)
            {
                int wordIndex = wordIndexes[w];
                Vector3 offset = Wobble(Time.time + w);

                for (int i = 0; i < wordLengths[w]; i++)
                {
                    TMP_CharacterInfo c = tmp.textInfo.characterInfo[wordIndex + i];

                    int index = c.vertexIndex;

                    vertices[index] += offset;
                    vertices[index + 1] += offset;
                    vertices[index + 2] += offset;
                    vertices[index + 3] += offset;
                }
            }

            mesh.vertices = vertices;
            tmp.canvasRenderer.SetMesh(mesh);

            yield return new WaitForSecondsRealtime(timeBetweenWaves);
        }
    }

    private IEnumerator CharacterWaveAnim()
    {
        int counter = startIndex;
        tmp.ForceMeshUpdate();
        mesh = tmp.mesh;
        vertices = mesh.vertices;

        while (true)
        {
            for (int i = 0; i < tmp.textInfo.characterCount; i++)
            {
                TMP_CharacterInfo c = tmp.textInfo.characterInfo[i];

                int index = c.vertexIndex;

                Vector3 offset = Wobble(Time.time + i);
                vertices[index] += offset;
                vertices[index + 1] += offset;
                vertices[index + 2] += offset;
                vertices[index + 3] += offset;
            }

            mesh.vertices = vertices;
            tmp.canvasRenderer.SetMesh(mesh);

            yield return new WaitForSecondsRealtime(timeBetweenWaves);
        }
    }

    private Vector2 Wobble(float time)
    {
        return new Vector2(Mathf.Sin(time * sinValue), Mathf.Cos(time * cosValue));
    }

}

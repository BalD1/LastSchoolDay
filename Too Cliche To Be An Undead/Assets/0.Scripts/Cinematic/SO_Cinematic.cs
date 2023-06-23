using UnityEngine;
using static SO_Cinematic;

[CreateAssetMenu(fileName = "NewCinematic", menuName = "Scriptable/Cinematic")]
public class SO_Cinematic : ScriptableObject
{
    [SerializeField] private S_CinematicActionWithIndex<CA_CinematicAction>[] actions = new S_CinematicActionWithIndex<CA_CinematicAction>[0];
    public S_CinematicActionWithIndex<CA_CinematicAction>[] CinematicActions { get => actions; set => actions = value; }

    [SerializeField] private S_CinematicActionWithIndex<CA_CinematicDialoguePlayer>[] dialoguePlayers = new S_CinematicActionWithIndex<CA_CinematicDialoguePlayer>[0];
    public S_CinematicActionWithIndex<CA_CinematicDialoguePlayer>[] CinematicDialogueActions { get => dialoguePlayers; private set => dialoguePlayers = value; }

    [SerializeField] private S_CinematicActionWithIndex<CA_CinematicPlayersMove>[] movePlayers = new S_CinematicActionWithIndex<CA_CinematicPlayersMove>[0];
    public S_CinematicActionWithIndex<CA_CinematicPlayersMove>[] CinematicMovePlayers { get => movePlayers; private set => movePlayers = value; }

    [System.Serializable]
    public struct S_CinematicActionWithIndex<T>
    {
        [SerializeField] private T action;
        public T Action { get => action; set => action = value; }

        [SerializeField] private int idxInCinematicArray;
        public int IndexInCinematicArray { get => idxInCinematicArray; set => idxInCinematicArray = value; }
        [SerializeField] private int idxInSelfArray;
        public int IndexInSelfArray { get => idxInSelfArray; set => idxInSelfArray = value; }

        public S_CinematicActionWithIndex(T act, int idInCinematicArray, int idInSelfArray)
        {
            action = act;
            idxInCinematicArray = idInCinematicArray;
            idxInSelfArray = idInSelfArray;
        }
    }

}
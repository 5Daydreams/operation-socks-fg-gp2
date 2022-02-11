using UnityEngine;

[CreateAssetMenu(fileName = "SceneData")]

public class SceneIndexes : ScriptableObject
{
    [SerializeField] internal int mainSceneIndex;
    [SerializeField] internal int startSceneIndex;
    [SerializeField] internal int winSceneIndex;
    [SerializeField] internal int loseSceneIndex;
    [SerializeField] internal int optionsSceneIndex;
}

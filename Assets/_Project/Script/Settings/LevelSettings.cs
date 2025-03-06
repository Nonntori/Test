using UnityEngine;

[CreateAssetMenu(fileName = "LevelSettings", menuName = "Settings/LevelSettings")] //Чтобы можно было создать как ассет файл
public class LevelSettings : ScriptableObject // лучше использовать ScriptableObject
{
    public int countBottle = 10;
    public GameObject bottlePrefab;
}
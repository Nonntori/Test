using UnityEngine;

public class Area : MonoBehaviour
{
    [SerializeField] private Box _boxPrefab;

    private Box _box;

    [ContextMenu("Create Box")]
    private void CreateBox()
    {
        Box box = Instantiate(_boxPrefab, transform, true);
        box.transform.position += transform.position;
    }
}

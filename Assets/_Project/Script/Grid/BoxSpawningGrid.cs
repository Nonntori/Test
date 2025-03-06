using UnityEngine;

public class BoxSpawningGrid: MonoBehaviour
{
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private Box _boxPrefab;
    
    [SerializeField] private int _gridWidth = 5;
    [SerializeField] private int _gridHeight = 5;
    [SerializeField] private float _spacingX = 1f;
    [SerializeField] private float _spacingY = 1f;
    [SerializeField] private Vector3 _startPosition;
    
    public void Initialize()
    {
        
    }
    
    private void CreateGrid()
    {
        for (int x = 0; x < _gridWidth; x++)    
        {
            for (int z = 0; z < _gridHeight; z++)
            {
                _startPosition = transform.position - new Vector3(x * _spacingX, 0f, z * _spacingY);
                Cell newObject = Instantiate(cellPrefab, _startPosition, Quaternion.identity);
                Box newBox = Instantiate(_boxPrefab, newObject.transform.position, Quaternion.identity);
                newBox.transform.SetParent(transform);
            }   
        }
    }

}

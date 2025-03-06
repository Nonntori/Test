 using System;
 using UnityEngine;

public class BottleCollectionGrid : MonoBehaviour
{
    [SerializeField] private Cell prefabCollectionArea;
    [SerializeField] private int _objectCountPerArea;
    [SerializeField] private float _spacing;
    [SerializeField] private Vector3 _startPosition;
    
    public Cell[] GatheringAreas { get; private set; }

    private void Awake()
    {
        GatheringAreas = new Cell[_objectCountPerArea];
        
        FillArray();
    }

    [ContextMenu("Create Area")]
    private void CreateCollectionArea()
    {
        for (int i = 0; i < _objectCountPerArea; i++)
        {
            _startPosition = transform.position - new Vector3(i * _spacing, 0, 0);
            
            Cell collectionArea = Instantiate(prefabCollectionArea, _startPosition, Quaternion.identity);
            collectionArea.transform.SetParent(transform);
            GatheringAreas[i] = collectionArea;
        }
    }

    private void FillArray()
    {
        Cell[] children = GetComponentsInChildren<Cell>(); 
        
        for (int i = 0; i < children.Length; i++)
        {
            GatheringAreas[i] = children[i];
        }
    }
    
    
    
    public void SetGatheringAreas(Cell[] areas)
    {
        GatheringAreas = areas;
    }
}
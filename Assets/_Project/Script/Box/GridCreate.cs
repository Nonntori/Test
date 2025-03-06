using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class GridCreate : MonoBehaviour
{
    [SerializeField] private int _gridWidth = 5;
    [SerializeField] private int _gridLength = 5;
    [SerializeField] private float _cellSpacing = 0.2f;
    [SerializeField] private Vector3 _gridOrigin = Vector3.zero;
    [SerializeField] private bool _showDebugGrid = true;
    [SerializeField] private bool _debugMode = false;
    [SerializeField] private GameObject _cellPrefab;

    public List<Cell> Cells { get; } = new List<Cell>();

    private void Awake()
    {
        CreateGrid();
    }

    /// <summary>
    /// Создание сетки
    /// </summary>
    public void CreateGrid()
    {
        // Очистка существующей сетки, если она есть
        ClearGrid();

        // Создание ячеек
        for (int x = 0; x < _gridWidth; x++) {
            for (int z = 0; z < _gridLength; z++) {
                // Вычисление позиции ячейки с учетом расстояния между ячейками
                float totalCellWidth = _cellSpacing;
                Vector3 cellPosition = _gridOrigin + new Vector3(x * totalCellWidth, 0, z * totalCellWidth);
                
                // Создание ячейки
                GameObject cell =  Instantiate(_cellPrefab, cellPosition, Quaternion.identity);
                cell.transform.SetParent(transform); // Установка родителя
                cell.name = $"Cell_{x}_{z}"; // Установка имени

                // Создание и добавление данных о ячейке в список
                Cell cellData = new Cell(cell, new Vector2Int(x, z), cellPosition);
                Cells.Add(cellData);
            }
        }

        if (_debugMode) {
            Debug.Log($"Создана сетка для коробок: {_gridWidth}x{_gridLength}");
        }
    }
    /// <summary>
    /// Очистка сетки
    /// </summary>
    private void ClearGrid()
    {
        // Удаление всех дочерних объектов
        while (transform.childCount > 0) {
            Object.DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
    /// <summary>
    /// Для визуализации сетки в редакторе
    /// </summary>
    private void OnDrawGizmos()
    {
        float cellSize = 1f;
        
        if (_showDebugGrid) {
            Gizmos.color = Color.yellow;

            float totalCellWidth = _cellSpacing;

            for (int x = 0; x < _gridWidth; x++) {
                for (int z = 0; z < _gridLength; z++) {
                    Vector3 pos = _gridOrigin + new Vector3(x * totalCellWidth, 0, z * totalCellWidth);
                    Gizmos.DrawWireCube(pos, new Vector3(cellSize, 0.1f, cellSize));

                    #if UNITY_EDITOR
                    Handles.Label(pos + Vector3.up * 0.3f, $"{x},{z}");
                    #endif
                }
            }
        }
    }
}

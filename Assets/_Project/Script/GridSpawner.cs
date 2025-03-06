using UnityEngine;
using System.Collections.Generic;

public class GridSpawner : MonoBehaviour
{
    [Header("Grid Settings")] [SerializeField]
    private int gridWidth = 5; // Ширина сетки (количество ячеек)

    [SerializeField] private int gridLength = 5; // Длина сетки (количество ячеек)
    [SerializeField] private float cellSize = 1f; // Размер одной ячейки
    [SerializeField] private float cellSpacing = 0.2f; // Расстояние между ячейками
    [SerializeField] private Vector3 gridOrigin = Vector3.zero; // Начальная позиция сетки

    [Header("Prefabs")] [SerializeField] private GameObject cellPrefab; // Префаб для визуализации ячейки
    [SerializeField] private GameObject boxPrefab; // Префаб для коробок

    [Header("Box Spawn Settings")] [SerializeField]
    private bool spawnBoxesOnStart = true; // Создавать ли коробки при старте

   

    // Структура для хранения данных ячейки
    [System.Serializable]
    public class CellData
    {
        public GameObject cellObject;
        public GameObject boxObject;
        public Vector2Int gridPosition;
        public Vector3 worldPosition;

        public CellData(GameObject cell, Vector2Int pos, Vector3 worldPos)
        {
            cellObject = cell;
            gridPosition = pos;
            worldPosition = worldPos;
            boxObject = null;
        }
    }

    // Список для хранения ячеек
    private List<CellData> cells = new List<CellData>();

    private void Start()
    {
        CreateGrid();

        if (spawnBoxesOnStart) {
            SpawnBoxes();
        }
    }

    // Создание сетки
    public void CreateGrid()
    {
        // Очистка существующей сетки, если она есть
        ClearGrid();

        // Создание ячеек
        for (int x = 0; x < gridWidth; x++) {
            for (int z = 0; z < gridLength; z++) {
                // Вычисление позиции ячейки с учетом расстояния между ячейками
                float totalCellWidth = cellSize + cellSpacing;
                Vector3 cellPosition = gridOrigin + new Vector3(x * totalCellWidth, 0, z * totalCellWidth);

                // Создание ячейки
                GameObject cell = Instantiate(cellPrefab, cellPosition, Quaternion.identity);
                cell.transform.localScale = new Vector3(cellSize, 0.1f, cellSize); // Настройка размера ячейки
                cell.transform.SetParent(transform); // Установка родителя
                cell.name = $"Cell_{x}_{z}"; // Установка имени

                // Создание и добавление данных о ячейке в список
                CellData cellData = new CellData(cell, new Vector2Int(x, z), cellPosition);
                cells.Add(cellData);
            }
        }
    }

    // Размещение коробок на сетке
    public void SpawnBoxes()
    {
        // Очистка существующих коробок, если они есть
        ClearBoxes();

        // Размещение коробок
        foreach (CellData cell in cells) {
            // Вычисление позиции коробки (чуть выше ячейки)
            Vector3 boxPosition = cell.worldPosition + new Vector3(0, cellSize / 2f, 0);

            // Создание коробки
            GameObject box = Instantiate(boxPrefab, boxPosition, Quaternion.identity);
            box.transform.SetParent(transform); // Установка родителя
            box.name = $"Box_{cell.gridPosition.x}_{cell.gridPosition.y}"; // Установка имени

            // Опционально: настройка размера коробки (если нужно)
            box.transform.localScale = new Vector3(cellSize * 0.8f, cellSize, cellSize * 0.8f);

            // Сохранение ссылки на коробку в данных ячейки
            cell.boxObject = box;
        }
    }

    // Очистка сетки
    private void ClearGrid()
    {
        // Удаление всех дочерних объектов
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        // Очистка списка ячеек
        cells.Clear();
    }

    // Очистка только коробок
    private void ClearBoxes()
    {
        foreach (CellData cell in cells) {
            if (cell.boxObject != null) {
                DestroyImmediate(cell.boxObject);
                cell.boxObject = null;
            }
        }
    }

    // Метод для изменения параметров сетки в рантайме
    public void ResizeGrid(int newWidth, int newLength)
    {
        gridWidth = newWidth;
        gridLength = newLength;
        CreateGrid();

        if (spawnBoxesOnStart) {
            SpawnBoxes();
        }
    }

    // Метод для изменения расстояния между ячейками
    public void SetCellSpacing(float spacing)
    {
        cellSpacing = spacing;
        CreateGrid();

        if (spawnBoxesOnStart) {
            SpawnBoxes();
        }
    }

    // Получение ячейки по координатам сетки
    public CellData GetCellAt(int x, int z)
    {
        return cells.Find(cell => cell.gridPosition.x == x && cell.gridPosition.y == z);
    }

    // Получение ячейки по мировой позиции
    public CellData GetCellAtWorldPosition(Vector3 worldPos)
    {
        float totalCellWidth = cellSize + cellSpacing;

        int x = Mathf.FloorToInt((worldPos.x - gridOrigin.x) / totalCellWidth);
        int z = Mathf.FloorToInt((worldPos.z - gridOrigin.z) / totalCellWidth);

        // Проверка, находятся ли координаты в пределах сетки
        if (x >= 0 && x < gridWidth && z >= 0 && z < gridLength) {
            return GetCellAt(x, z);
        }

        return null;
    }

    // Для визуализации сетки в редакторе
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        float totalCellWidth = cellSize + cellSpacing;

        for (int x = 0; x < gridWidth; x++) {
            for (int z = 0; z < gridLength; z++) {
                Vector3 pos = gridOrigin + new Vector3(x * totalCellWidth, 0, z * totalCellWidth);
                Gizmos.DrawWireCube(pos, new Vector3(cellSize, 0.1f, cellSize));
            }
        }
    }
}

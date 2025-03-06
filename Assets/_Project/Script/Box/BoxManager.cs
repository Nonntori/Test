using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxManager : MonoBehaviour
{
    [SerializeField] private Box boxPrefab; // Префаб для коробок
    
    [SerializeField] private GridCreate gridCreate;
    
    // Словарь для хранения доступных коробок по цветам
    private Dictionary<Colors, List<Box>> availableBoxes = new Dictionary<Colors, List<Box>>();

    // Список для хранения ячеек
    private Coroutine boxSpawnCoroutine; // Ссылка на корутину создания коробок
    
    private void Awake()
    {
        // Инициализируем словарь для каждого цвета
        foreach (Colors color in Enum.GetValues(typeof(Colors))) {
            availableBoxes[color] = new List<Box>();
        }
    }

    private void Start()
    {
        // Создаем начальные коробки, если нужно
        SpawnBoxes();
    }
    
    //Выбирает случайный цвет для новой коробки с учетом текущего распределения
    private Colors GetRandomColorForBox()
    {
        // Получаем все цвета, кроме None
        List<Colors> allColors = Enum.GetValues(typeof(Colors))
            .Cast<Colors>()
            .Where(c => c != Colors.None)
            .ToList();

        // Находим цвета с наименьшим количеством коробок
        int minBoxCount = int.MaxValue;
        foreach (var color in allColors) {
            minBoxCount = Mathf.Min(minBoxCount, availableBoxes[color].Count);
        }

        // Отбираем цвета с наименьшим количеством коробок
        List<Colors> priorityColors = allColors
            .Where(c => availableBoxes[c].Count == minBoxCount)
            .ToList();

        // Выбираем случайный цвет из приоритетных
        return priorityColors[UnityEngine.Random.Range(0, priorityColors.Count)];
    }
    
    // Размещение коробок на сетке
    private void SpawnBoxes()
    {
        // Размещение коробок
        foreach (Cell cell in gridCreate.Cells) {
            
            Colors randomColor = GetRandomColorForBox();

            // Вычисление позиции коробки (чуть выше ячейки)
            Vector3 boxPosition = cell.worldPosition;

            // Создание коробки
            Box newBox = Instantiate(boxPrefab, boxPosition, Quaternion.identity);
            newBox.transform.SetParent(transform); // Установка родителя
            newBox.name = $"Box_{cell.gridPosition.x}_{cell.gridPosition.y}_{randomColor}"; // Установка имени

            // Устанавливаем цвет коробки
            newBox.SetBoxColor(randomColor);

            // Сохранение ссылки на коробку в данных ячейки
            cell.boxObject = newBox;

            // Добавляем коробку в список доступных
            availableBoxes[randomColor].Add(newBox);
        }
    }
    
     public Box GetBoxForBottle(Colors bottleColor, bool preferNonFull = true)
     {
         if (availableBoxes.TryGetValue(bottleColor, out List<Box> boxes) && boxes.Count > 0)
         {
             if (preferNonFull)
             {
                 // Ищем не полную коробку
                 foreach (var box in boxes)
                 {
                     if (!box.IsFull)
                     {
                         return box;
                     }
                 }
             }
         }

         return null;
     }
}

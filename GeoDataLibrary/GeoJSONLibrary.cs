using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Статический класс для работы с GeoJSON данными.
/// </summary>
public static class GeoJSONLibrary
{
    /// <summary>
    /// Загружает GeoJSON данные из файла.
    /// </summary>
    /// <param name="filePath">Путь к файлу с GeoJSON данными.</param>
    /// <returns>Объект GeoJSONData, представляющий загруженные данные.</returns>
    public static GeoJSONData LoadGeoJSON(string filePath)
    {
        var json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<GeoJSONData>(json);
    }

    /// <summary>
    /// Сохраняет GeoJSON данные в файл.
    /// </summary>
    /// <param name="filePath">Путь к файлу для сохранения GeoJSON данных.</param>
    /// <param name="data">Объект GeoJSONData для сохранения.</param>
    public static void SaveGeoJSON(string filePath, GeoJSONData data)
    {
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Получает тип геометрии первого объекта в GeoJSON данных.
    /// </summary>
    /// <param name="data">Объект GeoJSONData для анализа.</param>
    /// <returns>Строка, представляющая тип геометрии.</returns>
    public static string GetGeometryType(GeoJSONData data)
    {
        return data?.Features?[0]?.Geometry?.Type;
    }

    /// <summary>
    /// Получает координаты первого объекта в GeoJSON данных.
    /// </summary>
    /// <param name="data">Объект GeoJSONData для анализа.</param>
    /// <returns>JToken, содержащий координаты.</returns>
    public static JToken GetCoordinates(GeoJSONData data)
    {
        return data?.Features?[0]?.Geometry?.Coordinates;
    }

    /// <summary>
    /// Рассчитывает площадь полигона в GeoJSON данных.
    /// </summary>
    /// <param name="data">Объект GeoJSONData, содержащий полигон.</param>
    /// <returns>Площадь полигона в квадратных метрах.</returns>
    public static double CalculateArea(GeoJSONData data)
    {
        if (data == null || data.Features == null || data.Features.Count == 0)
            throw new ArgumentException("Неверные данные GeoJSON.");

        var feature = data.Features[0];
        if (feature.Geometry.Type != "Полигон")
            throw new InvalidOperationException("Геометрия - это не многоугольник");

        var coordinates = feature.Geometry.Coordinates.ToObject<List<List<List<double>>>>();
        return ComputePolygonArea(coordinates[0]);
    }

    /// <summary>
    /// Рассчитывает расстояние между двумя точками в GeoJSON данных.
    /// </summary>
    /// <param name="data1">Первый объект GeoJSONData, содержащий точку.</param>
    /// <param name="data2">Второй объект GeoJSONData, содержащий точку.</param>
    /// <returns>Расстояние между точками в километрах.</returns>
    public static double CalculateDistance(GeoJSONData data1, GeoJSONData data2)
    {
        if (data1 == null || data1.Features == null || data1.Features.Count == 0 ||
            data2 == null || data2.Features == null || data2.Features.Count == 0)
            throw new ArgumentException("Неверные данные GeoJSON.");

        var coord1 = data1.Features[0].Geometry.Coordinates.ToObject<List<double>>();
        var coord2 = data2.Features[0].Geometry.Coordinates.ToObject<List<double>>();

        return ComputeDistance(coord1[1], coord1[0], coord2[1], coord2[0]);
    }

    /// <summary>
    /// Вспомогательный метод для расчета площади полигона.
    /// </summary>
    /// <param name="coordinates">Список координат полигона.</param>
    /// <returns>Площадь полигона.</returns>
    private static double ComputePolygonArea(List<List<double>> coordinates)
    {
        double area = 0;
        int j = coordinates.Count - 1;

        for (int i = 0; i < coordinates.Count; i++)
        {
            area += (coordinates[j][0] + coordinates[i][0]) * (coordinates[j][1] - coordinates[i][1]);
            j = i;
        }

        return Math.Abs(area / 2.0);
    }

    /// <summary>
    /// Вспомогательный метод для расчета расстояния между двумя точками.
    /// </summary>
    /// <param name="lat1">Широта первой точки.</param>
    /// <param name="lon1">Долгота первой точки.</param>
    /// <param name="lat2">Широта второй точки.</param>
    /// <param name="lon2">Долгота второй точки.</param>
    /// <returns>Расстояние между точками.</returns>
    private static double ComputeDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371; // Радиус земельки в км
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c; // расстояние в км
    }

    /// <summary>
    /// Вспомогательный метод для преобразования градусов в радианы.
    /// </summary>
    /// <param name="deg">Угол в градусах.</param>
    /// <returns>Угол в радианах.</returns>
    private static double ToRadians(double deg)
    {
        return deg * (Math.PI / 180);
    }
    /// <summary>
    /// Находит объект (Feature), ближайший к заданным координатам.
    /// </summary>
    /// <param name="data">Объект GeoJSONData, содержащий набор объектов для поиска.</param>
    /// <param name="latitude">Широта точки, для которой нужно найти ближайший объект.</param>
    /// <param name="longitude">Долгота точки, для которой нужно найти ближайший объект.</param>
    /// <returns>Объект Feature, который является ближайшим к заданным координатам.</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если переданный объект GeoJSONData недействителен или пуст.</exception>
    public static Feature FindNearestGeometry(GeoJSONData data, double latitude, double longitude)
    {

            if (data == null || data.Features == null || data.Features.Count == 0)
                throw new ArgumentException("Неверные данные GeoJSON.");

            Feature nearestFeature = null;
            double minDistance = double.MaxValue;

            foreach (var feature in data.Features)
            {
                var coord = feature.Geometry.Coordinates.ToObject<List<double>>();
                var distance = ComputeDistance(latitude, longitude, coord[1], coord[0]);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestFeature = feature;
                }
            }

            return nearestFeature;
        
    }

}

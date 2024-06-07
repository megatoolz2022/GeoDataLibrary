using System;

using Newtonsoft.Json.Linq;

class Program
{
    static void Main()
    {

        var geoData = GeoJSONLibrary.LoadGeoJSON("C:\\Users\\megat\\Downloads\\data.geojson");

        // Заданные координаты точки, к которой нужно найти ближайший объект
        double latitude = 55.7558;  //  широта 
        double longitude = 37.6173; // долгота 

        // Ищем ближайший объект к заданной точке
        var nearestFeature = GeoJSONLibrary.FindNearestGeometry(geoData, latitude, longitude);

        // Выводим информацию о ближайшем объекте
        if (nearestFeature != null)
        {
            var geometryType = nearestFeature.Geometry.Type;
            var coordinates = nearestFeature.Geometry.Coordinates;
            var properties = nearestFeature.Properties;

            Console.WriteLine($"Ближайший тип геометрии: {geometryType}");
            Console.WriteLine($"Ближайшие геометрические координаты: {coordinates}");
            Console.WriteLine("Свойства ближайшей геометрии:");
            foreach (var property in properties)
            {
                Console.WriteLine($"{property.Key}: {property.Value}");
            }
        }
        else
        {
            Console.WriteLine("В данных GeoJSON не обнаружено никаких объектов.");
        }
    }
}
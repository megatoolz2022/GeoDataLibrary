using NUnit.Framework;
using System;
using Newtonsoft.Json.Linq;

[TestFixture]
public class GeoJSONLibraryTests
{
    private const string SampleFilePath = "C:\\Users\\megat\\Downloads\\data.geojson";
    private GeoJSONData sampleData;

    [SetUp]
    public void SetUp()
    {
 
        sampleData = new GeoJSONData
        {
            Type = "FeatureCollection",
            Features = new List<Feature>
            {
                new Feature
                {
                    Type = "Feature",
                    Geometry = new Geometry
                    {
                        Type = "Point",
                        Coordinates = JToken.FromObject(new List<double> { 127.5, 10.5 })
                    },
                    Properties = new Dictionary<string, object>
                    {
                        { "name", "Dinagat Islands" }
                    }
                }
            }
        };

    }

    [Test]
    public void LoadGeoJSON_ValidFile_ReturnsGeoJSONData()
    {
        var result = GeoJSONLibrary.LoadGeoJSON(SampleFilePath);
        Assert.IsNotNull(result);

    }

    [Test]
    public void SaveGeoJSON_ValidData_WritesToFile()
    {

        GeoJSONLibrary.SaveGeoJSON(SampleFilePath, sampleData);

    }

    [Test]
    public void GetGeometryType_ValidData_ReturnsCorrectType()
    {
        var result = GeoJSONLibrary.GetGeometryType(sampleData);
        Assert.AreEqual("Polygon", result);
    }

    [Test]
    public void GetCoordinates_ValidData_ReturnsCoordinates()
    {
        var result = GeoJSONLibrary.GetCoordinates(sampleData);
        Assert.IsInstanceOf<JToken>(result);

    }

    [Test]
    public void CalculateArea_ValidPolygonData_ReturnsArea()
    {
        var result = GeoJSONLibrary.CalculateArea(sampleData);
        Assert.Greater(result, 0);
    }

    [Test]
    public void CalculateDistance_ValidData_ReturnsDistance()
    {

        var result = GeoJSONLibrary.CalculateDistance(sampleData, sampleData);
        Assert.Greater(result, 0);
    }

    [Test]
    public void FindNearestGeometry_ValidData_ReturnsFeature()
    {
        var result = GeoJSONLibrary.FindNearestGeometry(sampleData, 0, 0);
        Assert.IsNotNull(result);

    }

}

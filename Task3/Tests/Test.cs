using WeatherService.DataModel;

namespace Tests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestOutput()
    {
        WeatherData data1 = new WeatherData();
        data1.TemperatureC = 1;
        data1.Humidity = "88";
        WeatherData data2 = new WeatherData();
        data2.Cloudiness = "99";
        data2.Humidity = "84";

        Assert.AreEqual(data1.TemperatureF, 33);
        WeatherSourceData output = new WeatherSourceData();
        output.AddWeatherSourceData("source1", data1);
        output.AddWeatherSourceData("source2", data2);

        Assert.AreEqual(output.GetWeatherSourceData()["source1"].TemperatureC, 1);
        Assert.AreEqual(output.GetWeatherSourceData()["source1"].TemperatureF, 33);
        Assert.AreEqual(output.GetWeatherSourceData()["source1"].Cloudiness, "NODATA");
        Assert.AreEqual(output.GetWeatherSourceData()["source1"].Humidity, "88");
        Assert.AreEqual(output.GetWeatherSourceData()["source1"].Precipitation, "NODATA");
        Assert.AreEqual(output.GetWeatherSourceData()["source1"].WindDirection, "NODATA");
        Assert.AreEqual(output.GetWeatherSourceData()["source1"].WindSpeed, "NODATA");

        Assert.AreEqual(output.GetWeatherSourceData()["source2"].TemperatureC, 0);
        Assert.AreEqual(output.GetWeatherSourceData()["source2"].TemperatureF, 32);
        Assert.AreEqual(output.GetWeatherSourceData()["source2"].Cloudiness, "99");
        Assert.AreEqual(output.GetWeatherSourceData()["source2"].Humidity, "84");
        Assert.AreEqual(output.GetWeatherSourceData()["source2"].Precipitation, "NODATA");
        Assert.AreEqual(output.GetWeatherSourceData()["source2"].WindDirection, "NODATA");
        Assert.AreEqual(output.GetWeatherSourceData()["source2"].WindSpeed, "NODATA");
    }
}
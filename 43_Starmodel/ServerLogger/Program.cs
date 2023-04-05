using Bogus;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

Randomizer.Seed = new Random(910);
var faker = new Faker("de");
var minDate = new DateTime(2020, 12, 28);
var weeks = 104;

var states = new Status[]
{
    new Status(200, "OK"),
    new Status(400, "Bad request"),
    new Status(404, "Not found"),
    new Status(500, "Internal server error")
};
var statusWeight = new float[] { 0.8f, 0.05f, 0.1f, 0.05f };
var servers = new Server[]
{
    new Server("www.spengergasse.at", "192.168.51.143", 1000),
    new Server("e-formular.spengergasse.at", "192.168.51.149", 1000),
    new Server("hub.spengergasse.at", "192.168.51.143", 500)
};
var serverNumbers = Enumerable.Range(0, servers.Length).Select(i => i).ToArray();
var serverWeights = new float[] { 0.5f, 0.4f, 0.1f };
var serverWeekdayWeights = new float[][]
{
    new float[]{0.143f, 0.143f, 0.143f, 0.143f, 0.143f, 0.143f, 0.143f},
    new float[]{0.178f, 0.178f, 0.178f, 0.178f, 0.178f, 0.089f, 0.022f},
    new float[]{0.178f, 0.178f, 0.178f, 0.178f, 0.178f, 0.089f, 0.022f},
};
var serverHourWeights = new float[][]
{
    new float[]{0.038f, 0.030f, 0.027f, 0.023f, 0.019f, 0.019f, 0.027f, 0.029f, 0.032f, 0.035f, 0.039f, 0.042f, 0.038f, 0.046f, 0.053f, 0.057f, 0.061f, 0.064f, 0.072f, 0.064f, 0.057f, 0.049f, 0.042f, 0.038f},
    new float[]{0.024f, 0.019f, 0.017f, 0.015f, 0.012f, 0.012f, 0.017f, 0.048f, 0.097f, 0.085f, 0.073f, 0.068f, 0.053f, 0.061f, 0.085f, 0.061f, 0.048f, 0.036f, 0.034f, 0.031f, 0.029f, 0.027f, 0.024f, 0.024f},
    new float[]{0.024f, 0.019f, 0.017f, 0.015f, 0.012f, 0.012f, 0.017f, 0.048f, 0.097f, 0.085f, 0.073f, 0.068f, 0.053f, 0.061f, 0.085f, 0.061f, 0.048f, 0.036f, 0.034f, 0.031f, 0.029f, 0.027f, 0.024f, 0.024f},
};

var ips = Enumerable.Range(0, 100).Select(i => faker.Internet.Ip()).ToList();
var urls = Enumerable.Range(0, 100).Select(i => faker.Internet.UrlRootedPath()).ToList();
var days = Enumerable.Range(0, 7).Select(i => i).ToArray();
var hours = Enumerable.Range(0, 24).Select(i => i).ToArray();

IEnumerable<Log> logs = new Faker<Log>("de").CustomInstantiator(f =>
{
    var serverNr = f.Random.WeightedRandom(serverNumbers, serverWeights);
    var week = f.Random.Int(0, weeks);
    var date = minDate.AddDays(7 * week)
        .AddDays(f.Random.WeightedRandom(days, serverWeekdayWeights[serverNr]))
        .AddHours(f.Random.WeightedRandom(hours, serverHourWeights[serverNr]))
        .AddSeconds(f.Random.Int(0, 3600));
    var log = new Log(
        date,
        f.Internet.Ip(),
        f.Internet.UrlRootedPath(),
        f.Random.Int(100, 1000),
        f.Random.Int(100, 1 << 20),
        servers[serverNr],
        f.Random.WeightedRandom(states, statusWeight));
    return log;
})
.GenerateLazy(1000000)
.ToList();

logs = logs.Concat(GenerateAttack(1000, "14.18.94.214", "/passwords", new DateTime(2021, 4, 12), servers[0], states[2]));
logs = logs.Concat(GenerateAttack(1000, "154.197.32.14", "/login", new DateTime(2022, 8, 12), servers[1], states[1]));
var logList = logs.ToList().OrderBy(l => l.RequestDate);
var config = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
{
    Delimiter = "\t",
    NewLine = "\r\n"
};
using (var writer = new CsvWriter(new StreamWriter("log_unicode.txt", false, Encoding.Unicode), config))
{
    writer.WriteRecords(logList);
}

var dates = Enumerable.Range(0, 24 * 7 * (weeks+1)).Select(h =>
{
    var hour = minDate.AddHours(h);
    var dayOfWeek = (int)hour.DayOfWeek + 6 % 7;
    return new Date(hour, hour.Date, hour.Hour, dayOfWeek, dayOfWeek < 6 ? 1 : 0);
});
using (var writer = new CsvWriter(new StreamWriter("date_unicode.txt", false, Encoding.Unicode), config))
{
    writer.WriteRecords(dates);
}

IEnumerable<Log> GenerateAttack(int count, string ip, string url, DateTime startDate, Server server, Status status) =>
    Enumerable.Range(0, count).Select(i => new Log(startDate.AddSeconds(i), ip, url, 10, 1000, server, status));
public record Log(
    [property: Format("yyyy-MM-ddTHH:mm:ss")] DateTime RequestDate, string ClientIp, string RequestUrl, int BytesIn, int BytesOut,
    [property: HeaderPrefix("Server")] Server Server, [property: HeaderPrefix("Status")] Status Status);
public record Server(string Domain, string Ip, int Bandwidth);
public record Status(int Code, string Name);
public record Date(
    [property: Format("yyyy-MM-ddTHH:mm:ss")] DateTime Hour,
    [property: Format("yyyy-MM-dd")] DateTime Day, int HourNr, int DayOfWeek,
    int WorkingDay);
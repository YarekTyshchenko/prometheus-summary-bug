using Prometheus;
Metrics.SuppressDefaultMetrics();

var metric = Metrics.CreateSummary(
    "test",
    "test",
    new SummaryConfiguration
    {
        Objectives = new[]
        {
            new QuantileEpsilonPair(0, 0),
            new QuantileEpsilonPair(1, 0),
        }
    });

// Write max value
metric.Observe(0.8);

// fill with min values
for (var i = 0; i < 498; i++)
{
    metric.Observe(0.2);
}

// Observe that 500 samples makes:
// # HELP test test
// # TYPE test summary
// test_sum 100.60000000000089
// test_count 500
// test{quantile="0"} 0.8
// test{quantile="1"} 0.8

// and 499 samples:
// # HELP test test
// # TYPE test summary
// test_sum 100.40000000000089
// test_count 499
// test{quantile="0"} 0.2
// test{quantile="1"} 0.8

// Decode and print metrics in console
var stream = new MemoryStream();
await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream);
stream.Position = 0;
var reader = new StreamReader(stream);
var text = await reader.ReadToEndAsync();
Console.WriteLine(text);

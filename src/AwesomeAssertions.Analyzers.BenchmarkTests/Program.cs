using BenchmarkDotNet.Running;

namespace AwesomeAssertions.Analyzers.BenchmarkTests
{
    public class Program
    {
        public static void Main() 
            => BenchmarkDotNet.Running.BenchmarkRunner.Run<AwesomeAssertionsBenchmarks>();
    }
}
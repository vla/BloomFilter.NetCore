using System;
using System.Text;

namespace PerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            TestExcute.Excute(typeof(Program));
        }
    }
}

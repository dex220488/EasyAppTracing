using EasyAppTracing;
using System;

namespace TestLogging
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    var a = Add(1, 3);
                    TracingAttribute tracingAttribute = new TracingAttribute();
                    tracingAttribute.Write("Main", "Test Message Error", true);
                    tracingAttribute.Write("Main", "Test Message Success", false);
                    var b = Division(1, 0);
                }
                catch (Exception ex)
                {
                    //throw;
                }
            }

            //se quita comentario
            Console.ReadLine();
        }

        [Tracing]
        private static int Add(int a, int b)
        {
            return a + b;
        }

        [Tracing]
        private static decimal Division(decimal a, decimal b)
        {
            return a / b;
        }
    }
}
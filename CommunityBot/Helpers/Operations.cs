using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityBot.Helpers
{
    public static class Operations
    {

        public static Dictionary<String, Func<double, double, double>> validOperations = new Dictionary<String, Func<double, double, double>>()
        {
            { "+", Add },
            { "-", Sub },
            { "*", Mult },
            { "/", Div },
            { "%", Mod }
        };

        public static double PerformComputation(String sentence)
        {
            if (sentence == null || sentence.Length == 0 || sentence.Length < 3) return 0;

            List<String> seperatedValues = new List<string>();
            String buffer = "";
            for (int i=0; i<sentence.Length; i++)
            {
                if (validOperations.ContainsKey(sentence[i].ToString()))
                {
                    seperatedValues.Add(buffer);
                    seperatedValues.Add(sentence[i].ToString());
                    buffer = "";
                }
                else
                {
                    buffer += sentence[i];
                }
            }
            seperatedValues.Add(buffer);

            if (seperatedValues.Count > 3)
            {
                int pos = 0;
                while (seperatedValues.Contains("*") || seperatedValues.Contains("/"))
                {
                    for (int i = pos; i < seperatedValues.Count; i++)
                    {
                        if (seperatedValues[i].Equals("*") || seperatedValues[i].Equals("/"))
                        {
                            String s = "";
                            s += seperatedValues[i - 1];
                            s += seperatedValues[i + 0];
                            s += seperatedValues[i + 1];
                            seperatedValues.RemoveAt(i - 1);
                            seperatedValues.RemoveAt(i - 1);
                            seperatedValues[i - 1] = ((double)Operations.PerformComputation(s)).ToString();
                            break;
                        }
                        pos++;
                    }
                }
                while (seperatedValues.Count > 3)
                {
                    String s = "";
                    s += seperatedValues[0];
                    s += seperatedValues[1];
                    s += seperatedValues[2];
                    seperatedValues.RemoveAt(0);
                    seperatedValues.RemoveAt(0);
                    seperatedValues[0] = ((double)Operations.PerformComputation(s)).ToString();
                }
            }
            double x = double.Parse(seperatedValues[0]);
            double y = double.Parse(seperatedValues[2]);
            return validOperations[seperatedValues[1]](x, y);
        }

        private static double Add(double x, double y)
        {
            return x + y;
        }

        private static double Sub(double x, double y)
        {
            return x - y;
        }

        private static double Mult(double x, double y)
        {
            return x * y;
        }

        private static double Div(double x, double y)
        {
            return x / y;
        }

        private static double Mod(double x, double y)
        {
            return x % y;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityBot.Helpers
{
    public static class Operations
    {

        private static Dictionary<String, Func<double, double, double>> validOperations = new Dictionary<String, Func<double, double, double>>
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
            SeperateValues(seperatedValues, sentence);

            if (seperatedValues.Count > 3)
            {
                int i = 0;
                while (i < seperatedValues.Count-1)
                {
                    if (seperatedValues[i + 1].Equals("*") || seperatedValues[i + 1].Equals("/"))
                    {
                        ReplaceSegment(seperatedValues, i);
                    }
                    else
                    {
                        i++;
                    }
                }
                while (seperatedValues.Count > 3)
                {
                    ReplaceSegment(seperatedValues, 0);
                }
            }
            double x = double.Parse(seperatedValues[0]);
            double y = double.Parse(seperatedValues[2]);
            return validOperations[seperatedValues[1]](x, y);
        }

        private static void SeperateValues(List<String> list, String sentence)
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < sentence.Length; i++)
            {
                if (validOperations.ContainsKey(sentence[i].ToString()))
                {
                    list.Add(buffer.ToString());
                    list.Add(sentence[i].ToString());
                    buffer = new StringBuilder();
                }
                else
                {
                    buffer.Append(sentence[i]);
                }
            }
            list.Add(buffer.ToString());
        }

        private static void ReplaceSegment(List<String> list, int startIdx)
        {
            if (list.Count < 3) return;
            String s = "";
            s += list[startIdx+0];
            s += list[startIdx+1];
            s += list[startIdx+2];
            list.RemoveAt(startIdx);
            list.RemoveAt(startIdx);
            list[startIdx] = Operations.PerformComputation(s).ToString();
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

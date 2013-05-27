using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanEditor
{
    public class ConvertStrings
    {
        public static List<double> ConverteString(string str)
        {
            var lst = new List<string>();

            bool hasZ = false;

            for (int i = 0; i < str.Length; ++i)
            {
                if ((str[i] == 'm') || (str[i] == 'M') || (str[i] == 'c') || (str[i] == 'l') || (str[i] == 'r') || (str[i] == 'z') || (str[i] == 'L'))
                {
                    lst.Add(str[i] + i.ToString());

                    if (str[i] == 'z')
                        hasZ = true;
                }
            }

            var data = new List<double>();

            for (int i = 0; i < lst.Count; ++i)
            {
                char c = lst[i].Substring(0, 1).ToCharArray()[0];

                int cp = 0;
                int np = 0;

                if (c == 'z') break;
                else if (!hasZ && (i == lst.Count - 1))
                {
                    cp = int.Parse(lst[i].Substring(1, lst[i].Length - 1));
                    np = str.Length + 1;
                }
                else
                {
                    cp = int.Parse(lst[i].Substring(1, lst[i].Length - 1));
                    np = int.Parse(lst[i + 1].Substring(1, lst[i + 1].Length - 1));
                }

                string source = str.Substring(cp + 2, np - cp - 3);

                List<double> tmp;
                switch (c)
                {
                    case 'c':
                        tmp = ConverteData(source + " ", true, data[data.Count - 2], data[data.Count - 1]);
                        break;
                    case 'L':
                        tmp = ConverteData(source + " ", false);
                        break;
                    case 'M':
                        tmp = ConverteData(source + " ");
                        break;
                    default:
                        tmp = data.Count != 0 ? ConverteData(source + " ", false, data[data.Count - 2], data[data.Count - 1]) : ConverteData(source + " ", false);
                        break;
                }

                data.AddRange(tmp);
            }

            return data;
        }

        private static List<double> ConverteData(string source)
        {
            int start = 0;
            int size = source.Length;

            var list = new List<double>();

            for (int i = 0; i < size; ++i)
            {
                if (source[i] != ' ') continue;

                string s = source.Substring(start, i - start);
                start = i + 1;
                int dot = s.IndexOf(',');
                if (dot == 0 || dot == -1) break;
                double x = Double.Parse(s.Substring(0, dot), CultureInfo.InvariantCulture);
                double y = Double.Parse(s.Substring(dot + 1, s.Length - dot - 1), CultureInfo.InvariantCulture);

                list.Add(x);
                list.Add(y);
            }

            return list;
        }
        private static List<double> ConverteData(string source, bool isCicle, double aX = 0, double aY = 0)
        {
            int start = 0;
            int size = source.Length;

            var list = new List<double>();
            bool isFirst = true;
            for (int i = 0; i < size; ++i)
            {
                if (source[i] == ' ')
                {
                    string s = source.Substring(start, i - start);
                    start = i + 1;
                    int dot = s.IndexOf(',');
                    if (dot == 0 || dot == -1) break;
                    double x = Double.Parse(s.Substring(0, dot), CultureInfo.InvariantCulture);
                    double y = Double.Parse(s.Substring(dot + 1, s.Length - dot - 1), CultureInfo.InvariantCulture);

                    if (isFirst || isCicle)
                    {
                        list.Add(aX + x);
                        list.Add(aY + y);

                        isFirst = false;
                    }
                    else
                    {
                        int count = list.Count;
                        list.Add(list[count - 2] + x);
                        list.Add(list[count - 1] + y);
                    }
                }
            }

            return list;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Linq;
using MoreLinq;

namespace DataMining
{
    class Program
    {
        public static List<int[]> fileContent = new List<int[]>();
        static void Main()
        {
            //initFile("..\\..\\opt0.txt");
            //writeToFileRandom();
            //writeToFileHalf("half0");
            //for (int i = 1; i < 10; i++)
            //{
            //    fileContent = new List<int[]>();
            //    initFile("..\\..\\opt"+i+".txt");
            //    writeToFileHalf("half"+i);
            //}
            //for (int i = 0; i < 10; i++)
            //{
            //    fileContent = new List<int[]>();
            //    initFile("..\\..\\half" + i + ".txt");
            //    Console.WriteLine("CORRELATIONS FOR FILE " + i);
            //    allCorrelations();
            //    Console.WriteLine("");
            //}
            //initFile("..\\..\\optAll.txt");
            //selectFields(new int[33] { 36, 28, 19, 20, 62, 45, 26, 18, 41, 10, 21, 5, 54, 42, 60, 53, 35, 50, 29, 37, 43, 33, 46, 6, 27, 38, 44, 2,34, 3, 59, 13, 14  }, "all5Base");
            Console.WriteLine("done");
        }

        private static void initFile(string file)
        {
            using (FileStream reader = File.OpenRead(file)) // mind the encoding - UTF8
            using (TextFieldParser parser = new TextFieldParser(reader))
            {
                parser.TrimWhiteSpace = true; // if you want
                parser.Delimiters = new[] { " " };
                parser.HasFieldsEnclosedInQuotes = true;
                while (!parser.EndOfData)
                {
                    string[] line = parser.ReadFields();
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (line[i] == "")
                        {
                            string[] temp = new string[i];
                            for(int j = 0; j< i; j++)
                            {
                                temp[j] = line[j];
                            }
                            line = temp;
                            break;
                        }
                    }
                    int[] intLine = new int[line.Length];
                    for (int i = 0; i < line.Length; i ++)
                    {
                        intLine[i] = Int32.Parse(line[i]);
                    }
                    fileContent.Add(intLine);
                }
            }
        }

        private static void selectFields(int [] fields, string fileName)
        {
            StreamWriter writer = new StreamWriter(@"..\\..\\" + fileName+ ".txt");
            foreach(var line in fileContent)
            {
                foreach (int field in fields)
                {
                    writer.Write(line[field] + " ");
                }
                writer.WriteLine(line[line.Length - 1]);
            }
            writer.Close();
        }

        private static void writeToFileRandom()
        {
            StreamWriter writer = new StreamWriter(@"..\\..\\randomAll.txt");
            Random rand = new Random();
            while(fileContent.Count > 0)
            {
                int line = rand.Next(0, fileContent.Count);
                int[] temp = fileContent[line];
                for(int i = 0; i<temp.Length - 1;i++)
                {
                    writer.Write(temp[i] + " ");
                }
                writer.WriteLine(temp[temp.Length - 1]);
                fileContent.RemoveAt(line);
            }
            writer.Close();
        }

        private static void writeToFileHalf(string fileName)
        {
            StreamWriter writer = new StreamWriter(@"..\\..\\" + fileName + ".txt");
            for (int j = 0; j < fileContent.Count/2; j++)
            {
                int[] temp = fileContent[j];
                for (int i = 0; i < temp.Length - 1; i++)
                {
                    writer.Write(temp[i] + " ");
                }
                writer.WriteLine(temp[temp.Length - 1]);
            }
            writer.Close();
        }

        private static void allCorrelations()
        {

            if (fileContent.Count > 0)
            {
                double[] correlations = new double[fileContent[0].Length];
                for (int i = 0; i < fileContent[0].Length - 1; i++)
                {
                    correlations[i] = pearsonCorrelationOf(i, fileContent[0].Length - 1);
                    //Console.WriteLine("Correlation for" + i + " is: " + correlations[i]);
                }
                //Array.Sort(correlations);
                //Console.WriteLine(string.Join(",", correlations));
                getTop(correlations, 5);
            }
        }

        private static void getTop(double[] list, int size)
        {
            Dictionary<int, double> bestValues = new Dictionary<int, double>();
            int i = 0;
            for (i = 0; (i < size && i < list.Length); i++)
            {
                bestValues.Add(i, list[i]);
            }
            if (size < list.Length)
            {
                for (i = size; i < list.Length; i++)
                {
                    KeyValuePair<int, double> minBestValue = bestValues.MinBy(kvp => Math.Abs(kvp.Value));
                    if (Math.Abs(list[i]) > Math.Abs(minBestValue.Value))
                    {
                        bestValues.Add(i, list[i]);
                        bestValues.Remove(minBestValue.Key);
                    }
                }
            }
            //int[] fields = new int[size];
            //i = 0;
            foreach(KeyValuePair<int, double>  kvp in bestValues)
            {
                Console.WriteLine("Field " + kvp.Key + ": " + kvp.Value);
                //fields[i++] = kvp.Key;
            }
            //return fields;
        }

        private static double pearsonCorrelationOf(int field1, int field2)
        {
            double mean1 = meanOfField(field1);
            double mean2 = meanOfField(field2);
            double std1 = stdOfField(field1);
            double std2 = stdOfField(field2);
            double sum = 0;
            if (std1 != 0 && std2 != 0)
            {
                foreach (int[] row in fileContent)
                {
                    sum += ((row[field1] - mean1) / std1) * ((row[field2] - mean2) / std2);
                }
            }
            return sum / (fileContent.Count - 1);
        }

        private static double stdOfField(int field)
        {
            double mean = meanOfField(field);
            double sum = 0;
            foreach(int[] row in fileContent)
            {
                sum += (row[field] - mean) * (row[field] - mean);
            }
            return Math.Sqrt( sum / (fileContent.Count - 1));
        }

        private static double meanOfField(int field)
        {
            double sum = 0;
            foreach(int[] row in fileContent)
            {
                sum += row[field];
            }
            return sum / fileContent.Count;
        }

        /*
        private static double getAccuracy()
        {
            double accuracy = 0;
            for(int i = 0; i < fileContent.Count; i++)
            {
                if (isClosestAccurate(i))
                {
                    accuracy++;
                }
                else
                {
                    Console.WriteLine(i + " is not accurate");
                }
            }
            return 100 * accuracy / fileContent.Count;
        }

        private static double getAccuracy(int classValue)
        {
            double accuracy = 0;
            int noElements = 0;
            for (int i = 0; i < fileContent.Count; i++)
            {
                if (Convert.ToInt32(fileContent[i][fileContent[i].Length - 1]) == classValue)
                {
                    noElements++;
                    if (isClosestAccurate(i))
                    {
                        accuracy++;
                    }
                    else
                    {
                        Console.WriteLine(i + " is not accurate");
                    }
                }
            }
            return 100 * accuracy / noElements;
        }

        private static bool isClosestAccurate(int lineNo)
        {
            string[] current = fileContent[lineNo];
            double[] doubleCurrent = new double[current.Length - 1];
            for(int i = 0; i < current.Length - 1; i++)
            {
                doubleCurrent[i] = Convert.ToDouble(current[i]);
            }
            double? minDiff = null;
            int? closestLineNo = null;  
            for(int i = 0; i < fileContent.Count; i++)
            {
                if (i != lineNo)
                {
                    double diff = 0;
                    for (int j = 0; j < doubleCurrent.Length; j++)
                    {
                        double temp = Convert.ToDouble(fileContent[i][j]);
                        diff += (doubleCurrent[j] - temp) * (doubleCurrent[j] - temp);
                    }
                    if (minDiff == null)
                    {
                        minDiff = diff;
                        closestLineNo = i;
                    }
                    else
                    {
                        if (diff < minDiff)
                        {
                            minDiff = diff;
                            closestLineNo = i;
                        }
                    }
                }

            }
            if(Convert.ToDouble(fileContent[(int)closestLineNo][current.Length-1]) == Convert.ToDouble(current[current.Length-1]))
            {
                return true;
            }
            return false;

        }

        private static void minMaxNormalize()
        {
            StreamWriter writer = new StreamWriter(@"..\\..\\don'toverride.data");
            int noColumns = fileContent[0].Length - 1;
            double[] mins = new double[noColumns];
            double[] maxes = new double[noColumns];
            for(int i = 0; i < noColumns; i++)
            {
                mins[i] = (double)findMinOfColumn(i);
                maxes[i] = (double)findMaxOfColumn(i);
            }
            foreach (var line in fileContent)
            {
                int i;
                for(i = 0; i < noColumns; i++)
                {
                    double value = (Convert.ToDouble(line[i]) - mins[i]) / (maxes[i] - mins[i]);
                    writer.Write(value + ",");
                }
                writer.WriteLine(line[i]);
            }
            writer.Close();
        }

        private static double? findMinOfColumn(int i)
        {
            double? min = null;
            foreach (var line in fileContent)
            {
                double temp = Convert.ToDouble(line[i]);
                if (min == null)
                {
                    min = temp;
                }else
                {
                    if(min > temp)
                    {
                        min = temp;
                    } 
                }
            }
            return min;
        }

        private static double? findMaxOfColumn(int i)
        {
            double? max = null;
            foreach (var line in fileContent)
            {
                double temp = Convert.ToDouble(line[i]);
                if (max == null)
                {
                    max = temp;
                }
                else
                {
                    if (max < temp)
                    {
                        max = temp;
                    }
                }
            }
            return max;
        }

        private static void writeToFileRandom()
        {
            StreamWriter writer = new StreamWriter(@"..\\..\\careDontOverride.arff");
            int i = 0;
            int r = 0;
            Random rand = new Random();
            r = rand.Next(1, 11);
            foreach (var line in fileContent)
            {
                i++;
                if (r == i)
                {
                    int j;
                    for(j = 0; j < line.Length - 1; j++)
                    {
                        writer.Write(line[j] + ",");
                    }
                    writer.WriteLine(line[j]);
                }
                if (i == 11)
                {
                    r = rand.Next(1, 11);
                    i = 0;
                }
            }
            writer.Close();
        }

        private static void do5BinDistribution(float classValue, int fieldNumber)
        {
            int[] noOfElements = new int[5];
            int noOfClassFields = 0;
            double maxofCol = 0.2;//(double)findMaxOfColumn(fieldNumber);
            foreach(var line in fileContent)
            {
                if (Convert.ToDouble(line[line.Length - 1]) == classValue)
                {
                    noOfClassFields++;
                    double value = Convert.ToDouble(line[fieldNumber]);
                    if(value < (maxofCol * 1/5))
                    {
                        noOfElements[0]++;
                    }else
                    {
                        if(value < (maxofCol* 2/5))
                        {
                            noOfElements[1]++;
                        }else
                        {
                            if(value < (maxofCol * 3 / 5))
                            {
                                noOfElements[2]++;
                            }else
                            {
                                if(value < (maxofCol * 4 / 5))
                                {
                                    noOfElements[3]++;
                                }else
                                {
                                    noOfElements[4]++;
                                }
                            }
                        }
                    }
                }
            }
            double[] distribution = new double[5];
            for(int i = 0; i < 5; i++)
            {
                distribution[i] = 100 * noOfElements[i] / noOfClassFields;
            }
            Console.WriteLine("Percentage for 0.2: " + distribution[0]);
            Console.WriteLine("Percentage for 0.4: " + distribution[1]);
            Console.WriteLine("Percentage for 0.6: " + distribution[2]);
            Console.WriteLine("Percentage for 0.8: " + distribution[3]);
            Console.WriteLine("Percentage for 1: " + distribution[4]);
        }*/
    }
}

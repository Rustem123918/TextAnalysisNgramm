using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            var parsedText = new List<List<string>>();
            var dictionaryOfMostFrequentWords = new Dictionary<string, string>();
            string finalPhrase = "";

            string text = File.ReadAllText("testkek.txt");
            parsedText = Parser(text);

            /*
            foreach (var a in parsedText)
            {
                foreach (var b in a)
                    Console.Write(b + " ");
                Console.WriteLine();
            }
            Console.WriteLine(@"////////////////ТЕКСТ РАСПАРСЕН//////////////");
            */

            Console.WriteLine("ТЕКСТ РАСПАРСЕН");
            dictionaryOfMostFrequentWords = GetMostFrequentNextWords(parsedText);
            
            while(true)
            {
                Console.WriteLine("Введите слово или несколько слов: ");
                string inputText = Console.ReadLine();
                finalPhrase = ContinuePhrase(dictionaryOfMostFrequentWords, inputText, 14);
                Console.WriteLine(finalPhrase);
            }
            
            Console.ReadKey();
        }

        static List<List<string>> Parser(string text)
        {
            var result = new List<List<string>>();

            text = text.ToLower();

            //Вспомогательные переменные
            string obama = "";

            //param1 - символы разделяющие предложения
            //param2 - символы разделяющие слова
            char[] param1 = { '.', '?', '!', ';', ':', '(', ')' };
            char[] param2 = { ' ', ',',
                '\t', '\r', '\n',
                '^', '#', '$',
                '-', '+', '=',
                '1', '2', '3', '4', '5', '6', '7', '8', '9', '0',
                '”', '“', '\"' }; //КЕКнутые ковычки в ГарриПоттере

            var oneSentence = new List<string>();

            //Разделяю весь текст на предложения
            foreach (var sentence in text.Split(param1))
            {

                if (!String.IsNullOrEmpty(obama))
                {
                    oneSentence.Add(obama);
                    obama = "";
                }

                //Если предложение не пустое и оно состоит из символов
                if (!String.IsNullOrEmpty(sentence))
                {
                    //Разделяю предложение на слова
                    foreach (var word in sentence.Split(param2))
                    {
                        //отдельно обрабатываю Mr и Mrs. Пока под вопросом нужно ли это ?????????????????
                        if (word == "mr" || word == "mrs")
                        {
                            obama = word;
                            continue;
                        }
                        if (!String.IsNullOrEmpty(word))
                            oneSentence.Add(word);
                    }
                    result.Add(oneSentence);
                }

                oneSentence = new List<string>();
            }
            return result;
        }

        static Dictionary<string, string> GetMostFrequentNextWords(List<List<string>> text)
        {
            var result = new Dictionary<string, string>();

            var tempDictionary = new Dictionary<string, Dictionary<string, int>>();
            //Ключ - значение для биграмм
            string key2 = "";
            string value2 = "";

            //Ключ - значение для триграмм
            string key3 = "";
            string value3 = "";

            //Выделяю биграммы и триграммы и записываю их в один словарь
            foreach (var sentence in text)
            {
                if (sentence.Count != 0)
                {
                    //Выделяю биграммы
                    for (int i = 0; i < sentence.Count - 1; i++)
                    {
                        key2 = sentence[i];
                        value2 = sentence[i + 1];
                        if (!tempDictionary.ContainsKey(key2))
                        {
                            tempDictionary[key2] = new Dictionary<string, int>();
                            tempDictionary[key2].Add(value2, 1);
                        } 
                        else
                        {
                            if(!tempDictionary[key2].ContainsKey(value2))
                            {
                                tempDictionary[key2].Add(value2, 1);
                            }
                            else
                            {
                                tempDictionary[key2][value2]++;
                            }
                        }
                    }

                    //Выделяю триграммы
                    for (int i = 0; i < sentence.Count - 2; i++)
                    {
                        key3 = sentence[i] + " " + sentence[i + 1];
                        value3 = sentence[i + 2];
                        if (!tempDictionary.ContainsKey(key3))
                        {
                            tempDictionary[key3] = new Dictionary<string, int>();
                            tempDictionary[key3].Add(value3, 1);
                        }
                        else
                        {
                            if (!tempDictionary[key3].ContainsKey(value3))
                            {
                                tempDictionary[key3].Add(value3, 1);
                            }
                            else
                            {
                                tempDictionary[key3][value3]++;
                            }
                        }
                    }
                }
            }
            /*
            Console.WriteLine("\nВСЕ БИГРАММЫ И ТРИГРАММЫ\n");
            foreach (var pair in tempDictionary)
            {
                foreach (var x in pair.Value)
                {
                    Console.WriteLine(pair.Key + " -> " + x.Key + ":" + x.Value);
                }
            }*/

            //Фильтрация
            int max = 0;
            string temp = "";
            bool ifFirst = true;

            foreach(var pair in tempDictionary)
            {
                max = 0;
                temp = "";
                ifFirst = true;
                foreach(var pairInside in pair.Value)
                {
                    //Сначала сравниваем по частоте, а потом лексикографически
                    if(ifFirst) { max = pairInside.Value; temp = pairInside.Key; ifFirst = false; continue; }
                    if(pairInside.Value < max) { continue; }
                    if(pairInside.Value > max) { max = pairInside.Value; temp = pairInside.Key; continue; }
                    if(pairInside.Value == max)
                    {
                        if(String.CompareOrdinal(pairInside.Key, temp) < 0) { temp = pairInside.Key; continue; }
                    }
                }
                result.Add(pair.Key, temp);
            }
            /*
            Console.WriteLine("\nОТФИЛЬТРОВАННЫЙ СЛОВАРЬ\n");
            foreach(var pair in result)
            {
                Console.WriteLine(pair.Key + " " + pair.Value);
            }*/

            return result;
        }

        static string ContinuePhrase(Dictionary<string, string> nextWords, string phraseBeginning, int wordsCount)
        {
            //В tempList я заношу все слова из предложения, чтобы удобно было менять ключи
            var tempList = new List<string>();
            string key2 = "";
            string key3 = "";
            int count = 0;

            foreach(var word in phraseBeginning.Split(' '))
            {
                tempList.Add(word);
            }

            if(tempList.Count >= 2)
            {
                key2 = tempList[tempList.Count - 1];
                key3 = tempList[tempList.Count - 2] + " " + tempList[tempList.Count - 1];
            }
            else
            {
                key2 = tempList[0];
            }

            //Сам алгоритм
            while(count < wordsCount)
            {
                if (nextWords.ContainsKey(key3))
                {
                    tempList.Add(nextWords[key3]);
                    phraseBeginning += " " + nextWords[key3];
                    key2 = nextWords[key3];
                    key3 = tempList[tempList.Count - 2] + " " + nextWords[key3];
                    count++;
                }
                else if (nextWords.ContainsKey(key2))
                {
                    tempList.Add(nextWords[key2]);
                    phraseBeginning += " " + nextWords[key2];
                    key3 = tempList[tempList.Count - 2] + " " + nextWords[key2];
                    key2 = nextWords[key2];
                    count++;
                }
                else
                {
                    break;
                }
            }
            
            return phraseBeginning;
        }
    }
}
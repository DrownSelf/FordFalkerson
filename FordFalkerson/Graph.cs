using System.Collections.Generic;
using System.Linq;

// Алгоритм Форда-Фалкерсона
namespace FordFalkerson
{
    class Graph
    {
        private Dictionary<int, Dictionary<int, int>> Links;
        private Queue<int> q;
        private int result;

        public Graph()
        {
            Links = new Dictionary<int, Dictionary<int, int>>();
            q = new Queue<int>();
        }

        private void Creating(int origin, int end, int thread)// Добавление вершин в граф
        {
            if (!Links.ContainsKey(origin))// Если словарь не содержит вершину, то создаем подсловарь со связями (вершины, с которыми он соединяется)
            {
                Dictionary<int, int> connection = new Dictionary<int, int>();
                connection.Add(end, thread);
                Links.Add(origin, connection);
            }
            else// Иначе, просто, добавляем нужную связь
                Links[origin].Add(end, thread);
        }

        public void AddLink(int origin, int end, int input, int output) // Добавление связей. Здесь содержится два creating, потому-что граф неориентированный (имеет связь в обе стороны). 
        {
            Creating(origin, end, input);
            Creating(end, origin, output);
        }

        private bool CheckVisited(int input, Dictionary<int, KeyValuePair<int, int>> thread, int path) // Проверка посещенных вершин
        {
            foreach (KeyValuePair<int, KeyValuePair<int, int>> vertex in thread)
                if (vertex.Key == input)
                    return false;
            if (path <= 0)
                return false;
            return true;
        }

        private void ReverseStream(Dictionary<int, KeyValuePair<int, int>> thread, int min) // Изменение потоков в пройденных вершинах. Например у нас есть вершина 1 и 2. Их поток 30/0.
        {// А минимальное значение первого пути 20. То поток поменяется на 10/20.
            foreach (KeyValuePair<int, KeyValuePair<int, int>> Link in thread)
            {
                if (thread.Keys.First() == Link.Key)
                    continue;
                Links[Link.Key][Link.Value.Key] += min;
                Links[Link.Value.Key][Link.Key] -= min;
            }
        }

        private void ClearPath(Dictionary<int, KeyValuePair<int, int>> thread) // Очистка пути для следующей итерации
        {
            foreach (KeyValuePair<int, KeyValuePair<int, int>> Link in thread)
                if (Link.Key != 1 && Link.Value.Value == int.MaxValue)
                    thread.Remove(Link.Key);
        }

        private void FindVertex(Dictionary<int, KeyValuePair<int, int>> thread, ref int max, ref int finding, int extracted) // Предполагалось отрефакторить код, дабы ввести отдельный проход по вершинам
        {// Но не получилось из-за большого количества локальных переменных
            foreach (KeyValuePair<int, int> Link in Links[extracted])
            {
                if (Link.Value > max && CheckVisited(Link.Key, thread, Link.Value))
                {
                    max = Link.Value;
                    finding = Link.Key;
                }
            }
        }

        public int FordFalkesrson(int begin, int end)
        {
            List<int> paths = new List<int>();
            bool flag = true;
            while (flag)
            {
                Dictionary<int, KeyValuePair<int, int>> stream = new Dictionary<int, KeyValuePair<int, int>>();
                q.Enqueue(begin);
                int prThread = int.MaxValue;
                int prVertex = int.MaxValue;
                while (q.Count != 0)
                {
                    int extracted = q.Peek();
                    int max = int.MinValue;
                    int finding = int.MinValue;

                    foreach (KeyValuePair<int, int> Link in Links[extracted])// Нахождение соседней вершины, у которой максимальный поток
                    {
                        if (Link.Value > max && CheckVisited(Link.Key, stream, Link.Value))
                        {
                            max = Link.Value;
                            finding = Link.Key;
                        }
                    }
                    if (finding == int.MinValue)// Проверка возникающая когда, все соседние потоки равны 0 (тупик)
                    {
                        if (stream.ContainsKey(extracted) && stream[extracted].Value == int.MaxValue || extracted == begin)
                        {// Проверка является ли это концом функции. Возникает тогда, когда от исходный вершины нельзя выйти (связь с соседними вершинами равна нулю)
                            foreach (int sum in paths)
                                result += sum;
                            flag = false;
                        }
                        else
                        {// Иначе возвращаемся на вершину назад
                            KeyValuePair<int, int> connection = new KeyValuePair<int, int>(prVertex, int.MaxValue); // Пара, которая заносится в путь
                            stream.Add(extracted, connection);
                            q.Enqueue(stream[extracted].Key);
                            q.Dequeue();
                            continue;
                        }
                    }
                    else
                    {// Блок после прохода вершины. Занесение связи в путь
                        KeyValuePair<int, int> connection = new KeyValuePair<int, int>(prVertex, prThread); // Пара, которая заносится в путь
                        if (!stream.ContainsKey(extracted))
                            stream.Add(extracted, connection);
                        prVertex = extracted;
                        prThread = max;
                        if (finding == end)// Если мы пришли к конечной вершине, то добавляем нужные данные и завершаем обход.
                        {
                            connection = new KeyValuePair<int, int>(prVertex, prThread);
                            stream.Add(finding, connection);
                            q.Dequeue();
                            break;
                        }
                        q.Enqueue(finding);
                    }
                    q.Dequeue();
                }
                int min = int.MaxValue;// Нахождение мимального значения в пройденном пути
                foreach (KeyValuePair<int, KeyValuePair<int, int>> Link in stream)
                    if (min > Link.Value.Value)
                        min = Link.Value.Value;
                paths.Add(min);// Добавление минимального значения
                ClearPath(stream); // Очищение пути для след.прохода
                ReverseStream(stream, min); // Изменение связей в пройденных вершинах
            }
            return result;
        }
    }
}
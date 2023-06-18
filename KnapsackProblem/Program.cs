using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnapsackProblem
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int[] weights, values;
            int n, W;
            List<Item> items = new List<Item>();

            var streamReader = new StreamReader("ks_19_0.txt");

            string[] splitLine = streamReader.ReadLine().Split(' ');

            n = Convert.ToInt32(splitLine[0]); //nesne adedi
            W = Convert.ToInt32(splitLine[1]); //çanta ağırlığı

            weights = new int[n]; //nesnelerin her birinin ağırlıklarının tutulduğu dizi
            values = new int[n]; //nesnelerin her birinin değerlerinin tutulduğu dizi

            //dosyadan her bir nesnenin ağırlık ve değerlerinin okunup dizilere atanması
            for (int i = 0; i < n; i++)
            {
                splitLine = streamReader.ReadLine().Split(' ');
                values[i] = Convert.ToInt32(splitLine[0]);
                if (n == 200)
                    weights[i] = Convert.ToInt32(splitLine[2]);
                else
                    weights[i] = Convert.ToInt32(splitLine[1]);

                if (n == 10000)
                {
                    items.Add(new Item(weights[i], values[i]));
                }
            }
            var time = new System.Diagnostics.Stopwatch();
            time.Start();
            if (n == 10000) //çok yüksek boyutlu dosya, bunun için dinamik programlama bellek açısından oldukça maliyetli olduğundan greedy çözüme göre hesaplandı.
            {
                GreedyAlgorithm(items, n, W);
            }
            else //yüksek boyutlu olmayan dosyalar için ise en iyi çözüme ulaşmak açısından dinamik programlama ile çözüme gidildi.
            {
                DynamicProgramming(n, W, values, weights);
            }
            time.Stop();
            Console.WriteLine("Çalışma süresi: " + time.ElapsedMilliseconds + "ms");
            Console.Read();
        }
        static void DynamicProgramming(int itemCount, int maxCapacity, int[] values, int[] weights) //knapsack problemini çözmede en iyi çözümü üretir ancak veri boyutu çok büyürse bellek kullanımı açısında oldukça maliyetli olur.
        {
            int[,] table = new int[itemCount + 1, maxCapacity + 1]; //dinamik programlama tablosunu oluşturur.

            for (int w = 0; w < maxCapacity; w++)
            {
                table[0, w] = 0;
            }

            //dinamik programlama ile knapsack probleminin çözümü
            for (int i = 1; i <= itemCount; i++)
            {
                for (int w = 1; w <= maxCapacity; w++)
                {
                    if (weights[i - 1] <= w)
                    {
                        table[i, w] = Math.Max(values[i - 1] + table[i - 1, w - weights[i - 1]], table[i - 1, w]);
                    }
                    else
                    {
                        table[i, w] = table[i - 1, w];
                    }
                }
            }

            // Seçilen nesneleri bulma
            int[] selected = new int[itemCount];
            int maxVal = table[itemCount, maxCapacity];
            int remainingW = maxCapacity;

            for (int i = itemCount; i >= 1; i--)
            {
                if (maxVal != table[i - 1, remainingW])
                {
                    selected[i - 1] = 1;
                    maxVal -= values[i - 1];
                    remainingW -= weights[i - 1];
                }
            }
            // Sonuçları yazdır
            Console.WriteLine("Optimal value değeri: " + table[itemCount, maxCapacity]); //en yüksek değer
            Console.WriteLine("Optimal çözüme dahil edilenler(0,1): " + string.Join(" ", selected)); //çantaya koyulan elemanlar(0,1)
            Console.Write("Optimal çözüme dahil edilen elemanların sıraları: "); //çantaya koyulan elemanların kaçıncı sırada oldukları
            for (int i = 0; i < selected.Count(); i++)
            {
                if (selected[i] == 1)
                {
                    Console.Write((i + 1) + " ");
                }
            }
            Console.WriteLine();
        }

        static void GreedyAlgorithm(List<Item> items, int itemCount, int maxCapacity) //10000 boyutlu dosya için belleği verimli kullanır, her adımda en yüksek değere(value/weight) sahip öğeyi seçerek devam eder. ancak her zaman en iyi sonucu vermeyebilir.
        {
            items.Sort((x, y) => y.ratio.CompareTo(x.ratio)); //nesneler verimlilik oranlarına göre sıralanır.

            int currentWeight = 0;
            int optimalValue = 0;
            int[] selected = new int[itemCount];

            // Çantaya sığan,en değerli nesneleri çantaya ekle
            foreach (Item item in items)
            {
                if (currentWeight + item.weight <= maxCapacity)
                {
                    currentWeight += item.weight;
                    optimalValue += item.value;
                    selected[items.IndexOf(item)] = 1;
                }
                else //çanta kapasitesi aşılırsa
                {
                    //eğer çanta kapasitesi aşılırsa mevcut eşyanın bir kısmını seçip çantanın tam doldurulması ve iyi bir optimal çözüme yaklaşılması

                    double fraction = (double)(maxCapacity - currentWeight) / item.weight; //seçilen öğe çanta kapasitesini aşıyorsa bu öğenin bir kısmının seçilmesi, kesirsel değer
                    currentWeight += (int)(fraction * item.weight); 
                    optimalValue += (int)(fraction * item.value);
                    selected[items.IndexOf(item)] = Convert.ToInt32(fraction);
                    break;
                }
            }
            // Sonuçları yazdır
            Console.WriteLine("Optimal value değeri: " + optimalValue); //toplam değer
            Console.WriteLine("Optimal çözüme dahil edilenler(0,1): " + string.Join(" ", selected)); //çantaya eklenen elemanların listesi
            Console.Write("Seçilen elemanların sıraları: ");
            for (int i = 0; i < selected.Count(); i++)
            {
                if (selected[i] == 1)
                {
                    Console.Write((i + 1) + " ");
                }
            }
            Console.WriteLine();
            Console.WriteLine("elde edilen toplam ağırlık: " + currentWeight);
        }
    }
    class Item //her nesnenin ağırlık,değer ve değer/ağırlık oranını tutar.
    {
        public int weight;
        public int value;
        public double ratio; //nesnenin değeri, verimlilik oranı

        public Item(int w, int v)
        {
            weight = w;
            value = v;
            ratio = (double)value / weight;
        }
    }
}

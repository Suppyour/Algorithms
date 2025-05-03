using System.Diagnostics;

namespace FirstApp;

class LevelOne
{
    static void Main()
    {
        int[] sizes = { 1000, 5000, 10000, 20000 };
        var random = new Random();

        foreach (var size in sizes)
        {
            int[] arr = new int[size];
            for (int i = 0; i < size; i++)
                arr[i] = random.Next(0, 10000);

            Console.WriteLine($"\nРазмер массива: {size}");

            var bubbleTime = MeasureTime(BubbleSort, arr);
            var quickTime = MeasureTime(ints => QuickSort(ints, 0, ints.Length - 1), arr);
            var countingTime = MeasureTime(CountingSort, arr);

            Console.WriteLine($"Пузырьковая сортировка: {bubbleTime:F6} секунд");
            Console.WriteLine($"Быстрая сортировка: {quickTime:F6} секунд");
            Console.WriteLine($"Сортировка подсчетом: {countingTime:F6} секунд");
        }
    }

    // --- Алгоритмы сортировки ---
    private static void BubbleSort(int[] arr)
    {
        int n = arr.Length;
        for (int i = 0; i < n - 1; i++)
        for (int j = 0; j < n - i - 1; j++)
            if (arr[j] > arr[j + 1])
                (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);
    }

    private static void QuickSort(int[] arr, int low, int high)
    {
        if (low < high)
        {
            int pi = Partition(arr, low, high);

            QuickSort(arr, low, pi - 1);
            QuickSort(arr, pi + 1, high);
        }
    }

    private static int Partition(int[] arr, int low, int high)
    {
        int pivot = arr[high];
        int i = (low - 1);

        for (int j = low; j <= high - 1; j++)
        {
            if (arr[j] < pivot)
            {
                i++;
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
        }

        (arr[i + 1], arr[high]) = (arr[high], arr[i + 1]);
        return (i + 1);
    }

    private static void CountingSort(int[] arr)
    {
        if (arr.Length == 0)
            return;

        int min = arr[0];
        int max = arr[0];
        foreach (var num in arr)
        {
            if (num < min) min = num;
            if (num > max) max = num;
        }

        int range = max - min + 1;
        int[] count = new int[range];
        int[] output = new int[arr.Length];

        for (int i = 0; i < arr.Length; i++)
            count[arr[i] - min]++;

        for (int i = 1; i < count.Length; i++)
            count[i] += count[i - 1];

        for (int i = arr.Length - 1; i >= 0; i--)
        {
            output[count[arr[i] - min] - 1] = arr[i];
            count[arr[i] - min]--;
        }

        for (int i = 0; i < arr.Length; i++)
            arr[i] = output[i];
    }

    private static double MeasureTime(Action<int[]> sortFunc, int[] original)
    {
        double totalSeconds = 0;
        for (int i = 0; i < 10; i++)
        {
            var copy = new int[original.Length];
            Array.Copy(original, copy, original.Length);

            var stopwatch = Stopwatch.StartNew();
            sortFunc(copy);
            stopwatch.Stop();

            totalSeconds += stopwatch.Elapsed.TotalSeconds;
        }

        return totalSeconds / 10;
    }
}
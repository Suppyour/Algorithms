using System.Diagnostics;
using System.Text;

namespace SecondApp;

class SubstringSearchBenchmark
{
    const int Iterations = 10;

    static void Main()
    {
        var random = new Random();
        var testCases = new[] { 1_000, 10_000, 100_000, 1_000_000 };

        foreach (var size in testCases)
        {
            var text = RandomString(size, random);
            var pattern = text.Substring(size / 2, 10);

            Console.WriteLine($"Тест на строке длины {size}, подстрока длины {pattern.Length}");

            RunBenchmark("Простой", text, pattern, NaiveSearch);
            RunBenchmark("Боуэр-Мур", text, pattern, BoyerMooreSearch);
            RunBenchmark("Рабин-Карп", text, pattern, RabinKarpSearch);
        }
    }

    static void RunBenchmark(string name, string text, string pattern, Func<string, string, int> searchMethod)
    {
        var sw = new Stopwatch();
        long totalTicks = 0;

        for (int i = 0; i < Iterations; i++)
        {
            sw.Restart();
            searchMethod(text, pattern);
            sw.Stop();
            totalTicks += sw.ElapsedTicks;
        }

        double avgMs = (totalTicks / (double)Iterations) / Stopwatch.Frequency * 1000;
        Console.WriteLine($"{name,-12}: {avgMs:F3} мс");
    }

    static string RandomString(int length, Random rand)
    {
        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++)
            sb.Append((char)('a' + rand.Next(26)));
        return sb.ToString();
    }

    static int NaiveSearch(string text, string pattern)
    {
        for (int i = 0; i <= text.Length - pattern.Length; i++)
        {
            int j;
            for (j = 0; j < pattern.Length; j++)
                if (text[i + j] != pattern[j])
                    break;
            if (j == pattern.Length)
                return i;
        }
        return -1;
    }

    static int RabinKarpSearch(string text, string pattern)
    {
        const int d = 256;
        const int q = 101;

        int m = pattern.Length;
        int n = text.Length;
        int p = 0, t = 0, h = 1;

        for (int i = 0; i < m - 1; i++)
            h = (h * d) % q;

        for (int i = 0; i < m; i++)
        {
            p = (d * p + pattern[i]) % q;
            t = (d * t + text[i]) % q;
        }

        for (int i = 0; i <= n - m; i++)
        {
            if (p == t)
            {
                int j;
                for (j = 0; j < m; j++)
                    if (text[i + j] != pattern[j])
                        break;
                if (j == m)
                    return i;
            }

            if (i < n - m)
            {
                t = (d * (t - text[i] * h) + text[i + m]) % q;
                if (t < 0)
                    t += q;
            }
        }

        return -1;
    }

    static int BoyerMooreSearch(string text, string pattern)
    {
        int[] badChar = BuildBadCharTable(pattern);
        int m = pattern.Length;
        int n = text.Length;

        int shift = 0;
        while (shift <= n - m)
        {
            int j = m - 1;

            while (j >= 0 && pattern[j] == text[shift + j])
                j--;

            if (j < 0)
                return shift;
            else
                shift += Math.Max(1, j - badChar[text[shift + j]]);
        }

        return -1;
    }

    static int[] BuildBadCharTable(string pattern)
    {
        const int ALPHABET_SIZE = 256;
        int[] badChar = new int[ALPHABET_SIZE];
        for (int i = 0; i < ALPHABET_SIZE; i++)
            badChar[i] = -1;

        for (int i = 0; i < pattern.Length; i++)
            badChar[pattern[i]] = i;

        return badChar;
    }
}
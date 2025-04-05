using System;
using System.Collections.Generic;
using ChainMapLib;

namespace ChainMapApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Przykład 1: Podstawowe użycie z dwoma słownikami
            Console.WriteLine("Przykład 1: Podstawowe użycie z dwoma słownikami");
            var dict1 = new Dictionary<string, string> { { "a", "1" }, { "b", "2" }, { "c", "3" } };
            var dict2 = new Dictionary<string, string> { { "b", "22" }, { "c", "33" }, { "d", "44" } };

            var chainMap1 = new ChainMap<string, string>(dict1, dict2);

            // Główny słownik jest pusty, wartości pochodzą z dict1 (pierwszy dodany)
            Console.WriteLine($"chainMap1[\"a\"]: {chainMap1["a"]}"); // 1 (z dict1)
            Console.WriteLine($"chainMap1[\"b\"]: {chainMap1["b"]}"); // 2 (z dict1)
            Console.WriteLine($"chainMap1[\"c\"]: {chainMap1["c"]}"); // 3 (z dict1)
            Console.WriteLine($"chainMap1[\"d\"]: {chainMap1["d"]}"); // 44 (z dict2)

            // Modyfikacja wartości w głównym słowniku
            chainMap1["b"] = "222";
            Console.WriteLine($"chainMap1[\"b\"] po modyfikacji: {chainMap1["b"]}"); // 222 (z głównego)

            Console.WriteLine("--------------------------------------------------");

            // Przykład 2: Dynamiczne dodawanie słowników z priorytetami
            Console.WriteLine("Przykład 2: Dynamiczne dodawanie słowników");
            var chainMap2 = new ChainMap<string, string>();

            // Dodajemy dict1 jako PIERWSZY słownik pomocniczy (index 0)
            chainMap2.AddDictionary(dict1, 0);
            // Dodajemy dict2 jako DRUGI słownik pomocniczy (index 1)
            chainMap2.AddDictionary(dict2, 1);

            Console.WriteLine($"chainMap2[\"a\"]: {chainMap2["a"]}"); // 1 (z dict1)
            Console.WriteLine($"chainMap2[\"b\"]: {chainMap2["b"]}"); // 2 (z dict1)
            Console.WriteLine($"chainMap2[\"d\"]: {chainMap2["d"]}"); // 44 (z dict2)

            Console.WriteLine("--------------------------------------------------");

            // Przykład 3: Próba dodania istniejącego klucza
            Console.WriteLine("Przykład 3: Próba dodania duplikatu do głównego słownika");
            try
            {
                chainMap2.Add("a", "nowa wartość");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"Błąd: {e.Message}"); // Klucz już istnieje
            }

            Console.WriteLine("--------------------------------------------------");

            // Przykład 4: Nadpisywanie wartości z niższych warstw
            Console.WriteLine("Przykład 4: Nadpisywanie wartości");
            chainMap2["d"] = "444";
            Console.WriteLine($"chainMap2[\"d\"]: {chainMap2["d"]}"); // 444 (z głównego)
            Console.WriteLine($"dict2[\"d\"]: {dict2["d"]}");         // 44 (oryginał niezmieniony)

            Console.WriteLine("--------------------------------------------------");

            // Przykład 5: Usuwanie ze słownika głównego
            Console.WriteLine("Przykład 5: Usuwanie klucza");
            chainMap2.Remove("d");
            Console.WriteLine($"chainMap2[\"d\"]: {chainMap2["d"]}"); // 44 (z dict2)

            Console.WriteLine("--------------------------------------------------");

            // Przykład 6: Scalanie słowników
            Console.WriteLine("Przykład 6: Scalanie słowników");
            var merged = chainMap2.Merge();
            foreach (var kvp in merged)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }

            Console.WriteLine("--------------------------------------------------");

            // Przykład 7: Zarządzanie priorytetami
            Console.WriteLine("Przykład 7: Zmiana priorytetów");
            var dict3 = new Dictionary<string, string> { { "a", "100" }, { "e", "500" } };
            chainMap2.AddDictionary(dict3, 0); // Najwyższy priorytet

            Console.WriteLine($"chainMap2[\"a\"]: {chainMap2["a"]}"); // 100 (z dict3)
            Console.WriteLine($"chainMap2[\"e\"]: {chainMap2["e"]}"); // 500 (z dict3)

            Console.WriteLine("--------------------------------------------------");

            // Przykład 8: Czyszczenie konfiguracji
            Console.WriteLine("Przykład 8: Resetowanie konfiguracji");
            chainMap2.ClearDictionaries();
            Console.WriteLine($"Liczba słowników: {chainMap2.CountDictionaries}"); // 0
            Console.WriteLine($"Czy klucz 'a' istnieje? {chainMap2.ContainsKey("a")}"); // False
        }
    }
}
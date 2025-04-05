using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChainMapLib;
using System;
using System.Collections.Generic;

namespace ChainMapTests
{
    [TestClass]
    public sealed class ChainMapFullCoverageTests
    {
        // Test 1: Konstruktor z pustymi parametrami
        [TestMethod]
        public void Constructor_WithNoDictionaries_InitializesMainDictionary()
        {
            var chainMap = new ChainMap<string, int>();
            Assert.IsNotNull(chainMap.MainDictionary);
            Assert.AreEqual(0, chainMap.Count);
        }

        // Test 2: Konstruktor z słownikami
        [TestMethod]
        public void Constructor_WithDictionaries_AddsThemInOrder()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 } };
            var dict2 = new Dictionary<string, int> { { "b", 2 } };
            var chainMap = new ChainMap<string, int>(dict1, dict2);

            Assert.AreEqual(2, chainMap.CountDictionaries);
        }

        // Test 3: Dodawanie słownika na konkretną pozycję
        [TestMethod]
        public void AddDictionary_AtSpecificIndex_InsertsCorrectly()
        {
            var chainMap = new ChainMap<string, int>();
            var dict1 = new Dictionary<string, int> { { "a", 1 } };

            chainMap.AddDictionary(dict1, 0);
            Assert.AreEqual(1, chainMap.CountDictionaries);
        }

        // Test 4: Dodawanie słownika z ujemnym indeksem
        [TestMethod]
        public void AddDictionary_WithNegativeIndex_AddsAtEnd()
        {
            var chainMap = new ChainMap<string, int>();
            var dict1 = new Dictionary<string, int> { { "a", 1 } };

            chainMap.AddDictionary(dict1, -1);
            Assert.AreEqual(1, chainMap.CountDictionaries);
        }

        // Test 5: Dodawanie słownika z indeksem większym niż liczba słowników
        [TestMethod]
        public void AddDictionary_WithIndexBeyondCount_AddsAtBeginning()
        {
            var chainMap = new ChainMap<string, int>();
            var dict1 = new Dictionary<string, int> { { "a", 1 } };

            chainMap.AddDictionary(dict1, 100);
            Assert.AreEqual(1, chainMap.CountDictionaries);
        }

        // Test 6: Usuwanie słownika
        [TestMethod]
        public void RemoveDictionary_ValidIndex_RemovesCorrectDictionary()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 } };
            var chainMap = new ChainMap<string, int>(dict1);

            Assert.IsTrue(chainMap.RemoveDictionary(0));
            Assert.AreEqual(0, chainMap.CountDictionaries);
        }

        // Test 7: Usuwanie nieistniejącego słownika
        [TestMethod]
        public void RemoveDictionary_InvalidIndex_ReturnsFalse()
        {
            var chainMap = new ChainMap<string, int>();
            Assert.IsFalse(chainMap.RemoveDictionary(100));
        }

        // Test 8: Czyszczenie słowników pomocniczych
        [TestMethod]
        public void ClearDictionaries_RemovesAllSecondary()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 } };
            var chainMap = new ChainMap<string, int>(dict1);

            chainMap.ClearDictionaries();
            Assert.AreEqual(0, chainMap.CountDictionaries);
        }

        // Test 9: Dostęp do klucza w głównym słowniku
        [TestMethod]
        public void Indexer_ReturnsValueFromMainDictionary_WhenKeyExistsInMain()
        {
            var chainMap = new ChainMap<string, int>();
            chainMap["a"] = 1;

            Assert.AreEqual(1, chainMap["a"]);
        }

        // Test 10: Dostęp do klucza w słowniku pomocniczym
        [TestMethod]
        public void Indexer_ReturnsValueFromSecondary_WhenKeyNotInMain()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 } };
            var chainMap = new ChainMap<string, int>(dict1);

            Assert.AreEqual(1, chainMap["a"]);
        }

        // Test 11: Dostęp do nieistniejącego klucza
        [TestMethod]
        public void Indexer_ThrowsKeyNotFoundException_WhenKeyMissing()
        {
            var chainMap = new ChainMap<string, int>();
            Assert.ThrowsException<KeyNotFoundException>(() => chainMap["a"]);
        }

        // Test 12: Sprawdzanie istnienia klucza w głównym słowniku
        [TestMethod]
        public void ContainsKey_ReturnsTrue_WhenKeyInMain()
        {
            var chainMap = new ChainMap<string, int>();
            chainMap["a"] = 1;

            Assert.IsTrue(chainMap.ContainsKey("a"));
        }

        // Test 13: Sprawdzanie istnienia klucza w słowniku pomocniczym
        [TestMethod]
        public void ContainsKey_ReturnsTrue_WhenKeyInSecondary()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 } };
            var chainMap = new ChainMap<string, int>(dict1);

            Assert.IsTrue(chainMap.ContainsKey("a"));
        }

        // Test 14: Sprawdzanie nieistniejącego klucza
        [TestMethod]
        public void ContainsKey_ReturnsFalse_WhenKeyMissing()
        {
            var chainMap = new ChainMap<string, int>();
            Assert.IsFalse(chainMap.ContainsKey("a"));
        }

        // Test 15: Sprawdzanie istnienia wartości w głównym słowniku
        

        // Test 18: Dodawanie nowego klucza do głównego słownika
        [TestMethod]
        public void TryAdd_AddsToMain_WhenKeyNew()
        {
            var chainMap = new ChainMap<string, int>();
            Assert.IsTrue(chainMap.TryAdd("a", 1));
            Assert.AreEqual(1, chainMap["a"]);
        }

        // Test 19: Próba dodania istniejącego klucza do głównego słownika
        [TestMethod]
        public void TryAdd_ReturnsFalse_WhenKeyInMain()
        {
            var chainMap = new ChainMap<string, int>();
            chainMap.TryAdd("a", 1);
            Assert.IsFalse(chainMap.TryAdd("a", 2));
        }

        // Test 20: Dodawanie klucza istniejącego w słowniku pomocniczym
        [TestMethod]
        public void TryAdd_AddsToMain_WhenKeyInSecondary()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 } };
            var chainMap = new ChainMap<string, int>(dict1);

            Assert.IsTrue(chainMap.TryAdd("a", 2));
            Assert.AreEqual(2, chainMap["a"]);
        }

        // Test 21: Usuwanie klucza z głównego słownika
        [TestMethod]
        public void Remove_KeyFromMain_ReturnsTrue()
        {
            var chainMap = new ChainMap<string, int>();
            chainMap["a"] = 1;

            Assert.IsTrue(chainMap.Remove("a"));
        }

        // Test 22: Usuwanie nieistniejącego klucza
        [TestMethod]
        public void Remove_KeyNotInMain_ReturnsFalse()
        {
            var chainMap = new ChainMap<string, int>();
            Assert.IsFalse(chainMap.Remove("a"));
        }

        // Test 23: Scalanie słowników z zachowaniem priorytetów
        [TestMethod]
        public void Merge_CombinesAllDictionariesWithPriority()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 } };
            var dict2 = new Dictionary<string, int> { { "a", 2 } };
            var chainMap = new ChainMap<string, int>(dict1, dict2);

            var merged = chainMap.Merge();
            Assert.AreEqual(1, merged["a"]);
        }

        // Test 24: Enumerator zwraca unikalne klucze
        [TestMethod]
        public void Enumerator_ReturnsAllUniqueKeys()
        {
            var dict1 = new Dictionary<string, int> { { "a", 1 } };
            var dict2 = new Dictionary<string, int> { { "a", 2 } };
            var chainMap = new ChainMap<string, int>(dict1, dict2);

            int count = 0;
            foreach (var kvp in chainMap)
            {
                count++;
            }
            Assert.AreEqual(1, count);
        }
    }
}
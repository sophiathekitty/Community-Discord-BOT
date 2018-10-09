using System;
using System.Collections.Generic;
using CommunityBot.Configuration;
using NUnit.Framework;

namespace CommunityBot.NUnit.Tests
{
    public class InMemoryStorageTests
    {
        private static readonly string TestStorageKey = "UnitTestKey";
        private IDataStorage storage;

        [SetUp]
        public void SetUpTest()
        {
            storage = new InMemoryDataStorage();
        }

        [Test]
        public void AddingAndRestoringNewItem_Test()
        {
            var expected = GetTimeBasedStringWithSuffix("value");

            storage.StoreObject(expected, TestStorageKey);
            var actual = storage.RestoreObject<string>(TestStorageKey);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FailingCastingThrows_Test()
        {
            storage.StoreObject(GetTimeBasedStringWithSuffix("value"), TestStorageKey);
            
            Assert.Throws<InvalidCastException>(() => storage.RestoreObject<int>(TestStorageKey));
        }

        [Test]
        public void GettingUnknownKeyThrows_Test()
        {
            Assert.Throws<KeyNotFoundException>(() => storage.RestoreObject<string>(TestStorageKey));
        }

        [Test]
        public void StoreAndRestoreMultipleObjectOfDifferentTypes_Test()
        {
            const string numberKey = "MyNumber";
            const string characterKey = "MyCharacter";
            const int expectedNumber = 88;
            const char expectedCharacter = 'G';
            storage.StoreObject(expectedNumber, numberKey);
            storage.StoreObject(expectedCharacter, characterKey);

            var actualNumber = storage.RestoreObject<int>(numberKey);
            var actualCharacter = storage.RestoreObject<char>(characterKey);

            Assert.AreEqual(expectedNumber, actualNumber);
            Assert.AreEqual(expectedCharacter, actualCharacter);
        }

        [Test]
        public void OverwriteStoredValue_Test()
        {
            const string expectedOld = "MyOldValue";
            const string expectedNew = "MyNewValue";
            storage.StoreObject(expectedOld, TestStorageKey);

            var actualOld = storage.RestoreObject<string>(TestStorageKey);
            Assert.AreEqual(expectedOld, actualOld);

            storage.StoreObject(expectedNew, TestStorageKey);
            var actualNew = storage.RestoreObject<string>(TestStorageKey);

            Assert.AreEqual(expectedNew, actualNew);
        }

        private static string GetTimeBasedStringWithSuffix(string suffix)
        {
            var dateTimeString = DateTime.Now.ToString("HH:mm:ss");
            return $"{dateTimeString}{suffix}";
        }
    }
}

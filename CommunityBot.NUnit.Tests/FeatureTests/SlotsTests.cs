using CommunityBot.Features.Economy;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CommunityBot.NUnit.Tests.FeatureTests
{
    public class SlotsTests
    {
        private Slot defaultSlot;

        [SetUp]
        public void TestSetup()
        {
            defaultSlot = new Slot();
        }

        [Test]
        public void Slots_Constructor_DefaultValue_SlotPiecesEqualsMinSpawnRate()
        {
            int totalMinSpawnRate = Slot.PossibleSlotPieces.Sum(p => p.minSpawnCount);
            Assert.AreEqual(totalMinSpawnRate, defaultSlot.Cylinders.First().SlotPieces.Count());
        }

        [Test]
        public static void Slots_Constructor_FewerPiecesThanMinSpawnRate_SlotPiecesEqualsMinSpawnRate()
        {
            int totalMinSpawnRate = Slot.PossibleSlotPieces.Sum(p => p.minSpawnCount);
            int numberOfPieces = totalMinSpawnRate - 1;
            var slot = new Slot(numberOfPieces);
            Assert.AreEqual(totalMinSpawnRate, slot.Cylinders.First().SlotPieces.Count());
        }

        [Test]
        public static void Slots_Constructor_GreaterPiecesThanMinSpawnRate_SlotPiecesEqualsPieceCount()
        {
            int totalMinSpawnRate = Slot.PossibleSlotPieces.Sum(p => p.minSpawnCount);
            int numberOfPieces = totalMinSpawnRate + 1;
            var slot = new Slot(numberOfPieces);
            Assert.AreEqual(numberOfPieces, slot.Cylinders.First().SlotPieces.Count());
        }

        [Test]
        public void Slots_GetPayoutAndFlavourText_FlavorTextNotEmptyMoneyGainNotNegativeFor1()
        {
            var payoutAndFlavorText = defaultSlot.GetPayoutAndFlavourText(1);
            Assert.IsNotEmpty(payoutAndFlavorText.Item2);
            Assert.IsTrue(payoutAndFlavorText.Item1 >= 0);
        }

        [Test]
        public void Slots_GetCylinderEmojis_ShowAllFalse_LineCountEqualsDefault3()
        {
            List<string> cylinderEmojis = defaultSlot.GetCylinderEmojis();
            Assert.AreEqual(3, cylinderEmojis.Count);
        }

        [Test]
        public void Slots_GetCylinderEmojis_ShowAllFalse_LinesEqualSpinLines()
        {
            List<string> spinResultLines = defaultSlot.Spin().Split("\n").ToList();
            List<string> cylinderEmojis = defaultSlot.GetCylinderEmojis();
            Assert.AreEqual(spinResultLines.Count, cylinderEmojis.Count);
            Assert.AreEqual(spinResultLines[0], cylinderEmojis[0]);
            Assert.AreEqual(spinResultLines[1], cylinderEmojis[1]);
            Assert.AreEqual(spinResultLines[2], cylinderEmojis[2]);
        }

        [Test]
        public void Slots_GetCylinderEmojis_ShowAllTrue_LineCountEqualSlotPieces()
        {
            List<string> expected = defaultSlot.GetCylinderEmojis(true);
            Assert.AreEqual(defaultSlot.Cylinders.First().SlotPieces.Count(), expected.Count);
        }

        [Test]
        public void Slots_Spin_ResultNotEmpty()
        {
            string spinResult = defaultSlot.Spin();
            Assert.IsNotEmpty(spinResult);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using CommunityBot.Helpers;

namespace CommunityBot.NUnit.Tests
{
    class OperationsTests
    {
        [Test]
        public static void OperationsMultBeforAdd()
        {
            double expected = 10 + 5 * 2;

            double actual = Operations.PerformComputation("10+5*2");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void OperationsWrongInput_Returns0()
        {
            double expected = 0;

            double actual = Operations.PerformComputation("1+x");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void OperationsMultipleOperationsAttached()
        {
            double expected = 0;

            double actual = Operations.PerformComputation("1++");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void OperationsInputTooShort_Returns0()
        {
            double expected = 0;

            double actual = Operations.PerformComputation("-3");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void OperationsInputLeadingOperationAddSub()
        {
            double expected = -4+5;

            double actual = Operations.PerformComputation("-4+5");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void OperationsInputLeadingOperationMultDiv()
        {
            double expected = 0 + 5;

            double actual = Operations.PerformComputation("*3+5");

            Assert.AreEqual(expected, actual);
        }
    }
}

﻿using NUnit.Framework;
using Omise;

namespace Omise.Tests {
    [TestFixture]
    public class KeyTest : OmiseTest {
        [Test]
        public void TestStringImplicitAssignable() {
            var s = "pkey_string_test";
            Key key = s;

            Assert.AreEqual(s, (string)key);
        }

        [Test]
        public void TestModes() {
            Key pkey = "pkey_test_123";
            Key skey = "skey_123";

            Assert.IsTrue(pkey.IsTest);
            Assert.IsTrue(skey.IsLive);
        }

        [Test]
        public void TestToString() {
            var s = "pkey_string_test";
            Key k = s;

            Assert.AreSame(k.ToString(), s);
        }
    }
}


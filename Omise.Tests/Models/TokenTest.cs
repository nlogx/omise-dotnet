using System;
using NUnit.Framework;
using Omise.Models;
using Omise.Tests.Util;

namespace Omise.Tests.Models {
    [TestFixture]
    public class TokenTest : ModelTest {
        [Test]
        public async void TestCharge() {
            var token = Create<Token>();
            await token.Charge(1000, "thb");
            AssertRequest("POST", "https://api.omise.co/charges");

            var request = Requester.LastRequest.Payload as CreateChargeRequest;
            Assert.IsNotNull(request);
            Assert.AreEqual(1000, request.Amount);
            Assert.AreEqual("thb", request.Currency);
            Assert.AreEqual(token.Id, request.Card);
        }

        [Test]
        public async void TestChargeRequest() {
            var request = new CreateChargeRequest
            {
                Amount = 1000,
                Currency = "thb",
            };

            var token = Create<Token>();
            await token.Charge(request);
            AssertRequest("POST", "https://api.omise.co/charges");

            var sentRequest = Requester.LastRequest.Payload as CreateChargeRequest;
            Assert.IsNotNull(sentRequest);
            Assert.AreSame(request, sentRequest);
            Assert.AreEqual(token.Id, sentRequest.Card);
        }
    }
}


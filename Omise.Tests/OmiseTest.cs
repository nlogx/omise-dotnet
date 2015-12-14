using NUnit.Framework;
using Omise.Tests.Util;
using Omise.Models;

namespace Omise.Tests {
    [TestFixture]
    public abstract class OmiseTest {
        protected Credentials DummyCredentials { get; private set; }
        protected MockRequester Requester { get; private set; }
        protected Serializer Serializer { get; private set; }

        [SetUp]
        public void SetupBase() {
            DummyCredentials = new Credentials("pkey_test_123", "skey_test_123");
            Requester = new MockRequester();
            Serializer = new Serializer();
        }

        protected void AssertRequest(
            string method,
            string uriFormat,
            params object[] uriArgs
        ) {
            var uri = string.Format(uriFormat, uriArgs);

            var attempt = Requester.LastRequest;
            Assert.AreEqual(method, attempt.Method, method);
            Assert.AreEqual(uri, attempt.Endpoint.ApiPrefix + attempt.Path);
        }

        protected void AssertSerializedRequest<TRequest>(
            TRequest request,
            string serialized
        ) where TRequest: Request {
            var encoded = Serializer.ExtractFormValues(request);
            var result = encoded.ReadAsStringAsync().Result;

            Assert.AreEqual(serialized, result);
        }
    }
}



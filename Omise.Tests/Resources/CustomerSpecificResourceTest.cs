using System;
using NUnit.Framework;
using Omise.Resources;

namespace Omise.Tests.Resources
{
    public class CustomerSpecificResourceTest : ResourceTest<CustomerSpecificResource>
    {
        const string CustomerId = "cust_test_4yq6txdpfadhbaqnwp3";

        [Test]
        public void TestCards()
        {
            Assert.AreEqual(CustomerId, Resource.ParentId);
            Assert.IsNotNull(Resource.Cards);
        }

        protected override CustomerSpecificResource BuildResource(IRequester requester)
        {
            return new CustomerSpecificResource(requester, CustomerId);
        }
    }
}

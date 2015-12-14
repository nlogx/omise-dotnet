using System;
using Omise;
using Omise.Tests.Util;
using System.Threading.Tasks;
using Omise.Resources;
using NUnit.Framework;
using Omise.Models;
using System.Net.Http;

namespace Omise.Tests.Resources {
    public abstract class ResourceTest<TResource> : OmiseTest {
        protected TResource Resource { get; private set; }
        protected TResource Fixtures { get; private set; }

        [SetUp]
        public void SetupResource() {
            var fixtures = new Requester(DummyCredentials, new FixturesRoundtripper());
            Resource = BuildResource(Requester);
            Fixtures = BuildResource(fixtures);
        }

        protected abstract TResource BuildResource(IRequester requester);

    }
}

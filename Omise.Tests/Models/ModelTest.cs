using Omise.Models;
using System;

namespace Omise.Tests.Models {
    public class ModelTest : OmiseTest {
        protected T Create<T>() where T: ModelBase, new() {
            var model = new T();
            model.Id = Guid.NewGuid().ToString();
            model.Requester = Requester;
            return model;
        }
    }
}


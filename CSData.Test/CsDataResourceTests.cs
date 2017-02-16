using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSData;
using Moq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;

namespace CSData.Test
{
    [TestClass]
    public class CsDataResourceTests
    {
        public struct Key
        {
            public Guid Id { get; set; }
        }

        public struct User
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public async Task TestFindCallsAdapterAndReturnsResult()
        {
            var testId = Guid.NewGuid();
            var user = new User
            {
                Id = testId,
                Name = testId.ToString()
            };

            var mockAdapter = new Mock<ICsDataAdapter>();
            mockAdapter.Setup(a => a.Find<Key, User>(new Key { Id = testId })).Returns(() => Task.FromResult(user));
            var dataStore = new CsDataStore(mockAdapter.Object);
            var resource = dataStore.DefineResource("user", (User u) => new Key { Id = u.Id });
            var result = await resource.Find(new Key { Id = testId });
            result.ShouldBeEquivalentTo(user);
        }

        [TestMethod]
        public async Task TestFindCallsAdapterAndReturnsResult()
        {
            var testId = Guid.NewGuid();
            var user = new User
            {
                Id = testId,
                Name = testId.ToString()
            };

            var mockAdapter = new Mock<ICsDataAdapter>();
            mockAdapter.Setup(a => a.Find<Key, User>(new Key { Id = testId })).Returns(() => Task.FromResult(user));
            var dataStore = new CsDataStore(mockAdapter.Object);
            var resource = dataStore.DefineResource("user", (User u) => new Key { Id = u.Id });
            var result = await resource.Find(new Key { Id = testId });
            result.ShouldBeEquivalentTo(user);
        }
    }
}

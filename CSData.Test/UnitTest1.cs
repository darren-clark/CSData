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
    public class UnitTest1
    {
        public struct User
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        [TestMethod]
        public async Task TestMethod1()
        {
            var result = await DoSomething(true);
            result.Should().Be(0);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var type = typeof(User);
            Expression<Func<bool, bool>> test1 = b => b;
            Expression<Func<bool, bool>> test2 = c => c;

            test1.ShouldBeEquivalentTo(test2);

            var whee = test1.ToString();
            test1.ToString().ShouldBeEquivalentTo(test2.ToString());
        }

        [TestMethod]
        public async Task TestMethod3()
        {
            var originaltask = DoSomething(false);
            var wrappedTask = WrapSomething(originaltask);
            var result = await DoSomething(false);
            result.Should().Be(1);
            wrappedTask.Status.Should().Be(TaskStatus.Canceled);
        }

        [TestMethod]
        public async Task TestMethod4()
        {
            var originalTask = DoSomething(true);
            var wrappedTask = WrapSomething(originalTask);
            try
            {
                var result = await originalTask;
            }catch(Exception)
            {
            }
            wrappedTask.Status.Should().Be(TaskStatus.RanToCompletion);
        }

        private Task<int> DoSomething(bool shouldThrow)
        {
            var task = Task.Delay(1000);
            if (shouldThrow)
            {
                return task.ContinueWith<int>(t =>
                {
                    throw new Exception();
                });
            } else
            {
                return task.ContinueWith(t => 1);
            }
        }

        private Task<int> WrapSomething(Task<int> task)
        {
            return task.ContinueWith(t => 2, TaskContinuationOptions.NotOnRanToCompletion);
        }
    }
}

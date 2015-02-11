﻿using System.Collections.Generic;
using FluentAssertions;
using TechTalk.SpecFlow;
using Thycotic.MemoryMq.Collections;

namespace Thycotic.MemoryMq.Tests.Collections
{
    [Binding]
    public class ConcurrentPriorityQueueSteps
    {
        [Given(@"there exists an integer ConcurrentPriorityQueue stored in the scenario as (\w+)")]
        public void GivenThereExistsAnIntegerConcurrentPriorityQueueStoredInTheScenarioAs(string queueName)
        {
            ScenarioContext.Current[queueName] = new ConcurrentPriorityQueue<int>();
        }

        [Given(@"the scenario integer ConcurrentPriorityQueue (\w+) is empty")]
        public void GivenTheScenarioObjectConcurrentPriorityQueueTestShouldBeEmpty(string queueName)
        {
            var queue = (ConcurrentPriorityQueue<int>)ScenarioContext.Current[queueName];

            queue.IsEmpty.Should().BeTrue();
        }

        [Given(@"the scenario integer ConcurrentPriorityQueue (\w+) is not empty")]
        public void GivenTheScenarioObjectConcurrentPriorityQueueTestShouldNotBeEmpty(string queueName)
        {
            var queue = (ConcurrentPriorityQueue<int>)ScenarioContext.Current[queueName];

            queue.IsEmpty.Should().BeFalse();
        }

        [Given(@"item (\d+) is enqueued in the scenario integer ConcurrentPriorityQueue (\w+)")]
        public void GivenItemIsEnqueuedInTheScenarioObjectConcurrentPriorityQueueTest(int item, string queueName)
        {
            var queue = (ConcurrentPriorityQueue<int>) ScenarioContext.Current[queueName];
            queue.Enqueue(item);
        }

        [Given(@"item is dequeued in the scenario integer ConcurrentPriorityQueue (\w+)")]
        public void GivenItemIsDequeuedInTheScenarioObjectConcurrentPriorityQueueTest(string queueName)
        {
            var queue = (ConcurrentPriorityQueue<int>)ScenarioContext.Current[queueName];
            int item;
            queue.TryDequeue(out item);
        }

        [Given(@"item (\d+) is priorty enqueued in the scenario integer ConcurrentPriorityQueue (\w+)")]
        public void GivenItemIsPriortyEnqueuedInTheScenarioObjectConcurrentPriorityQueueTest(int item, string queueName)
        {
            var queue = (ConcurrentPriorityQueue<int>)ScenarioContext.Current[queueName];
            queue.PriorityEnqueue(item);
        }

        [When(@"all items are dequeued from scenario integer ConcurrentPriorityQueue (\w+) and stored in scenario integer array (\w+)")]
        public void WhenAllItemsAreDequeuedFromScenarioObjectConcurrentPriorityQueueTestAndStoredInScenarioObjectConcurrentPriorityQueueResults(string queueName, string resultsName)
        {
            var queue = (ConcurrentPriorityQueue<int>)ScenarioContext.Current[queueName];

            var results = new List<int>();

            int item;
            while (queue.TryDequeue(out item))
            {
                results.Add(item);
            }

            ScenarioContext.Current[resultsName] = results.ToArray();
        }

        [Then(@"the scenario integer ConcurrentPriorityQueue (\w+) should be empty")]
        public void ThenTheScenarioObjectConcurrentPriorityQueueTestShouldBeEmpty(string queueName)
        {
            var queue = (ConcurrentPriorityQueue<int>)ScenarioContext.Current[queueName];

            queue.IsEmpty.Should().BeTrue();
        }

        [Then(@"the scenario integer ConcurrentPriorityQueue (\w+) should not be empty")]
        public void ThenTheScenarioObjectConcurrentPriorityQueueTestShouldNotBeEmpty(string queueName)
        {
            var queue = (ConcurrentPriorityQueue<int>)ScenarioContext.Current[queueName];

            queue.IsEmpty.Should().BeFalse();
        }


        [Then(@"the string join of scenario integer array (\w+) should be ""(.*)""")]
        public void ThenTheStringJoinOfScenarioObjectConcurrentPriorityQueueResultsShouldBe(string resultsName, string resultsString)
        {
            var resultArray = (int[]) ScenarioContext.Current[resultsName];

            string.Join(" ", resultArray).Should().Be(resultsString);
        }
    }
}
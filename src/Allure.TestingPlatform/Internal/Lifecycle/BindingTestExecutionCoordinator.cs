using System;
using System.Collections.Generic;
using Allure.TestingPlatform.Internal.Functions;
using Allure.TestingPlatform.Sdk.ExecutionState;

namespace Allure.TestingPlatform.Internal.Lifecycle;

sealed class BindingTestExecutionCoordinator : ITestExecutionCoordinator
{
    class ExecutionBase
    {
        public Queue<Action<TestExecutionStateUid>> Operations { get; } = [];

        public bool IsFinished { get; set; }
    }

    sealed class PendingExecution : ExecutionBase;

    sealed class Execution(TestExecutionStateUid testNodeUid) : ExecutionBase
    {
        public TestExecutionStateUid TestNodeUid { get; } = testNodeUid;

        public Occurrence? Occurrence { get; set; }

        public bool IsActive { get; set; }

        public bool IsCompleted { get; set; }
    }

    sealed class Occurrence(Action? start)
    {
        public Action? Start { get; } = start;

        public Action? Finish { get; set; }

        public Execution? Execution { get; set; }

        public bool IsStartInvoked { get; set; }

        public bool IsNodeFinished { get; set; }
    }

    sealed class Node
    {
        public Queue<Occurrence> Occurrences { get; } = [];

        public Queue<Occurrence> UnpairedOccurrences { get; } = [];

        public Queue<Execution> UnpairedExecutions { get; } = [];

        public Occurrence? OpenOccurrence { get; set; }
    }

    readonly Dictionary<TestExecutionStateUid, Node> nodes = [];

    readonly Dictionary<TestExecutionStateUid, Execution> executions = [];

    readonly Dictionary<TestExecutionStateUid, PendingExecution> pendingExecutions = [];

    public void StartTestNode(TestExecutionStateUid testNodeUid, Action start)
    {
        var node = this.GetOrCreateNode(testNodeUid);

        if (node.OpenOccurrence is not null)
        {
            throw new InvalidOperationException(
                $"Test node {testNodeUid} is already running."
            );
        }

        Occurrence occurrence = new(start);

        node.OpenOccurrence = occurrence;
        node.Occurrences.Enqueue(occurrence);
        node.UnpairedOccurrences.Enqueue(occurrence);

        this.PairAndProgress(node);
    }

    public void FinishTestNode(TestExecutionStateUid testNodeUid, Action finish)
    {
        var node = this.GetOrCreateNode(testNodeUid);
        // Occurrence occurrence;

        if (node.OpenOccurrence is { } occurrence)
        {
            node.OpenOccurrence = null;
        }
        else
        {
            // Finish without start. The finish callback must create the state
            // before finalization.
            occurrence = new(start: null);

            node.Occurrences.Enqueue(occurrence);
            node.UnpairedOccurrences.Enqueue(occurrence);
        }

        occurrence.IsNodeFinished = true;
        occurrence.Finish = finish;

        this.PairAndProgress(node);
    }

    public void BindTestExecution(TestExecutionStateUid testNodeUid, TestExecutionStateUid executionUid)
    {
        if (this.executions.TryGetValue(executionUid, out var existingExecution))
        {
            if (existingExecution.TestNodeUid != testNodeUid)
            {
                throw new InvalidOperationException(
                    $"Test execution {executionUid} is already bound to "
                        + $"test node {existingExecution.TestNodeUid}."
                );
            }

            return;
        }

        Execution execution = new(testNodeUid);

        if (Dictionaries.TryRemoveAndGet(this.pendingExecutions, executionUid, out var pendingExecution))
        {
            var executionOperations = execution.Operations;
            var pendingOperations = pendingExecution.Operations;

            while (pendingOperations.Count > 0)
            {
                executionOperations.Enqueue(
                    pendingOperations.Dequeue()
                );
            }

            execution.IsFinished = pendingExecution.IsFinished;
        }

        this.executions.Add(executionUid, execution);

        var node = this.GetOrCreateNode(testNodeUid);
        node.UnpairedExecutions.Enqueue(execution);

        this.PairAndProgress(node);
    }

    public void FinishTestExecution(TestExecutionStateUid executionUid)
    {
        if (!this.executions.TryGetValue(executionUid, out var execution))
        {
            // No test node started yet but the execution has already finished.
            var pendingExecution = this.GetOrCreatePendingExecution(executionUid);

            this.ThrowIfFinished(pendingExecution, executionUid);

            pendingExecution.IsFinished = true;
            return;
        }

        this.ThrowIfFinished(execution, executionUid);
        this.ThrowIfCompleted(execution, executionUid);

        execution.IsFinished = true;

        if (execution.Occurrence is not null)
        {
            // Conclude the execution of a finished test node occurrence.
            var node = this.nodes[execution.TestNodeUid];
            this.Progress(node);
        }
    }

    public void Route(TestExecutionStateUid executionUid, Action<TestExecutionStateUid> operation)
    {
        if (!this.executions.TryGetValue(executionUid, out var execution))
        {
            var pendingExecution = this.GetOrCreatePendingExecution(executionUid);

            this.ThrowIfFinished(pendingExecution, executionUid);

            // No corresponding test node occurred yet and no execution started.
            pendingExecution.Operations.Enqueue(operation);
            return;
        }

        this.ThrowIfFinished(execution, executionUid);
        this.ThrowIfCompleted(execution, executionUid);

        if (execution.IsActive)
        {
            // The test node occurrence started and not finished yet,
            // which means the state exists and can be updated.
            operation(execution.TestNodeUid);
        }
        else
        {
            // Inactive, unfinished, and uncompleted:
            // the execution has just been declared by the execution start
            // message but has not yet been associated with the test node
            // occurrence.
            execution.Operations.Enqueue(operation);
        }
    }

    Node GetOrCreateNode(TestExecutionStateUid testNodeUid)
    {
        if (!this.nodes.TryGetValue(testNodeUid, out var node))
        {
            this.nodes[testNodeUid] = node = new();
        }

        return node;
    }

    PendingExecution GetOrCreatePendingExecution(TestExecutionStateUid executionUid)
    {
        if (!this.pendingExecutions.TryGetValue(executionUid, out var execution))
        {
            this.pendingExecutions[executionUid] = execution = new();
        }

        return execution;
    }

    void PairAndProgress(Node node)
    {
        var unpairedOccurrences = node.UnpairedOccurrences;
        var unpairedExecutions = node.UnpairedExecutions;

        while (unpairedOccurrences.Count > 0 && unpairedExecutions.Count > 0)
        {
            var occurrence = unpairedOccurrences.Dequeue();
            var execution = unpairedExecutions.Dequeue();

            occurrence.Execution = execution;
            execution.Occurrence = occurrence;
        }

        this.Progress(node);
    }

    void Progress(Node node)
    {
        var occurrences = node.Occurrences;

        while (occurrences.Count > 0)
        {
            var occurrence = occurrences.Peek();

            if (!occurrence.IsStartInvoked)
            {
                // A later occurrence of the test node must wait until
                // the current one releases its state.
                occurrence.IsStartInvoked = true;
                occurrence.Start?.Invoke();
            }

            var execution = occurrence.Execution;
            if (execution is null)
            {
                // Test node started but no binding established yet.
                return;
            }

            if (!execution.IsActive)
            {
                // Binding arrived before test node started.
                // Execution operations bound to the execution UID also
                // may have arrived early.
                execution.IsActive = true;

                var operations = execution.Operations;
                while (operations.Count > 0)
                {
                    var operation = execution.Operations.Dequeue();
                    operation(execution.TestNodeUid);
                }
            }

            if (!occurrence.IsNodeFinished || !execution.IsFinished)
            {
                // The test node occurrence and the execution are in sync.
                // At least one has not signalled its completion yet.
                return;
            }

            var finish = occurrence.Finish
                ?? throw new InvalidOperationException(
                    $"Test node {execution.TestNodeUid} is marked as finished "
                        + "but has no finish operation."
                );

            try
            {
                finish();
            }
            finally
            {
                execution.IsActive = false;
                execution.IsCompleted = true;
                node.Occurrences.Dequeue();
            }
        }
    }

    void ThrowIfFinished(ExecutionBase execution, TestExecutionStateUid executionUid)
    {
        if (execution.IsFinished)
        {
            throw new InvalidOperationException(
                $"Test execution {executionUid} has already finished."
            );
        }
    }

    void ThrowIfCompleted(Execution execution, TestExecutionStateUid executionUid)
    {
        if (execution.IsCompleted)
        {
            throw new InvalidOperationException(
                $"Test execution {executionUid} has already completed."
            );
        }
    }
}

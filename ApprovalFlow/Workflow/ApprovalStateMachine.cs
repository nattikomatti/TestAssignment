using ApprovalFlow.Data;

namespace ApprovalFlow.Workflow
{
    public class ApprovalStateMachine
    {
        private static readonly Dictionary<ApprovalState, ApprovalState[]> AllowedTransitions = new()
        {
            { ApprovalState.Pending, [ApprovalState.Approved, ApprovalState.Rejected] },
            { ApprovalState.Approved, [ApprovalState.Undone] },
            { ApprovalState.SentToExternalSystem, [ApprovalState.Undone] },
            { ApprovalState.Rejected, [] },
            { ApprovalState.Undone, [ApprovalState.Approved, ApprovalState.Rejected] },
        };

        public bool CanTransition(ApprovalState current, ApprovalState target)
        {
            return AllowedTransitions.TryGetValue(current, out var allowed) && allowed.Contains(target);
        }

        public bool CanUndo(ApprovalStep step, List<ApprovalStep> allSteps)
        {
            if (step.State is not (ApprovalState.Approved or ApprovalState.SentToExternalSystem))
                return false;

            var hasLaterApprovedStep = allSteps.Any(s =>
                s.StepOrder > step.StepOrder &&
                s.State is ApprovalState.Approved or ApprovalState.SentToExternalSystem);

            return !hasLaterApprovedStep;
        }

        public ApprovalStep? GetCurrentPendingStep(List<ApprovalStep> steps)
        {
            return steps
                .Where(s => s.State is ApprovalState.Pending or ApprovalState.Undone)
                .OrderBy(s => s.StepOrder)
                .FirstOrDefault();
        }

        public ApprovalStep? GetLastApprovedStep(List<ApprovalStep> steps)
        {
            return steps
                .Where(s => s.State is ApprovalState.Approved or ApprovalState.SentToExternalSystem)
                .OrderByDescending(s => s.StepOrder)
                .FirstOrDefault();
        }
    }
}

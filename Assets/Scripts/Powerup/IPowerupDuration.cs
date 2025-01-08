namespace RolliCanoli {
    public interface IPowerupDuration {
        IPowerup Parent { get; }
        PowerupType PowerupType { get; }
        bool IsComplete { get; }
        float PercentIncomplete { get; }

        void ForceComplete();
    }

    public interface IPowerupDuration<TPowerup> : IPowerupDuration where TPowerup : IPowerup {
        new TPowerup Parent { get; }
    }
}

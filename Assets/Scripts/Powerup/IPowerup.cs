namespace RolliCanoli {
    public interface IPowerup {
        PowerupType PowerupType { get; }

        bool Respawn();
        IPowerupDuration CreateDuration();
        bool CreateDuration(out IPowerupDuration duration);
        bool PlaySound();
    }

    public interface IPowerup<TPowerupDuration> : IPowerup where TPowerupDuration : IPowerupDuration {
        new TPowerupDuration CreateDuration();
        bool CreateDuration(out TPowerupDuration duration);
        bool CommitToDuration(TPowerupDuration duration);
        bool GetCommitedDuration(out TPowerupDuration duration);
    }
}

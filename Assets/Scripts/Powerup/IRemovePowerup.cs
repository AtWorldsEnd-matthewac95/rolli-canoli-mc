namespace RolliCanoli {
    public interface IRemovePowerup {
        bool RemovesAllPowerups { get; }

        bool ValidateRemoval(OblongPlayerController player);
    }
}

public interface IInteractable
{
    InteractionType InteractionType { get; }

    float InteractionDuration { get; }

    void Interact();
}
using Navigation.DeterministicMath;

namespace Assets.Navigation.AStar
{
    public interface IPriorityQueueEntry
    {
        DeterministicFloat GetCost();
    }
}

namespace MurderMystery.Interfaces
{
    public interface IStorylineStrategy
    {
        string Name { get; }
        bool Execute(TimelineContext context);
    }
}

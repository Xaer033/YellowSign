
public interface ICommand
{
    CommandType commandType { get; }
    bool allowMultiplePerTick { get; }

    //byte getOwnerId();
}
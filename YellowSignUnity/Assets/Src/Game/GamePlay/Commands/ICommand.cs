
public interface ICommand
{
    // Used for De-serialization
    CommandType commandType { get; }

    //byte getOwnerId();
}
namespace CSharpLocalAndRemote.Error;

public abstract class TenistaError
{
    private string Message { get; init; }

    public override string ToString()
    {
        return Message;
    }

    public class StorageError : TenistaError
    {
        public StorageError(string message)
        {
            Message = "ERROR: " + message;
        }
    }
}
namespace CSharpLocalAndRemote.Error;

public abstract class TenistaError
{
    private string? Message { get; init; }

    public override string ToString()
    {
        return Message ?? "ERROR";
    }

    public class StorageError : TenistaError
    {
        public StorageError(string message)
        {
            Message = "ERROR: " + message;
        }
    }

    public class DatabaseError : TenistaError
    {
        public DatabaseError(string message)
        {
            Message = "ERROR: " + message;
        }
    }

    public class RemoteError : TenistaError
    {
        public RemoteError(string message)
        {
            Message = "ERROR: " + message;
        }
    }
}
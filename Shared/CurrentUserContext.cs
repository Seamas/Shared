using System.Threading;

namespace Wang.Seamas.Shared;

public static class CurrentUserContext
{

    private static readonly AsyncLocal<int?> _userId = new();

    public static int? UserId
    {
        get => _userId.Value;
        set => _userId.Value = value;
    }
    
    public static void Clear() => _userId.Value = null;
}
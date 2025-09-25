namespace LibraryApi.Tests.TestUtilities;

public static class TestConstants
{
    public const string ValidBookName = "Test Book";
    public const string ValidOwnerName = "Test Owner";
    public const bool ValidAvailability = true;
    
    public const string EmptyString = "";
    public const string WhitespaceString = "   ";
    public const string NullString = null!;
    
    public const int ValidPage = 1;
    public const int ValidPageSize = 10;
    public const int InvalidPage = 0;
    public const int InvalidPageSize = 0;
    public const int MaxPageSize = 100;
    public const int ExceedsMaxPageSize = 200;
}

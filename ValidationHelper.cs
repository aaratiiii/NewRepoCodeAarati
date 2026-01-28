using Aarati_s_Journal.Exceptions;

namespace Aarati_s_Journal.Helpers;

public static class ValidationHelper
{
    public static void Require(bool condition, string message)
    {
        if (!condition) throw new ValidationException(message);
    }
}

namespace Benefits.Common
{
    public static class Guard
    {
        public static T NotNull<T>(T argument, [System.Runtime.CompilerServices.CallerArgumentExpression(nameof(argument))]
        string? paramName = null)
        {
            ArgumentNullException.ThrowIfNull(argument, paramName);

            return argument;
        }
    }
}

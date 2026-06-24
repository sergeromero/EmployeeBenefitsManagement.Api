namespace Benefits.Common
{
    public static class Argument
    {
        public static T NotNull<T>(T argument, string argumentName)
        {
            if(argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            return argument;
        }
    }
}

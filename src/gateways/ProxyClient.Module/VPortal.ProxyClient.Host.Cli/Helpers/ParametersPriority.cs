using VPortal.ProxyClient.Host.Cli.Parameters;

namespace VPortal.ProxyClient.Host.Cli.Helpers
{
    public class ParametersPriority
    {
        // The list of CLI parameter types, from the most prioritized to the least.
        private static Type[] Priorities = new Type[]
        {
            typeof(ChangePortParameter),
            typeof(ClearProxyRegistrationParameter),
            typeof(RegisterProxyParameter),
            typeof(StartProxyParameter),
        };

        public static int GetPriority<TParam>()
            => GetPriority(typeof(TParam));

        public static int GetPriority(Type paramType)
        {
            var ix = Array.IndexOf(Priorities, paramType);
            if (ix == -1)
            {
                return 0;
            }

            return Priorities.Length - ix;
        }
    }
}

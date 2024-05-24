using System.Reflection;

namespace KSRKlient2
{
    class Program
    {
        public static void Main(string[] args)
        {
            string progId = "KSR20.COM3Klasa.1";
            string methodName = "Test";

            Type type = Type.GetTypeFromProgID(progId);
            if (type != null)
            {
                try
                {

                    object act = Activator.CreateInstance(type);
                    MethodInfo methodInfo = type.GetMethod(methodName);
                    if (methodInfo != null)
                    {
                        ParameterInfo[] parameters = methodInfo.GetParameters();

                        bool acceptsStringParam = false;
                        foreach (ParameterInfo param in parameters)
                        {
                            Console.WriteLine(param.ParameterType);
                        }
                    }
                    type.InvokeMember(methodName, System.Reflection.BindingFlags.InvokeMethod, null, act, ["klasa dziala w c#"]);
                }
                catch
                {
                    Console.WriteLine("cos nie dziala");
                }
            }
            else
            {
                Console.WriteLine("nie pobrano typu");
            }
        }
    }
}
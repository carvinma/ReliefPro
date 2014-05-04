using Autofac;
using Autofac.Configuration;

namespace ReliefProMain
{
    public class AutoFacManger
    {
        public static IContainer container;

        static AutoFacManger()
        {
            if (container == null)
            {
                ContainerBuilder builder = new ContainerBuilder();
                builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
                container = builder.Build();
                
            }
        }
        private void Regiser()
        {
            //Type objTypeA = Type.GetType("Pro9._2.Read92,Pro9.2");
            //Type objTypeB = Type.GetType("Pro9._1.Read91,Pro9.1");

            //builder.RegisterType(objTypeB).Named<IPro>("Pro91");
            //container = builder.Build();
        }
    }
}

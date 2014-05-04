
using System;
using System.Reflection;
using Autofac;
using ProII;
namespace ReliefProBLL
{

    public class ReadFromPro
    {
        IContainer container;
        public ReadFromPro()
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }
        private bool NeedAutoRegisterToContainer(Assembly assembly)
        {
            const string needBuildAssemblyStartName = "Pro";
            return assembly.FullName.StartsWith(needBuildAssemblyStartName);
        }

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Assembly assembly = args.LoadedAssembly;
            if (assembly.GlobalAssemblyCache)
                return;
            if (!NeedAutoRegisterToContainer(assembly))
                return;
            UpdateContainerFromAssemblies(assembly);
        }
        /// <summary>
        /// 将待注册程序集列表中的程序集注册到容器中
        /// </summary>
        /// <param name="assemblies">待注册程序集列表</param>
        private void UpdateContainerFromAssemblies(params Assembly[] assemblies)
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof(IProIIReader).IsAssignableFrom(t))
                .AsImplementedInterfaces();
            if (container == null)
            { container = builder.Build(); }
            else
                builder.Update(container);

        }

        public string ReadData()
        {
            var pro9 = container.Resolve<IProIIReader>();
            return pro9.Read();
        }
    }
}

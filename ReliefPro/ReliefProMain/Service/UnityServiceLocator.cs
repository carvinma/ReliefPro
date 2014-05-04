using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReliefProMain.Interface;
using Microsoft.Practices.Unity;

namespace ReliefProMain.Service
{
    class UnityServiceLocator : IServiceLocator
    {
        private UnityContainer container;

        public UnityServiceLocator()
        {
            container = new UnityContainer();
        }
    
        void IServiceLocator.Register<TInterface, TImplementation>()
        {
            container.RegisterType<TInterface, TImplementation>();
        }

        TInterface IServiceLocator.Get<TInterface>()
        {
            return container.Resolve<TInterface>();
        }
    }
}

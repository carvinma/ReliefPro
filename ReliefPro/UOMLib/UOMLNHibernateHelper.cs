using System;
using System.Collections;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;

namespace UOMLib
{
    public class UOMLNHibernateHelper : IDisposable
    {
        public readonly ISessionFactory SessionFactory;

        public UOMLNHibernateHelper(string dbPath)
        {
            Configuration config = new Configuration();
            IDictionary props = new Hashtable();
            props["current_session_context_class"] = "thread_static";
            props["connection.provider"] = "NHibernate.Connection.DriverConnectionProvider";
            props["dialect"] = "NHibernate.JetDriver.JetDialect, NHibernate.JetDriver";
            props["connection.driver_class"] = "NHibernate.JetDriver.JetDriver, NHibernate.JetDriver";
            props["connection.connection_string"] = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", dbPath);
            foreach (DictionaryEntry de in props)
            {
                config.SetProperty(de.Key.ToString(), de.Value.ToString());
            }
            SessionFactory = config.AddAssembly("ReliefProModel").BuildSessionFactory();
        }

        private void BindSession()
        {
            if (!CurrentSessionContext.HasBind(SessionFactory))
            {
                CurrentSessionContext.Unbind(SessionFactory);
            }
            CurrentSessionContext.Bind(SessionFactory.OpenSession());
        }
        public ISession GetCurrentSession()
        {
            try
            {
                BindSession();
                return SessionFactory.GetCurrentSession();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        public void CloseSession(ISession currentSession)
        {
            if (currentSession != null && currentSession.IsOpen) currentSession.Close();
        }

        ~UOMLNHibernateHelper()
        { }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    SessionFactory.Close();
                    SessionFactory.Dispose();
                }
                m_disposed = true;
            }
        }
        private bool m_disposed;
    }
}

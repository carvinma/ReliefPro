﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ReliefProModel.GlobalDefault;

namespace UOMLib
{
    public class TempleSession
    {
        private static readonly string dbConnectPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"Template\plant.mdb";

        public static ISession Session { get; private set; }
        public static int UnitFormFlag = 0;
        static TempleSession()
        {
            using (var helper = new UOMLNHibernateHelper(dbConnectPath))
            {
                Session = helper.GetCurrentSession();
            }
        }
        private static TempleSession _instance;

        public static TempleSession Instance()
        {
            if (_instance == null)
                _instance = new TempleSession();
            return _instance;
        }
        public static List<FlareSystem> lstFlareSys
        {
            get;
            set;
        }

    }
}

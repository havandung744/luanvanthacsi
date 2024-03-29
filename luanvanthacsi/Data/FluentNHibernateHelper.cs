﻿using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using luanvanthacsi.Data.Entities;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using ISession = NHibernate.ISession;

namespace luanvanthacsi.Data
{
    public static class FluentNHibernateHelper
    {
        public static ISessionFactory _sessionFactory;
        public static ISessionFactory SessionFactory
        {
            get { return _sessionFactory == null ? _sessionFactory = OpenConect() : _sessionFactory; }
        }



        public static ISessionFactory OpenConect()
        {
            try
            {
                //string connectionString = "Data Source=LAPTOP-EHJCF543\\SQLEXPRESS;Initial Catalog=luanvanthacsi;User ID=sa;Password=123456;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                string connectionString = "Data Source=172.16.10.59,1499;Initial Catalog=thesis;User ID=hrm;Password=hrm123456aA@;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


                var sessionFactory = Fluently.Configure().Database(MsSqlConfiguration.MsSql2012.
                     ConnectionString(connectionString))
                     .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Scientist>()).
                     ExposeConfiguration(cfg => new SchemaExport(cfg)
                     .Create(false, false))
                     .BuildSessionFactory();
                return sessionFactory;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static ISession OpenSession()
        {
            try
            {
                return SessionFactory.OpenSession();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

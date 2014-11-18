using System;
using System.Data;
using System.IO;
using Machine.Specifications;
using VirtualObjects.Config;

namespace VirtualObjects.Tests
{
    public abstract class SpecsUtilityBelt
    {
        Establish context =
            () =>
            {
                AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"));

                Session = new Session(new SessionConfiguration
                {
                    Logger = Console.Out,
                    SaveGeneratedCode = true
                }, "northwind");

                Mapper = ((InternalSession)Session.InternalSession).Mapper;

                Connection = ((InternalSession)Session.InternalSession).Connection;
                
            };
        
        protected static IMapper Mapper;
        protected static IDbConnection Connection;
        protected static Session Session;
    }
}
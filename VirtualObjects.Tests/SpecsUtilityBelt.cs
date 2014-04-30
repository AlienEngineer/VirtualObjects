using System;
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
                    Logger = Console.Out
                }, "northwind");

                Mapper = ((InternalSession)Session.InternalSession).Mapper;
                
            };
        
        protected static IMapper Mapper;
        protected static Session Session;
    }
}
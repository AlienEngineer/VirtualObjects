using System;
using Machine.Specifications;
using VirtualObjects.Config;

namespace VirtualObjects.Tests
{
    public abstract class SpecsUtilityBelt
    {
        Establish context =
            () =>
            {
                Session = new Session(new SessionConfiguration(), "northwind");

                Mapper = ((InternalSession)Session.InternalSession).Mapper;
                
                Console.WriteLine("This is the first context.");
            };
        
        protected static IMapper Mapper;
        protected static Session Session;
    }
}
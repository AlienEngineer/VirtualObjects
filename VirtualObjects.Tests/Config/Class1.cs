using System;
using System.Data;
using System.IO;
using System.Reflection;
using Fasterflect;
using FluentAssertions;
using Machine.Specifications;
using Ninject;
using VirtualObjects.Config;

namespace VirtualObjects.Tests.Config
{
    [Subject(typeof(AppDomain)), Tags("AppDomain")]
    public class When_creating_a_new_AppDomain
    {
        Establish context = () => { };

        Because of = 
            () =>
            {
                domain = AppDomain.CreateDomain("TestDomain", AppDomain.CurrentDomain.Evidence);
                
                mspec = domain.Load(File.ReadAllBytes("Ninject.dll"));
                
                type = mspec.GetType(typeof(IKernel).FullName);
            };

        Cleanup after = () => AppDomain.Unload(domain);

        It should_not_be_null = 
            () => domain.Should().NotBeNull();

        It should_have_a_type_representation = 
            () => type.Should().NotBeNull();
                                               
        static Type type;
        static Assembly mspec;
        static AppDomain domain;
    }
}

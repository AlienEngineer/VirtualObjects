using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace VirtualObjects.Domains
{
    public class DomainHandler : MarshalByRefObject
    {
        private readonly string _path;
        AppDomain domain;
        public Boolean IsLoaded { get; private set; }
        
        public ICollection<Assembly> Assemblies { get { return _assemblies; } }

        readonly ICollection<Assembly> _assemblies = new Collection<Assembly>();

        public DomainHandler(String path)
        {
            _path = path;
        }

        public void LoadAssemblies()
        {
            domain = AppDomain.CreateDomain("ManagedDomains",
                AppDomain.CurrentDomain.Evidence,
                AppDomain.CurrentDomain.BaseDirectory,
                AppDomain.CurrentDomain.RelativeSearchPath, true);

            domain.Load(typeof(DomainHandler).Assembly.FullName);

            domain.DomainUnload += (sender, args) =>
                IsLoaded = false;

            var path = !String.IsNullOrEmpty(_path) && _path != "\\" ?
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _path) :
                AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(path))
            {
                return;
            }

            foreach (var file in Directory.GetFiles(path, "*.dll"))
            {
                var assembly = domain.Load(File.ReadAllBytes(file));
                _assemblies.Add(assembly);
            }

            IsLoaded = true;
        }

        public void Unload()
        {
            _assemblies.Clear();
            AppDomain.Unload(domain);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.WaitForFullGCComplete();
        }
        
    }
}

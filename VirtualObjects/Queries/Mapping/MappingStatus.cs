using System;

namespace VirtualObjects.Queries.Mapping
{
    static class MappingStatus
    {

        private static bool _internalLoading;
        private static bool _noLazyLoad;
        
        public static bool InternalLoading
        {
            get { return _internalLoading; }
        }

        public static bool NoLazyLoad
        {
            get
            {
                return _noLazyLoad;
            }
        }

        public static void WithNoLazyLoad(Action loadAction)
        {
            _noLazyLoad = true;
            try
            {
                loadAction();
            }
            finally
            {
                _noLazyLoad = false;
            }
        }

        public static void InternalLoad(Action loadAction)
        {
            _internalLoading = true;
            try
            {
                loadAction();
            }
            finally
            {
                _internalLoading = false;
            }
        }

    }
}

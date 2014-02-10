using System;

namespace VirtualObjects.Queries.Mapping
{
    static class MappingStatus
    {

        private static bool _internalLoading;

        public static bool InternalLoading
        {
            get { return _internalLoading; }
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

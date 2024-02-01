using System;

namespace Amg.Authentication.Infrastructure.Base
{
    public class LazyDependency<T>
    {
        private readonly IServiceProvider _provider;
        private T _instance;

        public LazyDependency(IServiceProvider provider)
        {
            _provider = provider;
        }

        public T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = (T)_provider.GetService(typeof(T));

                return _instance;
            }
        }
    }
}

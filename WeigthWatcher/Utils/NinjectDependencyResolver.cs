using Ninject;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;

namespace WeigthWatcher.Utils
{
    /// <summary>
    /// The dependency scope.
    /// </summary>
    public class NinjectDependencyScope : IDependencyScope
    {
        /// <summary>
        /// The dependency resolver
        /// </summary>
        private readonly IResolutionRoot resolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectDependencyScope" /> class.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        internal NinjectDependencyScope(IResolutionRoot resolver)
        {
            this.resolver = resolver;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            // Do nothing.
            // We don't want Ninject to dispose the kernel, as .NET handles this for us
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The actual service implentation.</returns>
        /// <exception cref="System.ObjectDisposedException">Thrown if the scope has already been disposed.</exception>
        public object GetService(Type serviceType)
        {
            if (resolver == null)
                throw new ObjectDisposedException("this", "This scope has already been disposed");

            try
            {
                return this.resolver.Get(serviceType);
            }
            catch (ActivationException ex)
            {
                if (!TypeIsSystemAssembly(serviceType))
                {
                    Trace.WriteLine(ex.Message, "Ninject");
                    Trace.Flush();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The actual service implentations.</returns>
        /// <exception cref="System.ObjectDisposedException">Thrown if the scope has already been disposed.</exception>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (resolver == null)
                throw new ObjectDisposedException("this", "This scope has already been disposed");

            try
            {
                return resolver.GetAll(serviceType);
            }
            catch (ActivationException ex)
            {
                if (!TypeIsSystemAssembly(serviceType))
                {
                    Trace.WriteLine(ex.Message, "Ninject");
                    Trace.Flush();
                }

                return new object[0];
            }
        }

        /// <summary>
        /// Returns a value indicating whether the specified service is a system assembly.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns><see langword="True"/> if the type is a system assembly; otherwise <see langword="false"/>.</returns>
        private bool TypeIsSystemAssembly(Type service)
        {
            return service.Namespace != null && service.Namespace.StartsWith("System", StringComparison.InvariantCultureIgnoreCase);
        }
    }

    /// <summary>
    /// Dependency resolver implementation for ninject.
    /// </summary>
    public class NinjectDependencyResolver : NinjectDependencyScope, IDependencyResolver
    {
        private IKernel kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectDependencyResolver" /> class.
        /// </summary>
        /// <param name="kernel">The Ninject kernel.</param>
        public NinjectDependencyResolver(IKernel kernel) : base(kernel)
        {
            this.kernel = kernel;
        }

        /// <summary>
        /// Starts the dependency scope.
        /// </summary>
        /// <returns>The current dependency scope.</returns>
        public IDependencyScope BeginScope()
        {
            return this;
        }
    }
}
using System;
using System.Collections.Generic;

namespace Bespoke.Common
{
	/// <summary>
	/// Container class implements the IServiceProvider interface. This is used
	/// to pass shared services between different components, for instance the
	/// ContentManager uses it to locate the IGraphicsDeviceService implementation.
	/// </summary>
	public class ServiceContainer : IServiceProvider
	{
		/// <summary>
		/// 
		/// </summary>
		public ServiceContainer()
		{
			mServices = new Dictionary<Type, object>();
		}

		/// <summary>
		/// Adds a new service to the collection.
		/// </summary>
		public void AddService<T>(T service)
		{
			mServices.Add(typeof(T), service);
		}

		/// <summary>
		/// Looks up the specified service.
		/// </summary>
		public object GetService(Type serviceType)
		{
			object service;
			mServices.TryGetValue(serviceType, out service);

			return service;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public T GetService<T>()
		{
			object service;
			mServices.TryGetValue(typeof(T), out service);

			return (T)service;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		public void RemoveService(Type serviceType)
		{
			mServices.Remove(serviceType);
		}

		private Dictionary<Type, object> mServices;
	}
}

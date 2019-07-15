using System;
using System.Collections.Concurrent;

namespace TestingGarbage
{
	public class SKObject : IDisposable
	{
		public static readonly ConcurrentDictionary<IntPtr, WeakReference> instances = new ConcurrentDictionary<IntPtr, WeakReference>();

		private IntPtr _hnd;
		private bool isDisposed;

		public SKObject(IntPtr handle)
		{
			Handle = handle;
		}

		public IntPtr Handle
		{
			get => _hnd;
			protected set
			{
				if (value == IntPtr.Zero)
				{
					DeregisterHandle(_hnd, this);
					_hnd = value;
				}
				else
				{
					_hnd = value;
					RegisterHandle(_hnd, this);
				}
			}
		}

		public static void RegisterHandle(IntPtr handle, SKObject instance)
		{
			if (handle == IntPtr.Zero || instance == null)
				return;

			var weak = new WeakReference(instance);
			instances.AddOrUpdate(handle, weak, Update);

			WeakReference Update(IntPtr key, WeakReference oldValue)
			{
				if (oldValue.Target is SKObject obj)
					obj.Disposepublic();

				return weak;
			}
		}

		public static void DeregisterHandle(IntPtr handle, SKObject instance)
		{
			if (handle == IntPtr.Zero)
				return;

			instances.TryRemove(handle, out _);
		}

		public static bool GetInstance<TSkiaObject>(IntPtr handle, out TSkiaObject instance)
			where TSkiaObject : SKObject
		{
			if (instances.TryGetValue(handle, out var weak))
			{
				if (weak.Target is TSkiaObject obj && obj.Handle != IntPtr.Zero)
				{
					instance = obj;
					return true;
				}
			}
			instance = null;
			return false;
		}

		~SKObject()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (isDisposed)
				return;
			isDisposed = true;

			Handle = IntPtr.Zero;
		}

		public void Dispose()
		{
			Disposepublic();
		}

		protected void Disposepublic()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}

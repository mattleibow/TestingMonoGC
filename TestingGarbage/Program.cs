using System;
using System.Collections.Concurrent;
using Xunit;

namespace TestingGarbage
{
	public class MainClass
	{
		[Fact]
		public void Carrots()
		{
			var handle = (IntPtr)666;

			Construct();

			GC.Collect();
			GC.WaitForPendingFinalizers();

			Assert.False(SKObject.GetInstance<SKObject>(handle, out _));

			void Construct()
			{
				var inst1 = new SKObject(handle);
				var inst2 = new SKObject(handle);

				Assert.NotSame(inst1, inst2);
			}
		}
	}
}

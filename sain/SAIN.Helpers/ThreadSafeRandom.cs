using System;
using System.Threading;

namespace SAIN.Helpers;

public static class ThreadSafeRandom
{
	[ThreadStatic]
	private static Random Local;

	public static Random ThisThreadsRandom => Local ?? (Local = new Random(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId));
}

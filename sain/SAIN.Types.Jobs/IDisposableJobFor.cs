using System;
using Unity.Jobs;

namespace SAIN.Types.Jobs;

public interface IDisposableJobFor : IJobFor, IDisposable
{
}

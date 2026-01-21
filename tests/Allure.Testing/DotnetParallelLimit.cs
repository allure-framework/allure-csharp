using System;
using TUnit.Core.Interfaces;

namespace Allure.Testing;

public class DotnetParallelLimit : IParallelLimit
{
    public int Limit => Environment.ProcessorCount;
}
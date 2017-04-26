﻿#if NET20 || NET30

namespace Theraot.Core.System
{
    /// <summary>Encapsulates a method that has one parameter and returns a value of the type specified by the <typeparam name="TResult" /> parameter.</summary>
    /// <typeparam name="T">The type of the parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <param name="arg">The parameter of the method that this delegate encapsulates.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
#if NETCF

    public delegate TResult Func<T, TResult>(T arg);

#else

    public delegate TResult Func<in T, out TResult>(T arg);

#endif
}

#endif
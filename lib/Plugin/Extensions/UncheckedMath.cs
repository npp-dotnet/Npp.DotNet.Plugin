/*
 * SPDX-FileCopyrightText: 2025 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Npp.DotNet.Plugin.Extensions
{
    static class UncheckedMath
    {
        /// <summary>
        /// Increments <paramref name="value"/>, discarding the sign bit in case of overflow.
        /// </summary>
        /// <returns>
        /// The unsigned result of adding 1 to <paramref name="value"/>.
        /// </returns>
        internal static int Increment(int value)
        {
            int abs = unchecked(value + 1) & int.MaxValue;
            return (abs > 0) ? abs : 1;
        }

#if NET7_0_OR_GREATER
        /// <inheritdoc cref="Increment(int)" />
        internal static T Increment<T>(T value) where T : IBinaryInteger<T>, IMinMaxValue<T>
        {
            T abs = unchecked(value + T.One) & T.MaxValue;
            return (abs > T.Zero) ? abs : T.One;
        }
#endif
    }
}

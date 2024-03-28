/*
 * SPDX-FileCopyrightText: 2023 Mark Johnston Olson <https://github.com/molsonkiko>
 *
 * SPDX-License-Identifier: Apache-2.0
 */
namespace Npp.DotNet.Plugin
{
    public class NanInf
    {
        /// <summary>
        /// a/b<br></br>
        /// may be necessary to generate infinity or nan at runtime
        /// to avoid the compiler pre-computing things<br></br>
        /// since if the compiler sees literal division by 0d in the code
        /// it just pre-computes it at compile time
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Divide(double a, double b) { return a / b; }

        public static double Inf { get => Divide(1d, 0d); }
        public static double NegInf { get => Divide(-1d, 0d); }
        public static double Nan { get => Divide(0d, 0d); }
    }
}

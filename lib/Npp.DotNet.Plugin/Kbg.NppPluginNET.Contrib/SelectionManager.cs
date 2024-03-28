﻿/*
 * SPDX-FileCopyrightText: 2024 Mark Johnston Olson <https://github.com/molsonkiko>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Npp.DotNet.Plugin
{
    public class SelectionManager
    {
        private static readonly Regex START_END_REGEX = new Regex(@"^\d+,\d+$", RegexOptions.Compiled);

        public static bool IsStartEnd(string x) => START_END_REGEX.IsMatch(x);

        public static (int start, int end) ParseStartEndAsTuple(string startEnd)
        {
            int[] startEndNums = ParseStartEnd(startEnd);
            return (startEndNums[0], startEndNums[1]);
        }

        public static List<(int start, int end)> GetSelectedRanges()
        {
            var selList = new List<(int start, int end)>();
            int selCount = NppUtils.Editor.GetSelections();
            for (int ii = 0; ii < selCount; ii++)
                selList.Add((NppUtils.Editor.GetSelectionNStart(ii), NppUtils.Editor.GetSelectionNEnd(ii)));
            return selList;
        }

        public static bool NoTextSelected(IList<(int start, int end)> selections)
        {
            (int start, int end) = selections[0];
            return selections.Count < 2 && start == end;
        }

        public static bool NoTextSelected(IList<string> selections)
        {
            int[] startEnd = ParseStartEnd(selections[0]);
            return selections.Count < 2 && startEnd[0] == startEnd[1];
        }

        /// <summary>
        /// takes a list of one or more comma-separated integers
        /// and transforms it into an array of numbers.
        /// </summary>
        /// <param name="startEnd"></param>
        /// <returns></returns>
        public static int[] ParseStartEnd(string startEnd)
        {
            return startEnd.Split(',').Select(s => int.Parse(s)).ToArray();
        }

        public static List<(int start, int end)> SetSelectionsFromStartEnds(IEnumerable<string> startEnds)
        {
            int ii = 0;
            NppUtils.Editor.ClearSelections();
            var result = new List<(int start, int end)>();
            foreach (string startEnd in startEnds)
            {
                (int start, int end) = ParseStartEndAsTuple(startEnd);
                if (start > end)
                    (start, end) = (end, start);
                result.Add((start, end));
                if (ii++ == 0)
                {
                    // first selection is handled differently
                    NppUtils.Editor.SetSelectionStart(start);
                    NppUtils.Editor.SetSelectionEnd(end);
                }
                else
                {
                    NppUtils.Editor.AddSelection(start, end);
                }
            }
            return result;
        }

        /// <summary>
        /// extract INTEGER1 from string of form INTEGER1,INTEGER2
        /// </summary>
        public static int StartFromStartEnd(string s)
        {
            int commaIdx = s.IndexOf(',');
            return int.Parse(s.Substring(0, commaIdx));
        }

        /// <summary>
        /// compare two strings, "INTEGER1,INTEGER2" and "INTEGER3,INTEGER4"
        /// comparing INTEGER1 to INTEGER3
        /// </summary>
        public static int StartEndCompareByStart(string s1, string s2)
        {
            return StartFromStartEnd(s1).CompareTo(StartFromStartEnd(s2));
        }

        public static int StartEndCompareByStart((int start, int end) se1, (int start, int end) se2)
        {
            return se1.start.CompareTo(se2.start);
        }

        /// <summary>
        /// Given selections (selstart1,selend1), (selstart2,selend2), ..., (selstartN,selendN)<br></br>
        /// returns a sep-separated list of "start,end" pairs.<br></br>
        /// EXAMPLE:<br></br>
        /// * StartEndListToString([(1, 2), (5, 7)], "], [") returns "1,2], [5,7"<br></br>
        /// * StartEndListToString([(1, 2), (9, 20), (30,45)], " ") returns "1,2 9,20 30,45"
        /// </summary>
        public static string StartEndListToString(IEnumerable<(int start, int end)> selections, string sep = " ")
        {
            return string.Join(sep, selections.OrderBy(x => x.start).Select(x => $"{x.start},{x.end}"));
        }

        /// <summary>
        /// equivalent to sep.Join(selections)
        /// </summary>
        public static string StartEndListToString(IEnumerable<string> selections, string sep = " ")
        {
            return string.Join(sep, selections.OrderBy(StartFromStartEnd));
        }
    }
}

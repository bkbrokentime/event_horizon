using System;
using System.Collections.Generic;
using System.Linq;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Serializable;
using GameDatabase.Model;
using UnityEngine;

namespace GameDatabase.Utils
{
    public static class UAVPlatformConverter
    {
        public static List<UAVLaunchPlatform> Convert(Layout layout, UAVLaunchPlatformSerializable[] serializableUAVLaunchPlatform)
        {
            var UAVLaunchPlatforms = new List<UAVLaunchPlatform>();
            var indices = FindUAVLaunchPlatforms(layout);
            var count = serializableUAVLaunchPlatform?.Length ?? 0;

            if (indices.Count != count)
                Debug.LogException(new ArgumentOutOfRangeException("serializableBarrels", "barrels do not fit layout"));

            for (var i = 0; i < count && i < indices.Count; ++i)
                UAVLaunchPlatforms.Add(new UAVLaunchPlatform(serializableUAVLaunchPlatform[i], null, indices[i]));

            return UAVLaunchPlatforms;
        }

        private static List<int> FindUAVLaunchPlatforms(Layout layout)
        {
            var index = 0;
            var size = layout.Size;
            var UAVLaunchPlatforms = layout.Data.Select(item => item == (char)CellType.UAVPlatform ? index++ : -1).ToArray();

            for (var k = 0; k < 1000; ++k)
            {
                var shoulContinue = false;

                for (var i = 0; i < size; ++i)
                {
                    for (var j = 0; j < size; ++j)
                    {
                        shoulContinue |= CheckUAVPlatformsCell(UAVLaunchPlatforms, j, i, size);
                        shoulContinue |= CheckUAVPlatformsCell(UAVLaunchPlatforms, size - j - 1, size - i - 1, size);
                    }
                }

                if (!shoulContinue) break;
            }

            var indices = new List<int>();
            var lastValue = -1;
            for (var i = 0; i < UAVLaunchPlatforms.Length; ++i)
            {
                var value = UAVLaunchPlatforms[i];
                if (value < 0 || value <= lastValue) continue;

                indices.Add(i);
                lastValue = value;
            }

            return indices;
        }

        private static bool CheckUAVPlatformsCell(int[] cells, int x, int y, int size)
        {
            var index = y * size + x;
            var value = cells[index];
            if (value < 0) return false;

            var left = x > 0 ? cells[y * size + x - 1] : -1;
            var right = x + 1 < size ? cells[y * size + x + 1] : -1;
            var top = y > 0 ? cells[(y - 1) * size + x] : -1;
            var bottom = y + 1 < size ? cells[(y + 1) * size + x] : -1;

            var newValue = value;
            if (left >= 0 && left < newValue) newValue = left;
            if (right >= 0 && right < newValue) newValue = right;
            if (top >= 0 && top < newValue) newValue = top;
            if (bottom >= 0 && bottom < newValue) newValue = bottom;

            if (newValue < value)
            {
                cells[index] = newValue;
                return true;
            }

            return false;
        }
    }
}

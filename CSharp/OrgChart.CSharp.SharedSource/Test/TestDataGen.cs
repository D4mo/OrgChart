﻿/*
 * Copyright (c) Roman Polunin 2016. 
 * MIT license, see https://opensource.org/licenses/MIT. 
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using OrgChart.Annotations;
using OrgChart.Layout;

namespace OrgChart.Test
{
    /// <summary>
    /// Test data generator utility.
    /// </summary>
    public class TestDataGen
    {
        /// <summary>
        /// Adds some data items into supplied <paramref name="dataSource"/>.
        /// </summary>
        public void GenerateDataItems([NotNull] TestDataSource dataSource, int count, int percentAssistants)
        {
            foreach (var item in GenerateRandomDataItems(count, percentAssistants))
            {
                dataSource.Items.Add(item.Id, item);
            }
        }

        private List<TestDataItem> GenerateRandomDataItems(int itemCount, int percentAssistants)
        {
            if (itemCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(itemCount), itemCount, "Count must be zero or positive");
            }

            var random = new Random(0);

            var items = new List<TestDataItem>(itemCount);
            for (var i = 0; i < itemCount; i++)
            {
                items.Add(new TestDataItem
                {
                    Id = i.ToString()
                });
            }

            var firstInLayer = 1;
            var prevLayerSize = 1;
            while (firstInLayer < itemCount)
            {
                var layerSize = 15 + prevLayerSize + random.Next(prevLayerSize * 2);
                for (var i = firstInLayer; i < firstInLayer + layerSize && i < itemCount; i++)
                {
                    var parentIndex = firstInLayer - 1 - random.Next(prevLayerSize);
                    items[i].ParentId = items[parentIndex].Id;
                }

                firstInLayer = firstInLayer + layerSize;
                prevLayerSize = layerSize;
            }

            // now shuffle the items a bit, to prevent clients from assuming that data always comes in hierarchical order
            for (var i = 0; i < items.Count/2; i++)
            {
                var from = random.Next(items.Count);
                var to = random.Next(items.Count);
                var temp = items[from];
                items[from] = items[to];
                items[to] = temp;
            }

            // now mark first five boxes 
            if (percentAssistants > 0)
            {
                var assistantCount = Math.Min(items.Count, (int) Math.Ceiling(items.Count * percentAssistants / 100.0));
                for (var i = 0; i < assistantCount; i++)
                {
                    items[random.Next(items.Count)].IsAssistant = true;
                }
            }

            return items;
        }

        /// <summary>
        /// Some random box sizes.
        /// </summary>
        public static void GenerateBoxSizes([NotNull] BoxContainer boxContainer)
        {
            const int minWidth = 50;
            const int minHeight = 50;
            const int widthVariation = 50;
            const int heightVariation = 50;

            var seed = 0;//Environment.TickCount;
            Debug.WriteLine(seed.ToString());
            var random = new Random(seed);
            foreach (var box in boxContainer.BoxesById.Values)
            {
                if (!box.IsSpecial)
                {
                    box.Size = new Size(minWidth + random.Next(widthVariation), minHeight + random.Next(heightVariation));
                }
            }
        }
    }
}
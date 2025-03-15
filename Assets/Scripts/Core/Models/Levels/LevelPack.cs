using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Soko.Core.Extensions;
using Soko.Unity.DataLayer.So;
using UnityEditor;
using UnityEngine;

namespace Soko.Core.Models.Levels
{
    [Serializable]
    public class LevelPack
    {
        public string Name;
        public string MusicKey;
        public Sprite LevelBackground;
        public Sprite HeaderSprite;
        [HideReferenceObjectPicker] public List<LevelData2> Levels2;

        // [Button]
        // public void Sync()
        // {
        //     Levels2 = new();
        //     var colors = AssetDatabase.LoadAssetAtPath<ColorDataSo>("Assets/Data/Game Data/ColorData.asset");
        //     for (int l = 0; l < Levels.Count; l++)
        //     {
        //         var level = Levels[l];
        //         Levels2.Add(new ());
        //         var level2 = Levels2[l];
        //         level2.Name = level.Name;
        //         
        //         var keys = level.LevelMap.Split('\n')
        //             .Select(rowString => rowString.SplitByTwoSeparators('[', ']'))
        //             .ToList();
        //         
        //         var columns = keys.Max(list => list.Count); // longest row
        //         var rows = keys.Count;
        //
        //         level2.Cells = new CellData[columns, rows];
        //         for (int x = 0; x < level2.Cells.GetLength(0); x++)
        //             for (int y = 0; y < level2.Cells.GetLength(1); y++)
        //                 level2.Cells[x, y] ??= new CellData();
        //         
        //         for (int y = 0; y < rows; y++)
        //         {
        //             for (int x = 0; x < columns; x++)
        //             {
        //                 var key = keys[y][x];
        //                 var data = new Dictionary<string, string>();
        //                 if (string.IsNullOrWhiteSpace(key)) continue;
        //                 
        //                 var dataSeparatorPosition = key.IndexOf('|');
        //                 if (dataSeparatorPosition > 0) // has special data
        //                 {
        //                     var keyData = key.Split('|');
        //                     key = keyData[0];
        //
        //                     for (int i = 1; i < keyData.Length; i++)
        //                     {
        //                         var dataElement = keyData[i];
        //                         var dataSplit = dataElement.Split(':');
        //                         data.Add(dataSplit[0], dataSplit[1]);
        //                     }
        //                 }
        //                 
        //                 level2.Cells[x, y].ObjectKey = key;
        //                 if (data.TryGetValue("c", out var value1))
        //                 {
        //                     level2.Cells[x, y].Color = colors.Colors[value1].Color;
        //                 }
        //
        //                 if (data.TryGetValue("g", out var value))
        //                 {
        //                     level2.Cells[x, y].Group = int.Parse(value);
        //                 }
        //             }
        //         }
        //     }
        // }
    }
}
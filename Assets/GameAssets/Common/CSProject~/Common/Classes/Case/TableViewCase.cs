
using Unity.Widget;
using Unity.Core;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using Unity.Utility;
using WLCore.Helper;
using System.Diagnostics;
using System.IO;

namespace Common
{
    public class TableViewCase : BaseCaseStage
    {
        private TableView _tableView;

        public override void OnInitialize()
        {
            //AssetsManager.SetLoadType(AssetsManager.LoadType.Local);

            base.OnInitialize();

            var prefab = WNode.Create("Common/Module2/prefabs3", "TabelViewCase.prefab");
            prefab.AddTo(root);

            _tableView = prefab.FindReference<TableView>("Scroll View");
            _tableView.NumberOfCellsCallback((tb) =>
            {
                return 10;
            })
            .TableCellSizeForIndexCallback((tb) =>
            {
                return new Vector2(300, 100);
            })
            .TableCellAtIndexCallback((tb, index) =>
            {
                var cell = tb.DequeueCell() as TableViewCellCase;
                if (cell == null)
                {
                    cell = new TableViewCellCase();
                }

                cell.SetString(index.ToString());
                return cell;
            });

            _tableView.ReloadData();
        }

        public void CaseJumpTo()
        {
            _tableView.JumpToPos(-1000, 0);
        }
    }
}

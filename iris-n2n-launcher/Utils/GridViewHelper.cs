namespace iris_n2n_launcher.Utils;

internal class GridViewHelper
{
    /// <summary>
    /// 初始化一个 dataGridView
    /// </summary>
    /// <param name="dataGridView"></param>
    public static void InitializeGridView(DataGridView dataGridView)
    {
        dataGridView.AutoGenerateColumns = true;
        dataGridView.RowHeadersVisible = false;
        dataGridView.ReadOnly = true;
        dataGridView.ColumnHeadersVisible = false;
        dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridView.MultiSelect = false;
        dataGridView.AllowUserToAddRows = false;
        dataGridView.AllowUserToResizeColumns = false;
        dataGridView.AllowUserToResizeRows = false;

        dataGridView.CellDoubleClick += (s, e) =>
        {
            if (e.RowIndex >= 0)
            {
                var row = dataGridView.Rows[e.RowIndex];
                List<string> rowData = [];
                foreach (DataGridViewCell cell in row.Cells)
                    rowData.Add(cell.Value?.ToString() ?? "");

                if (dataGridView.Tag is Dictionary<string, object> tagDict &&
                    tagDict.TryGetValue("OnRowDoubleClick", out var act) &&
                    act is Action<List<string>> cb)
                {
                    cb(rowData);
                }
            }
        };

        dataGridView.CellMouseEnter += (s, e) =>
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var cell = dataGridView[e.ColumnIndex, e.RowIndex];
                var val = cell.Value?.ToString();
                cell.ToolTipText = val ?? "";
            }
        };

    }
    public static void UpdateData(DataGridView dataGridView, List<List<string>> dataSource, List<float> columnWeights)
    {
        if (dataGridView.ColumnCount != (dataSource.FirstOrDefault()?.Count ?? 0) - 1)
        {
            dataGridView.Columns.Clear();
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dataSource.Count > 0 && dataSource[0].Count > 1)
            {
                int colCount = dataSource[0].Count - 1;
                for (int i = 0; i < colCount; i++)
                {
                    var column = new DataGridViewTextBoxColumn
                    {
                        Name = $"col{i + 1}",
                        HeaderText = $"列{i + 1}",
                        FillWeight = (columnWeights != null && i < columnWeights.Count) ? columnWeights[i] : 1f,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                    };
                    dataGridView.Columns.Add(column);
                }
            }
        }

        HashSet<string> existingKeys = [];
        Dictionary<string, int> keyToRowMap = [];

        for (int i = 0; i < dataGridView.Rows.Count; i++)
        {
            if (dataGridView.Rows[i].Tag is string key)
            {
                existingKeys.Add(key);
                keyToRowMap[key] = i;
            }
        }

        foreach (var rowData in dataSource)
        {
            if (rowData.Count == 0) continue;
            string rowKey = rowData[0]; // 第一个值用于唯一检测
            var visibleData = rowData.Skip(1).ToArray();

            if (existingKeys.Contains(rowKey))
            {
                // 比较是否有变化，若有则更新
                int rowIndex = keyToRowMap[rowKey];
                bool changed = false;
                for (int i = 0; i < visibleData.Length; i++)
                {
                    var cell = dataGridView.Rows[rowIndex].Cells[i];
                    if (cell.Value?.ToString() != visibleData[i])
                    {
                        cell.Value = visibleData[i];
                        changed = true;
                    }
                }
                if (!changed)
                    continue; // 没变化则跳过
            }
            else
            {
                // 新行
                int idx = dataGridView.Rows.Add(visibleData);
                dataGridView.Rows[idx].Tag = rowKey;
            }
        }

        // 删除不存在的旧行
        for (int i = dataGridView.Rows.Count - 1; i >= 0; i--)
        {
            var key = dataGridView.Rows[i].Tag as string;
            if (key != null && !dataSource.Any(r => r.Count > 0 && r[0] == key))
            {
                dataGridView.Rows.RemoveAt(i);
            }
        }
    }
}

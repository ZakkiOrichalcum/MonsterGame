using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCG.Utility
{
    public class Table<T>
    {
        public ColumnRow<T> TitleRow { get; set; }
        public TableRow<T>[] Rows { get; set; }

        #region CONSTRUCTOR
        public Table()
        {
            this.TitleRow = null;
            this.Rows = new TableRow<T>[0];
        }

        public Table(T[][] data, bool firstRowisTitles)
        {
            var startingIndex = 0;
            if (firstRowisTitles)
            {
                this.TitleRow = new ColumnRow<T>(data[0]);
                startingIndex++;
            }
            this.Rows = new TableRow<T>[data.GetLength(0) - startingIndex];
            for (var i = startingIndex; i < data.GetLength(0); i++)
            {
                this.Rows[i - startingIndex] = new TableRow<T>(data[i]);
            }
        }

        public Table(T[][] data, bool firstRowisTitles, int[] columnOrder)
        {
            var startingIndex = 0;
            if (firstRowisTitles)
            {
                this.TitleRow = new ColumnRow<T>(data[0], columnOrder);
                startingIndex++;
            }
            this.Rows = new TableRow<T>[data.GetLength(0) - startingIndex];
            for (var i = startingIndex; i < data.GetLength(0); i++)
            {
                this.Rows[i - startingIndex] = new TableRow<T>(data[i], columnOrder);
            }
        }

        public Table(T[][] data, bool firstRowisTitles, string columnOrder)
        {
            var startingIndex = 0;
            if (firstRowisTitles)
            {
                this.TitleRow = new ColumnRow<T>(data[0], columnOrder);
                startingIndex++;
            }
            this.Rows = new TableRow<T>[data.GetLength(0) - startingIndex];
            for (var i = startingIndex; i < data.GetLength(0); i++)
            {
                this.Rows[i - startingIndex] = new TableRow<T>(data[i], columnOrder);
            }
        }

        public Table(T[] titleRow, T[][] data, int[] columnOrder)
        {
            this.TitleRow = new ColumnRow<T>(titleRow, columnOrder);
            this.Rows = new TableRow<T>[data.GetLength(0)];
            for (var i = 0; i < data.GetLength(0); i++)
            {
                this.Rows[i] = new TableRow<T>(data[i], columnOrder);
            }
        }

        public Table(ColumnRow<T> titles, T[][] data)
        {
            this.TitleRow = titles;
            this.Rows = new TableRow<T>[data.GetLength(0)];
            for (var i = 0; i < data.GetLength(0); i++)
            {
                this.Rows[i] = new TableRow<T>(data[i]);
            }
        }
        public Table(ColumnRow<T> titles, T[][] data, int[] columnOrder)
        {
            this.TitleRow = titles;
            this.Rows = new TableRow<T>[data.GetLength(0)];
            for (var i = 0; i < data.GetLength(0); i++)
            {
                this.Rows[i] = new TableRow<T>(data[i], columnOrder);
            }
        }
        public Table(string titleJson, string dataJson)
        {
            var titles = JSON.Parse(titleJson);
            var data = JSON.Parse(dataJson);

            var entire = new JSONClass();
            entire["titles"] = titles;
            entire["rows"] = data;

            InitializeTableWithJson(entire);
        }
        public Table(string entireJson)
        {
            InitializeTableWithJson(JSON.Parse(entireJson));
        }
        private void InitializeTableWithJson(JSONNode json)
        {
            var columns = new List<Column<T>>();
            var titles = json["titles"];
            for (var i = 0; i < titles.Count; i++)
            {
                var value = titles[i];
                var col = new Column<T>() { Item = (T)Objects.ConvertAny<T>(value["item"]), ColumnType = value["type"], DisplayName = value["displayName"] };
                if (value["details"] != null)
                    col.Details = value["details"];

                columns.Add(col);
            }
            this.TitleRow = new ColumnRow<T>(columns.ToArray());

            var tableRows = new List<TableRow<T>>();
            var rows = json["rows"];
            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                var tRow = new List<T>();
                for (var j = 0; j < row.Count; j++)
                {
                    tRow.Add((T)Objects.ConvertAny<T>(row[j]));
                }

                tableRows.Add(new TableRow<T>(tRow.ToArray()));
            }
            this.Rows = tableRows.ToArray();
        }

        #endregion

        public void AppendTableRows(Table<T> otherTable)
        {
            var outputRows = new List<TableRow<T>>();
            outputRows.AddRange(Rows);
            outputRows.AddRange(otherTable.GetRowsInColumnOrder());

            this.Rows = outputRows.ToArray();
        }

        public void AddColumnOrder(int[] columnOrder)
        {
            if (TitleRow != null)
                TitleRow.ColumnOrder = columnOrder;

            foreach (var row in Rows)
            {
                row.ColumnOrder = columnOrder;
            }
        }
        public void AddColumnOrder(string columnOrderString)
        {
            AddColumnOrder(columnOrderString.Split(',').Select(x => int.Parse(x)).ToArray());
        }
        public void AddColumnTypes(string columnTypes)
        {
            TitleRow.AddColumnTypes(columnTypes);
        }
        public void AddTitleRowInformation(T[] items, string[] displayNames, string[] typings, string[] details, int[] columnOrder)
        {
            var columnRows = new ColumnRow<T>(items, typings, displayNames, details);
            ReorderRowData(columnOrder);
            TitleRow = columnRows;
        }
        private void ReorderRowData(int[] ordering)
        {
            foreach (var row in Rows)
            {
                row.ReorderRow(ordering);
            }
        }

        public string ToJSON()
        {
            var sb = new StringBuilder("{");
            sb.Append("\"titles\":");
            if (TitleRow != null)
            {
                sb.Append(TitleRow.ToJson());
            }
            else
            {
                sb.Append(GetDefaultTitleRow().ToJson());
            }
            sb.Append(",");
            if (Rows != null && Rows.Length > 0)
            {
                sb.Append("\"rows\":");
                sb.Append(GetJSONData());
            }
            else
            {
                sb.Append("\"rows\":[]");
            }

            sb.Append("}"); //end of complete object

            return sb.ToString();
        }
        public string GetJSONTitles()
        {
            if (TitleRow == null)
                throw new NullReferenceException("Titles row not defined.");

            return TitleRow.ToJson();
        }
        public string GetJSONData()
        {
            var sb = new StringBuilder("[");

            if (Rows != null && Rows.Length > 0)
            {

                for (var i = 0; i < Rows.Length; i++)
                {
                    sb.Append(Rows[i].ToJSON());
                    if (i < Rows.Length - 1)
                        sb.Append(",");

                }
            }
            else
            {
                sb.Append("");
            }

            sb.Append("]"); //end of complete object

            return sb.ToString();
        }

        public string ToXElementString()
        {
            var sb = new StringBuilder();
            sb.Append("<Data>");
            if (Rows != null && Rows.Length > 0)
            {
                for (var i = 0; i < Rows.Length; i++)
                {
                    sb.Append(Rows[i].ToXElementString());
                }
            }
            sb.Append("</Data>"); //end of complete object

            return sb.ToString();
        }

        public string ToXElementTitles()
        {
            if (TitleRow != null)
            {
                return TitleRow.ToXElementString();
            }
            else
            {
                return GetDefaultTitleRow().ToXElementString();
            }
        }

        public TableRow<T>[] GetRowsInColumnOrder()
        {
            var output = new List<TableRow<T>>();
            foreach (var row in Rows)
            {
                var tableRow = new TableRow<T>(row.GetRowInOrder());
                var num = new int[tableRow.Columns.Length];
                for (var i = 0; i < tableRow.Columns.Length; i++)
                {
                    num[i] = i;
                }
                tableRow.ColumnOrder = num;

                output.Add(tableRow);
            }

            return output.ToArray();
        }

        private ColumnRow<string> GetDefaultTitleRow()
        {
            var data = new string[Rows[0].ColumnOrder.Length];
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = i.ToString();
            }

            return new ColumnRow<string>(data);
        }
    }

    public class TableRow<T>
    {
        public T[] Columns { get; set; }
        public int[] ColumnOrder { get; set; }

        public TableRow(T[] data)
        {
            this.Columns = data;
            this.ColumnOrder = new int[Columns.Length];
            for (int i = 0; i < Columns.Length; i++)
            {
                ColumnOrder[i] = i;
            }
        }

        public TableRow(T[] data, string columnOrderStr)
        {
            this.Columns = data;
            this.ColumnOrder = columnOrderStr.Split(',').Select(x => int.Parse(x)).ToArray();
        }

        public TableRow(T[] data, int[] columnOrder)
        {
            this.Columns = data;
            this.ColumnOrder = columnOrder;
        }

        public T GetColumn(int index)
        {
            if (index < Columns.Length)
                return Columns[index];
            else
                return default(T);
        }

        public void ReorderRow(int[] ordering)
        {
            var row = new T[ordering.Length];
            for (int i = 0; i < ordering.Length; i++)
            {
                row[i] = GetColumn(ordering[i]);
            }
            Columns = row;

            ColumnOrder = new int[ordering.Length];
            for (int i = 0; i < ordering.Length; i++)
            {
                ColumnOrder[i] = i;
            }
        }

        public T[] GetRowInOrder()
        {
            var output = new List<T>();

            for (int i = 0; i < ColumnOrder.Length; i++)
            {
                output.Add(GetColumn(ColumnOrder[i]));
            }

            return output.ToArray();
        }

        public string ToJSON()
        {
            var sb = new StringBuilder();
            sb.Append("[");

            for (int i = 0; i < ColumnOrder.Length; i++)
            {
                sb.Append("\"" + GetColumn(ColumnOrder[i]).ToString() + "\"");
                if (i < Columns.Length - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append("]");

            return sb.ToString();
        }

        public string ToXElementString()
        {
            var sb = new StringBuilder();
            sb.Append("<Row><Columns>");

            for (int i = 0; i < Columns.Length; i++)
            {
                sb.Append("<Column>" + GetColumn(ColumnOrder[i]).ToString() + "</Column>");
            }

            sb.Append("</Columns></Row>");

            return sb.ToString();
        }

        public string ToXElementString(string[] columnTypes)
        {
            var sb = new StringBuilder();
            sb.Append("<Row><Columns>");

            for (int i = 0; i < Columns.Length; i++)
            {
                sb.Append("<Column type=\"" + columnTypes[ColumnOrder[i]] + "\">" + GetColumn(ColumnOrder[i]) + "</Column>");
            }

            sb.Append("</Columns></Row>");

            return sb.ToString();
        }
    }

    public class ColumnRow<T>
    {
        public Column<T>[] Columns { get; set; }
        public int[] ColumnOrder { get; set; }

        public ColumnRow(T[] data)
        {
            Initate(data);
        }
        public ColumnRow(T[] data, string[] typings)
        {
            Initate(data, typings);
        }
        public ColumnRow(T[] data, string[] typings, int[] columnOrder)
        {
            Initate(data, typings, columnOrder);
        }
        public ColumnRow(T[] data, string[] typings, int[] columnOrder, string[] displayNames)
        {
            Initate(data, typings, columnOrder, displayNames);
        }
        public ColumnRow(T[] data, string[] typings, int[] columnOrder, string[] displayNames, string[] details)
        {
            Initate(data, typings, columnOrder, displayNames, details);
        }
        public ColumnRow(T[] data, string[] typings, string[] displayNames, string[] details)
        {
            Initate(data, typings, null, displayNames, details);
        }
        public ColumnRow(T[] data, string[] typings, string[] displayNames)
        {
            Initate(data, typings, null, displayNames);
        }
        public ColumnRow(T[] data, int[] columnOrder)
        {
            Initate(data, null, columnOrder);
        }
        public ColumnRow(T[] data, string columnOrder)
        {
            Initate(data, null, columnOrder.Split(',').Select(x => int.Parse(x)).ToArray());
        }
        public ColumnRow(Column<T>[] columns)
        {
            this.Columns = columns;
            this.ColumnOrder = new int[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                ColumnOrder[i] = i;
            }
        }
        public ColumnRow(Column<T>[] columns, int[] columnOrder)
        {
            this.Columns = columns;
            this.ColumnOrder = columnOrder;
        }
        private void Initate(T[] data, string[] typings = null, int[] columnOrder = null, string[] displayNames = null, string[] details = null)
        {
            if (typings == null)
            {
                typings = new string[data.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    typings[i] = "textbox";
                }
            }
            if (columnOrder == null)
            {
                columnOrder = new int[data.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    columnOrder[i] = i;
                }
            }

            if (data.Length != typings.Length)
                throw new ArgumentOutOfRangeException("Typings for Columns and number of Columns must match!");

            var columnList = new List<Column<T>>();
            for (int i = 0; i < data.Length; i++)
            {
                var column = new Column<T>() { Item = data[i], ColumnType = typings[i] };

                if (displayNames != null)
                    column.DisplayName = displayNames[i];
                if (details != null)
                    column.Details = (details[i] != "") ? details[i] : null;

                columnList.Add(column);
            }

            this.Columns = columnList.ToArray();
            this.ColumnOrder = columnOrder;
        }

        public void AddColumnTypes(string columnTypes)
        {
            var typings = columnTypes.Split(',');
            for (int i = 0; i < typings.Length; i++)
            {
                Columns[ColumnOrder[i]].ColumnType = typings[ColumnOrder[i]];
            }
        }



        public string ToJson()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < ColumnOrder.Length; i++)
            {
                sb.Append(GetOriginalIndexedColumn(ColumnOrder[i]).ToJson());
                if (i < Columns.Length - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append("]");

            return sb.ToString();
        }

        public string ToXElementString()
        {
            var sb = new StringBuilder();

            sb.Append("<Header><Columns>");
            for (int i = 0; i < ColumnOrder.Length; i++)
            {
                sb.Append(GetOriginalIndexedColumn(ColumnOrder[i]).ToXElementString());
            }
            sb.Append("</Columns></Header>");

            return sb.ToString();
        }

        public Column<T> GetColumn(int index)
        {
            return GetOriginalIndexedColumn(ColumnOrder[index]);
        }

        private Column<T> GetOriginalIndexedColumn(int index)
        {
            if (index < Columns.Length)
                return Columns[index];
            else
                return new Column<T>();
        }
    }

    public class Column<T>
    {
        public T Item { get; set; }
        public string ColumnType { get; set; }
        public string DisplayName { get; set; }
        public string Details { get; set; }

        public string ToJson()
        {
            return string.Format("{{ \"item\": \"{0}\", \"type\":\"{1}\", \"displayName\":\"{2}\" {3}}}", Item, ColumnType, ((DisplayName == null) ? Item.ToString() : DisplayName), ((Details != null) ? ", \"details\":\"" + Details + "\"" : ""));
        }

        public string ToXElementString()
        {
            return string.Format("<Column name=\"{0}\" type=\"{1}\"{2}>{3}</Column>", Item, ColumnType, ((Details != null) ? " options=\"" + Details + "\"" : ""), (DisplayName == null) ? Item.ToString() : DisplayName);
        }
    }
}
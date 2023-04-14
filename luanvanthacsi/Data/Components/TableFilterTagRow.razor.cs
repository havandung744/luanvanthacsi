using AntDesign;
using AntDesign.TableModels;
using luanvanthacsi.Data.Extentions;
using luanvanthacsi.Models;
using Microsoft.AspNetCore.Components;

namespace luanvanthacsi.Data.Components
{
    public partial class TableFilterTagRow<TItem>
    {
        [CascadingParameter] public ITable Table { get; set; }
        [Parameter] public EventCallback<QueryModel<TItem>> OnChange { get; set; }
        [Parameter] public bool HasSelectionColumn { get; set; }
        List<SortFilterTag> sortFilterTags = new List<SortFilterTag>();
        public bool HasFilterValue { get; set; }
        Table<TItem> table;
        int removeTagCount;
        protected override void OnInitialized()
        {
            table = Table as Table<TItem>;
            table.OnChange = new EventCallback<QueryModel<TItem>>(this, (Action<QueryModel<TItem>>)BuildFilterTag);
        }

        public void BuildFilterTag(QueryModel<TItem> query)
        {
            if (removeTagCount != 0)
            {
                removeTagCount--;
                return;
            }
            HasFilterValue = query.SortModel.Any(c => c.Sort.IsNotNullOrEmpty()) || query.FilterModel.Any(c => c.SelectedValues?.Any() == true);
            var locale = (TableLocale)Table.GetValue("Locale");
            sortFilterTags.Clear();
            sortFilterTags.AddRange(
                query.SortModel
                .Where(c => c.Sort.IsNotNullOrEmpty())
                .OrderBy(c => c.Priority)
                .Select(c => new SortFilterTag
                {
                    Direction = c.Sort,
                    FieldName = c.FieldName,
                    Type = SortFilterTagType.Sort,
                    FieldDisplayName = GetFieldTitle(table, c.ColumnIndex),
                }));
            sortFilterTags.AddRange(
                query.FilterModel
                .SelectMany(c => c.Filters.Select((f, index) => new SortFilterTag
                {
                    FieldName = c.FieldName,
                    Type = SortFilterTagType.Filter,
                    Value = GetFieldValue(f),
                    Operator = f.FilterCompareOperator.ToString(),
                    Index = index,
                    FieldDisplayName = GetFieldTitle(table, c.ColumnIndex),
                    OperatorDisplay = locale?.FilterOptions?.GetValue(f.FilterCompareOperator.ToString())?.ToString(),
                })));
            if (OnChange.HasDelegate)
            {
                OnChange.InvokeAsync(query);
            }
        }

        string GetFieldTitle<T>(Table<T> table, int columnIndex)
        {
            var column = table.ColumnContext.HeaderColumns.FirstOrDefault(c => c.ColIndex == columnIndex);
            if (column is IFieldColumn fieldColumn)
            {
                return column.Title;
            }
            return string.Empty;
        }

        string GetFieldValue(TableFilter f)
        {
            if (f.Text.IsNotNullOrEmpty())
            {
                return f.Text;
            }
            if (f.FilterCompareOperator == TableFilterCompareOperator.TheSameDateWith && (f.Value is DateTime || f.Value is DateTime?))
            {
                return (f.Value as DateTime?)?.ToString("dd/MM/yyyy");
            }

            return f.Value?.ToString();
        }

        void CloseTag(SortFilterTag tag)
        {
            var queryModel = Table.GetQueryModel();
            HasFilterValue = sortFilterTags.Any();
            if (tag.Type == SortFilterTagType.Sort)
            {
                var sort = queryModel.SortModel.FirstOrDefault(c => c.FieldName == tag.FieldName);
                if (sort != null)
                {
                    queryModel.SortModel.Remove(sort);
                    queryModel.SortModel.Add(new SortModel<string>(sort.ColumnIndex - 1, sort.Priority, sort.FieldName, null));
                }
            }
            else
            {
                var filter = queryModel.FilterModel.FirstOrDefault(c => c.FieldName == tag.FieldName);
                if (filter != null)
                {
                    filter.Filters.RemoveAt(tag.Index);
                    if (!filter.Filters.Any())
                    {
                        queryModel.FilterModel.Remove(filter);
                    }
                }
            }
            if (HasSelectionColumn)
            {
                List<ITableSortModel> softs = new List<ITableSortModel>();
                foreach (var currentSort in queryModel.SortModel)
                {
                    var tableSorter = new SortModel<string>(currentSort.ColumnIndex - 1, currentSort.Priority, currentSort.FieldName, currentSort.Sort);
                    softs.Add(tableSorter);
                }
                queryModel.SortModel.Clear();
                ((List<ITableSortModel>)queryModel.SortModel).AddRange(softs);

                List<ITableFilterModel> filters = new List<ITableFilterModel>();
                foreach (var currentFilter in queryModel.FilterModel)
                {
                    var tableFilter = new FilterModel<string>(currentFilter.ColumnIndex - 1, currentFilter.FieldName,
                        currentFilter.Filters.Where(x => x.Selected).Select(c => c.Value?.ToString()), currentFilter.Filters);
                    filters.Add(tableFilter);
                }
                queryModel.FilterModel.Clear();
                ((List<ITableFilterModel>)queryModel.FilterModel).AddRange(filters);
            }
            removeTagCount = queryModel.FilterModel.Count;
            Table.ReloadData(queryModel);
        }

        void CLearFilter()
        {
            try
            {
                HasFilterValue = false;
                table.ResetData();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

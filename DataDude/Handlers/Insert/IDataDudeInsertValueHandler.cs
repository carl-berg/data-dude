using DataDude.Schema;

namespace DataDude.Handlers.Insert
{
    public interface IDataDudeInsertValueHandler
    {
        ColumnValue Handle(TableInformation table, ColumnInformation column, ColumnValue value);
    }
}

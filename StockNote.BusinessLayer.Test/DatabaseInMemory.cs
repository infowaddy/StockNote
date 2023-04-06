using Microsoft.EntityFrameworkCore;

namespace StockNote.BusinessLayer.Test
{
    public static class DatabaseInMemory
    {
        static internal StockNoteDBContext StockNoteDBContext
        {
            get {
                var builder = new DbContextOptionsBuilder<StockNoteDBContext>()
                    .UseInMemoryDatabase("StockNote" + Guid.NewGuid().ToString());
                var dbContext = new StockNoteDBContext(builder.Options);
                return dbContext;
            }
        }

    }
}

using System.Collections.Generic;

namespace Engine.entity
{
   public interface IReport
    {
        string GetCommand();
        List<string> GetColumnNames();
    }
}

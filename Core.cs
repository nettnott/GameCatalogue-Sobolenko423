using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCatalogue
{
    /// <summary>
    /// Логика взаимодействия с базой данных, используя EntityFramework
    /// Создается статический экземпляр Context сущности (Entities) базы данных для последующего обращения к ней
    /// </summary>
    internal class Core
    {
        public static DB_Sobolenko_GameCatalogueEntities Context = new DB_Sobolenko_GameCatalogueEntities();
    }
}

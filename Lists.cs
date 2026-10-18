using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCatalogue
{
    internal class Lists
    {
        public static List<Category> categoriesDBL = Core.Context.Category.ToList();
        public static List<Games> gamesDBL = Core.Context.Games.ToList();
        public static List<Users> usersDBL = Core.Context.Users.ToList();

        public static Users currentUser = null;
    }
}

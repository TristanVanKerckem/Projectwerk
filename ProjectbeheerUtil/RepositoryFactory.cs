using ProjectbeheerBL.Beheerder;
using ProjectbeheerBL.Interfaces;
using ProjectbeheerDL.Repository;

namespace ProjectbeheerUtil
{
    public static class RepositoryFactory
    {
        public static IProjectRepository GeefRepository(string repoType, string connectionString)
        {
            switch (repoType.ToUpper())
            {
                case "ADO":
                    return new ProjectRepo(connectionString); 
                default:
                    return null;
            }
        }
    }
}

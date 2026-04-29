using ASC.Model;
using System.Linq.Expressions;

namespace ASC.Business.Helpers
{
    public static class Queries
    {
        public static Expression<Func<ServiceRequest, bool>> GetServiceRequestQuery(
            string role,
            string userEmail)
        {
            var predicate = PredicateBuilder.True<ServiceRequest>();

            predicate = predicate.And(x => !x.IsDeleted);

            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return predicate;
            }

            if (role.Equals("Engineer", StringComparison.OrdinalIgnoreCase))
            {
                predicate = predicate.And(x =>
                    x.ServiceEngineer != null &&
                    x.ServiceEngineer == userEmail);

                return predicate;
            }

            predicate = predicate.And(x =>
                x.CustomerEmail == userEmail ||
                x.RequestedBy == userEmail);

            return predicate;
        }
    }
}
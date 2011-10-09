using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Snippets.Repository
{
    /// <summary>
    /// Represents a repository for Policy entities.
    /// </summary>
    public interface IPolicyRepository
    {
        /// <summary>
        /// Gets the Policy entity specified by <paramref name="policyId"/>.
        /// </summary>
        /// <param name="policyId"></param>
        /// <param name="includePaths"></param>
        /// <param name="refreshCache"></param>
        /// <returns></returns>
        Policy GetPolicyByPolicyId(int                                policyId,
                                   Expression<Func<Policy, object>>[] includePaths = null,
                                   bool                               refreshCache = false);

        /// <summary>
        /// Gets all the Policy entities specified by <paramref name="clientId"/>.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="includePaths"></param>
        /// <param name="refreshCache"></param>
        /// <returns></returns>
        IEnumerable<Policy> GetPoliciesByClientId(int                                clientId,
                                                  Expression<Func<Policy, object>>[] includePaths = null,
                                                  bool                               refreshCache = false);
    }
}

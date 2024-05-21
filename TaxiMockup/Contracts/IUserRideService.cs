using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUserRideService :IService
    {
        /// <summary>
        /// Check if provided user is of type driver and verified
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DriverExistsAndVerifiedAsync(Guid id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Check if user request ride is of type user
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task<bool> CheckPasengerTypeAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum UserState
    {
        Created,
        Requested,
        Activated,
        Verified,
        Denied
    }

    public enum UserType
    {
        Admin,
        User,
        Driver
    }

    public enum RideState
    {
        None,
        Requested,
        Confirmed,
        Pending,
        InProgress,
        Finished
    }
}

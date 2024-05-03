using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum UserState
    {
        Default,
        Verified,
        Denied
    }

    public enum UserType
    {
        User,
        Driver,
        Admin
    }

    public enum RideState
    {
        Requested,
        Confirmed,
        Pending,
        InProgress,
        Finished
    }
}

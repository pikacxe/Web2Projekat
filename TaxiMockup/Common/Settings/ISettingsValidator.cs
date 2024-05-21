using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Settings
{
    public interface ISettingsValidator
    {
        bool isValid { get; }
    }
}

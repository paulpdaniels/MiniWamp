using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallWamp
{
    public enum MessageType
    {
        WELCOME = 0,
        CALL,
        CALLRESULT,
        CALLERROR,
        SUBSCRIBE,
        UNSUBSCRIBE,
        PUBLISH,
        EVENT
    }
}

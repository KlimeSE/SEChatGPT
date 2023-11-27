using SEChatGPT.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEChatGPT.Outputs
{
    public class BaseOutput
    {
        public virtual void RegisterConnections(List<BaseConnection> connections)
        {

        }

        public virtual void Unload()
        {

        }
    }
}

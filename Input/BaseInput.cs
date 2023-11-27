using SEChatGPT.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEChatGPT.Inputs
{
    public class BaseInput
    {
        public virtual void RegisterConnections(List<BaseConnection> connections)
        {

        }

        public virtual void Unload()
        {

        }
    }
}

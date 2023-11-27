using SEChatGPT.Inputs;
using SEChatGPT.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEChatGPT.Connections
{
    public  class BaseConnection
    {
        public virtual void RegisterInputs(List<BaseInput> inputs)
        {

        }

        public virtual void RegisterOutputs(List<BaseOutput> outputs)
        {

        }

        public virtual void Unload()
        {

        }
    }
}

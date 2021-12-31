using System;
using System.Collections.Generic;
using System.Text;

namespace BT.Actions
{
    public class GenericNode : ActionNode
    {
        protected override void OnStart()
        {
            m_Start?.Invoke();
        }

        protected override void OnStop()
        {
            m_Stop?.Invoke();
        }

        protected override NodeResult OnUpdate()
        {
            if (m_Update == null)
                return NodeResult.Success;
            return m_Update.Invoke();
        }
    }
}

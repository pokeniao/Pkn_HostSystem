using Pkn_HostSystem.Base.Log;

namespace Pkn_HostSystem.NodifyControl.Operation
{
    public class TestOperation : IStartOperation
    {
        private readonly Func< object[]> _func;

        public LogControl<AddOperation> Log;
        public TestOperation()
        {
            Log = new LogControl<AddOperation>();

            _func = () => [1,2];
        }

        public object[] Execute() => _func.Invoke();

        public object[] Execute(params object[] operands) => _func.Invoke();
    }
}
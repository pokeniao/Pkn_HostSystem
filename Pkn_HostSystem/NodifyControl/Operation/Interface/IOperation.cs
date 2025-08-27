namespace Pkn_HostSystem.NodifyControl.Operation
{
    public interface IOperation
    {
        object[] Execute(params object[] operands);
    }
}

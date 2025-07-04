using AspectCore.DynamicProxy;

namespace Pkn_HostSystem.Base.Attribute
{
    public class HttpAddSql : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            Console.WriteLine();
            await next(context);
            Console.WriteLine();
        }
    }
}
using ArxOne.MrAdvice.Advice;
using ArxOne.MrAdvice.Annotation;
using System;

namespace EasyAppTracing
{
    [ExcludePointcut("*.ctor@", "<@>*")]
    public abstract class BaseLoggingAdviseAttribute : Attribute, IMethodAdvice
    {
        public void Advise(MethodAdviceContext context)
        {
            OnEntry(context);
            try
            {
                context.Proceed();
                OnSuccess(context);
            }
            catch (Exception ex)
            {
                OnException(context, ex);
            }
        }

        public abstract void OnEntry(MethodAdviceContext context);

        public abstract void OnException(MethodAdviceContext context, Exception ex);

        public abstract void OnSuccess(MethodAdviceContext context);
    }
}
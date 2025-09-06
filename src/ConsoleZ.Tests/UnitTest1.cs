using System.Linq.Expressions;

using ConsoleZ.Core;

namespace ConsoleZ.Tests;

public class ExpressionTests
{
    [Fact]
    public void CanUseExpressionToGetActionName()
    {
        callCount = 0;
        var result =  GetMethodName(()=>HelloWorld());
        Assert.Equal("HelloWorld", result.Name);
        result.method();
        Assert.True(callCount > 0);
    }

    [Fact]
    public void CanUseExpressionToGetActionNameWithChain()
    {
        callCount = 0;
        var obj = this;

        var result =  GetMethodName(()=>obj.HelloWorld());
        Assert.Equal("HelloWorld", result.Name);
        result.method();
        Assert.True(callCount > 0);
    }

    int callCount=0;
    void HelloWorld()
    {
        callCount++;
    }

    static (string Name, Action method) GetMethodName(Expression<Action> expAct)
    {
        if (expAct.Body is MethodCallExpression methodCall)
        {
            return (methodCall.Method.Name, expAct.Compile());
        }
        throw new ArgumentException("Expression is not a method call", nameof(expAct));
    }
}

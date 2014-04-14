using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSerialization
{
    public abstract class ExpressionSerializer
    {
        public abstract LambdaExpression Deserialize(string input);
        public abstract string Serialize(LambdaExpression exp);
    }
}
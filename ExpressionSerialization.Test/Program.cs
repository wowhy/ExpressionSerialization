using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSerialization.Test
{
    public class Test 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Expression<Func<int, int, int>> exp1 = (int x, int y) => x + y;
            Expression<Func<bool, bool>> exp2 = (bool b) => !b;
            Expression<Func<Test>> exp3 = () => new Test 
            {
                Id = Guid.NewGuid(),
                Name = "test"
            };
            Expression<Func<int, bool>> exp4 = (int x) =>
                ((x >= 10) && (x <= 100)) ||
                ((x >= 110) && (x <= 200));

            var serializer = new ExpressionJsonSerializer();

            Console.WriteLine(serializer.Serialize(exp1));
            Console.WriteLine(serializer.Serialize(exp2));
            Console.WriteLine(serializer.Serialize(exp3));
            Console.WriteLine(serializer.Serialize(exp4));
        }
    }
}

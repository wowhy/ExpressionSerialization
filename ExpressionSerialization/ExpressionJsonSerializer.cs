using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionSerialization
{
    public partial class ExpressionJsonSerializer : ExpressionSerializer
    {
        private JsonTextWriter writer;
        private LambdaExpression lambda;
        private Expression root;

        public ExpressionJsonSerializer()
        {
        }

        public override LambdaExpression Deserialize(string input)
        {
            throw new NotImplementedException();
        }

        public override string Serialize(LambdaExpression exp)
        {
            using (var buffer = new StringWriter())
            {
                this.writer = new JsonTextWriter(buffer);
                this.lambda = exp;
                this.root = exp.Body;

                writer.WriteStartObject();

                this.WriteParameters();
                this.WriteBody();
                this.WriteReturn();

                writer.WriteEndObject();
                return buffer.ToString();
            }
        }

        private void WriteParameters()
        {
            writer.WritePropertyName("parameters");

            if (lambda.Parameters.Count == 0)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartArray();
            foreach (var item in lambda.Parameters)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("name");
                writer.WriteValue(item.Name);
                writer.WritePropertyName("type");
                writer.WriteValue(item.Type.FullName);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        private void WriteBody()
        {
            writer.WritePropertyName("body");
            this.WriteExpression(this.root);
        }

        private void WriteReturn()
        {
            writer.WritePropertyName("return");
            if (lambda.ReturnType == null ||
                lambda.ReturnType == typeof(void))
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(lambda.ReturnType.FullName);
            }
        }

        private void WriteExpression(Expression exp)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("exp");
            writer.WriteValue(exp.NodeType.ToString());

            switch (exp.GetType().Name)
            {
                case "SimpleBinaryExpression":
                case "BinaryExpression":
                    WriteBinaryExpression((BinaryExpression)exp);
                    break;

                case "PrimitiveParameterExpression`1":
                case "ParameterExpression":
                    WriteParameterExpression((ParameterExpression)exp);
                    break;

                case "NewExpression":
                    WriteNewExpression((NewExpression)exp);
                    break;

                case "MemberInitExpression":
                    WriteMemberInitExpression((MemberInitExpression)exp);
                    break;
            }

            writer.WritePropertyName("type");
            writer.WriteValue(exp.Type.Name);

            writer.WriteEndObject();
        }

        private void WriteMemberInitExpression(MemberInitExpression exp)
        {
            writer.WritePropertyName("new");
            writer.WriteStartObject();
            this.WriteNewExpression(exp.NewExpression);
            writer.WriteEndObject();

            writer.WritePropertyName("bindings");
            writer.WriteStartArray();
            foreach (MemberAssignment bind in exp.Bindings)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("member");
                writer.WriteValue(bind.Member.Name);

                writer.WritePropertyName("type");
                writer.WriteValue(bind.BindingType.ToString());

                writer.WritePropertyName("assignment");
                this.WriteExpression(bind.Expression);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        private void WriteNewExpression(NewExpression exp)
        {
            writer.WritePropertyName("args");
            if (exp.Arguments.Count == 0)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteStartArray();
                writer.WriteEndArray();
            }
        }

        private void WriteParameterExpression(ParameterExpression exp)
        {
            writer.WritePropertyName("name");
            writer.WriteValue(exp.Name);
        }

        private void WriteBinaryExpression(BinaryExpression exp)
        {
            writer.WritePropertyName("left");
            this.WriteExpression(exp.Left);
            
            writer.WritePropertyName("right");
            this.WriteExpression(exp.Right);
        }
    }
}

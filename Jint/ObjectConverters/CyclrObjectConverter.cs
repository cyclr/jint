using System;
using System.Collections;
using System.Collections.Generic;
using Esprima;
using Esprima.Ast;
using Jint.Native;
using Jint.Runtime.Interop;
using Jint.Runtime.Interpreter.Expressions;
using Newtonsoft.Json;

namespace Jint.ObjectConverters
{
    public class CyclrObjectConverter : IObjectConverter
    {
        public bool TryConvert(Engine engine, object value, out JsValue result)
        {
            // parse known object and list types to JintExpression
            if (value is IDictionary<string, object> or IList)
            {
                var jsonCode = JsonConvert.SerializeObject(value);
                var parser = new JavaScriptParser(jsonCode, Engine.DefaultParserOptions);
                var expression = parser.ParseExpression();
                JintExpression jintExpression;

                switch (expression)
                {
                    case ObjectExpression objectExpression:
                        jintExpression = new JintObjectExpression(objectExpression);
                        break;

                    case ArrayExpression arrayExpression:
                        jintExpression = new JintArrayExpression(arrayExpression);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                result = jintExpression.GetValue(engine._activeEvaluationContext).Value;
                return true;
            }

            result = null;
            return false;
        }
    }
}

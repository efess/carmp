using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjectLayer
{
    /// <summary>
    /// DoQueryConstraint represents a constraint on a field & value
    /// 
    /// Note: Programmer has to be smart when using this. It is *not* type safe,
    /// there are no checks on proper usage.
    /// </summary>
    public class DoQueryConstraint
    {
        public DoQueryConstraint(string pField, object pValue, QueryPredicate pPredicate)
        {
            _field = pField;
            _value = pValue;
            _predicate = pPredicate;
        }

        private QueryPredicate _predicate;
        /// <summary>
        /// Indicates type of comparison
        /// </summary>
        public QueryPredicate Predicate
        {
            get { return _predicate; }
            set { _predicate = value; }
        }
        private string _field;
        /// <summary>
        /// Field on table to compare value to
        /// </summary>
        public string Field
        {
            get { return _field; }
            set { _field = value; }
        }
        private object _value;
        /// <summary>
        /// Value to constrain field to
        /// </summary>
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }

    public class DoQueryExpression : List<DoQueryConstraint>
    {
        private const string AND = " AND ";
        public string GetWhere()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (DoQueryConstraint queryConstraint in this)
            {
                if(first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(AND);
                }

                sb.Append(queryConstraint.Field)
                    .Append(' ')
                    .Append(GetPredicateString(queryConstraint.Predicate))
                    .Append(' ')
                    .Append(GetStringVersionOfObject(queryConstraint.Value));
            }
            return sb.ToString();
        }

        private string GetStringVersionOfObject(object pObject)
        {
            switch (pObject.GetType().Name)
            {
                case "Int16":
                case "Int32":
                case "Int64":
                case "Double":
                    return pObject.ToString();
                case "List<string>":
                    return GetIncludeOrExcludeString(pObject as List<string>);
                case "String":
                    return SQLiteCommon.Quoted(pObject.ToString());
                case "DateTime":
                    return SQLiteCommon.Quoted(((DateTime)pObject).ToString("MM-dd-yyyy HH:mm:ss"));
                default:
                    return SQLiteCommon.Quoted(pObject.ToString());
            }
        }


        private string GetIncludeOrExcludeString(List<string> pList)
        {
            if (pList.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("(");

            bool first = true;
            
            foreach (object obj in pList)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(", ");
                }
                sb.Append(SQLiteCommon.Quoted(obj.ToString()));
            }

            sb.Append(")");
            return sb.ToString();
        }

        private string GetPredicateString(QueryPredicate pPredicate)
        {
            switch (pPredicate)
            {
                case QueryPredicate.NotIn:
                    return "NOT IN";
                case QueryPredicate.In:
                    return "IN";
                case QueryPredicate.Equal:
                    return "=";
                case QueryPredicate.GreaterThan:
                    return ">";
                case QueryPredicate.GreaterThanEqual:
                    return ">=";
                case QueryPredicate.LessThan:
                    return "<";
                case QueryPredicate.LessThanEqual:
                    return "<=";
                case QueryPredicate.NotEqual:
                    return "<>";
                default:
                    // should not get here.
                    return "=";
            }
        }
    }

    public enum QueryPredicate
    {
        Equal,
        NotEqual,
        LessThan,
        LessThanEqual,
        GreaterThan,
        GreaterThanEqual,
        In,
        NotIn
    }
}

using System.Linq.Expressions;
using System.Reflection;

namespace BotManager.Metadata.Helpers;

public static class ExpressionHelper
{
	public static MemberInfo GetMemberInfo<T>(Expression<Func<T, object>> expression)
	{
        ArgumentNullException.ThrowIfNull(expression);

        // Handle the expression body
        var memberExpression = GetMemberExpression(expression.Body);
        
		if (memberExpression == null)
			throw new ArgumentException(
				"Expression must be a member access expression (property or field)", 
				nameof(expression));

		return memberExpression.Member;
	}
	
	private static MemberExpression? GetMemberExpression(Expression expression)
	{
		return expression switch
		{
			MemberExpression memberExpression => memberExpression,

			UnaryExpression { NodeType: ExpressionType.Convert } unaryExpression => unaryExpression
				.Operand as MemberExpression,
			
			_ => null,
		};
	}

	public static string? GetString(this MemberInfo memberInfo)
	{
		return memberInfo.MemberType switch
		{
			MemberTypes.Field => ((FieldInfo)memberInfo).GetRawConstantValue() as string,
			MemberTypes.Property => ((PropertyInfo)memberInfo).GetRawConstantValue() as string,
			var _ => null,
		};
	}
}
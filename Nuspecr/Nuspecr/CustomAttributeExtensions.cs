namespace Nuspecr
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class CustomAttributeExtensions
    {
        public static TValue GetFirstAttributeValueOrDefault<TValue>(this IEnumerable<CustomAttribute> attributes, string attributeTypeName, Func<TValue> defaultGetter)
        {
            if (attributeTypeName == null)
            {
                throw new ArgumentNullException(nameof(attributeTypeName));
            }

            if (string.IsNullOrWhiteSpace(attributeTypeName))
            {
                throw new ArgumentException($"{nameof(attributeTypeName)} is null or empty/whitespace", nameof(attributeTypeName));
            }

            if (defaultGetter == null)
            {
                defaultGetter = () => default(TValue);
            }

            var matchingAttribute = attributes.FirstOrDefault(attr => attr.AttributeType.Name.Equals(attributeTypeName, StringComparison.OrdinalIgnoreCase));
            if (matchingAttribute == null)
            {
                return defaultGetter();
            }

            TValue result = matchingAttribute.TryGetFirstParameterValue(out result) ? result : defaultGetter();
            return result;
        }

        public static bool TryGetFirstParameterValue<TValue>(this CustomAttribute attribute, out TValue value)
        {
            if (attribute != null && attribute.HasConstructorArguments)
            {
                value = (TValue)Convert.ChangeType(attribute.ConstructorArguments[0].Value, typeof(TValue));
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }
    }
}

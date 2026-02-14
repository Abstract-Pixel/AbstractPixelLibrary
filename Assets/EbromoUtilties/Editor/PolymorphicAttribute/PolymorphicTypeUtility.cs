using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;


public static class PolymorphicTypeUtility
{

    public static List<Type> GetPropertyCompatibleTypes(SerializedProperty _property)
    {
        Type propertyType = GetPropertyTypeFromManagedReference(_property);
        TypeCache.TypeCollection potentialTypes = TypeCache.GetTypesDerivedFrom(propertyType);
        List<Type> filteredTypes = potentialTypes.Where(x => !x.IsAbstract && !x.IsInterface && !x.IsGenericType && x.IsSerializable)
                                                  .Where(x => x.GetCustomAttribute<SerializableAttribute>() != null)
                                                  .OrderBy(type => type.Name)
                                                  .ToList();
        return filteredTypes;
    }

    public static Type GetPropertyTypeFromManagedReference(SerializedProperty _property)
    {
        string[] splitPrportyParts = _property.managedReferenceFieldTypename.Split(" ");
        string assemblyName = splitPrportyParts[0];
        string typeName = splitPrportyParts[1];
        string properFormatedName = $"{typeName}, {assemblyName}";
        Type finalType = Type.GetType(properFormatedName);
        return finalType;
    }

}


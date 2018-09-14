////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/

// based on https://www.codeproject.com/Articles/318877/Comparing-the-properties-of-two-objects-via-Reflec

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Framework.Tools.Helpers
{
    ////////////////////////////////////////////////////////

    public class CompareProperties
    {
        public static bool AreObjectsPropertiesEqual(object objectA, object objectB, params string[] ignoreList)
        {
            return AreObjectsPropertiesEqual(objectA, objectB, new HashSet<object>(), ignoreList);
        }

        private static bool AreObjectsPropertiesEqual(object objectA, object objectB, HashSet<object> compared, params string[] ignoreList)
        {
            // check for circles e.g. ClassA => ICollection<ClassB> => classA
            if (compared.Contains(objectA)) // 
            {
                return true;
            }

            compared.Add(objectA);

            if (objectA != null && objectB != null)
            {
                Type objectType;

                objectType = objectA.GetType();

                foreach (PropertyInfo propertyInfo in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && !ignoreList.Contains(p.Name)))
                {
                    object valueA;
                    object valueB;

                    valueA = propertyInfo.GetValue(objectA, null);
                    valueB = propertyInfo.GetValue(objectB, null);

                    if (CanDirectlyCompare(propertyInfo.PropertyType))
                    {
                        if (!AreValuesEqual(valueA, valueB))
                        {
                            return false;
                        }
                    }
                    // if it implements IEnumerable, then scan any items
                    else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        IEnumerable<object> collectionItems1;
                        IEnumerable<object> collectionItems2;
                        int                 collectionItemsCount1;
                        int                 collectionItemsCount2;

                        if (valueA == null && valueB != null || valueA != null && valueB == null)
                        {
                            return false;
                        }
                        else if (valueA != null && valueB != null)
                        {
                            collectionItems1      = ((IEnumerable) valueA).Cast<object>();
                            collectionItems2      = ((IEnumerable) valueB).Cast<object>();
                            collectionItemsCount1 = collectionItems1.Count();
                            collectionItemsCount2 = collectionItems2.Count();

                            // check the counts to ensure they match
                            if (collectionItemsCount1 != collectionItemsCount2)
                            {
                                return false;
                            }
                            // and if they do, compare each item...
                            // this assumes both collections have the same order
                            else
                            {
                                for (int i = 0; i < collectionItemsCount1; i++)
                                {
                                    object collectionItem1;
                                    object collectionItem2;
                                    Type   collectionItemType;

                                    collectionItem1    = collectionItems1.ElementAt(i);
                                    collectionItem2    = collectionItems2.ElementAt(i);
                                    collectionItemType = collectionItem1.GetType();

                                    if (CanDirectlyCompare(collectionItemType))
                                    {
                                        if (!AreValuesEqual(collectionItem1, collectionItem2))
                                        {
                                            return false;
                                        }
                                    }
                                    else if (!AreObjectsPropertiesEqual(collectionItem1, collectionItem2, compared, ignoreList))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                    else if (propertyInfo.PropertyType.IsClass)
                    {
                        if (!AreObjectsPropertiesEqual(propertyInfo.GetValue(objectA, null), propertyInfo.GetValue(objectB, null), compared, ignoreList))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!object.Equals(objectA, objectB))
                {
                    return false;
                }
            }

            return true;
        }

        ////////////////////////////////////////////////////////

        private static bool CanDirectlyCompare(Type type)
        {
            return typeof(IComparable).IsAssignableFrom(type) || type.IsPrimitive || type.IsValueType;
        }

        ////////////////////////////////////////////////////////

        private static bool AreValuesEqual(object valueA, object valueB)
        {
            IComparable selfValueComparer;

            selfValueComparer = valueA as IComparable;

            if (valueA == null && valueB != null || valueA != null && valueB == null)
            {
                return false; // one of the values is null
            }

            if (selfValueComparer != null && selfValueComparer.CompareTo(valueB) != 0)
            {
                return false; // the comparison using IComparable failed
            }

            if (!object.Equals(valueA, valueB))
            {
                return false; // the comparison using Equals failed
            }

            return true;
        }
    }
}
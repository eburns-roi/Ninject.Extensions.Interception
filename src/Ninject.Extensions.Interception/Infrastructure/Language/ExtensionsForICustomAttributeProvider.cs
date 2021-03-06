// -------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsForICustomAttributeProvider.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Extensions.Interception.Infrastructure.Language
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides extension methods for <see cref="ICustomAttributeProvider"/>.
    /// </summary>
    internal static class ExtensionsForICustomAttributeProvider
    {
        /// <summary>
        /// Gets the first attribute of a specified type that decorates the member.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="member">The member to examine.</param>
        /// <returns>The first attribute matching the specified type.</returns>
        public static T GetOneAttribute<T>(this ICustomAttributeProvider member)
            where T : Attribute
        {
            var attributes = member.GetCustomAttributes(typeof(T), true) as T[];

            return (attributes == null) ||
                   (attributes.Length == 0)
                       ? null
                       : attributes[0];
        }

        /// <summary>
        /// Gets the first attribute of a specified type that decorates the member.
        /// </summary>
        /// <param name="member">The member to examine.</param>
        /// <param name="type">The type of attribute to search for.</param>
        /// <returns>The first attribute matching the specified type.</returns>
        public static object GetOneAttribute(this ICustomAttributeProvider member, Type type)
        {
            object[] attributes = member.GetCustomAttributes(type, true);

            return (attributes == null) ||
                   (attributes.Length == 0)
                       ? null
                       : attributes[0];
        }

        /// <summary>
        /// Gets an array of attributes matching the specified type that decorate the member.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="member">The member to examine.</param>
        /// <returns>An array of attributes matching the specified type.</returns>
        public static T[] GetAllAttributes<T>(this ICustomAttributeProvider member)
            where T : Attribute
        {
            return member.GetCustomAttributes(typeof(T), true) as T[];
        }

        /// <summary>
        /// Gets an array of attributes matching the specified type that decorate the member.
        /// </summary>
        /// <param name="member">The member to examine.</param>
        /// <param name="type">The type of attribute to search for.</param>
        /// <returns>An array of attributes matching the specified type.</returns>
        public static object[] GetAllAttributes(this ICustomAttributeProvider member, Type type)
        {
            return member.GetCustomAttributes(type, true);
        }

        /// <summary>
        /// Determines whether the member is decorated with one or more attributes of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="member">The member to examine.</param>
        /// <returns><see langword="True"/> if the member is decorated with one or more attributes of the type, otherwise <see langword="false"/>.</returns>
        public static bool HasAttribute<T>(this ICustomAttributeProvider member)
            where T : Attribute
        {
            return member.IsDefined(typeof(T), true);
        }

        /// <summary>
        /// Determines whether the member is decorated with one or more attributes of the specified type.
        /// </summary>
        /// <param name="member">The member to examine.</param>
        /// <param name="type">The type of attribute to search for.</param>
        /// <returns><see langword="True"/> if the member is decorated with one or more attributes of the type, otherwise <see langword="false"/>.</returns>
        public static bool HasAttribute(this ICustomAttributeProvider member, Type type)
        {
            return member.IsDefined(type, true);
        }

        /// <summary>
        /// Determines whether the member is decorated with an attribute that matches the one provided.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="member">The member to examine.</param>
        /// <param name="attributeToMatch">The attribute to match against.</param>
        /// <returns><see langword="True"/> if the member is decorated with a matching attribute, otherwise <see langword="false"/>.</returns>
        public static bool HasMatchingAttribute<T>(this ICustomAttributeProvider member, T attributeToMatch)
            where T : Attribute
        {
            T[] attributes = member.GetAllAttributes<T>();

            if ((attributes == null) ||
                 (attributes.Length == 0))
            {
                return false;
            }

            return attributes.Any(attribute => attribute.Match(attributeToMatch));
        }
    }
}
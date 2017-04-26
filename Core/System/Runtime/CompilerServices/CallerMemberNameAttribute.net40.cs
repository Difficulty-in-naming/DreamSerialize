using System;
using System.ComponentModel;

#if NET20 || NET30 || NET35 || NET40

namespace Theraot.Core.System.Runtime.CompilerServices
{
    /// <summary>
    ///   Allows you to obtain the method or property name of the caller to the method.
    /// </summary>
    /// <remarks>
    ///   You apply the <b>CallerMemberName</b> attribute to an optional parameter that has a default value.
    ///   You must specify an explicit default value for the optional parameter.
    ///   You can't apply this attribute to parameters that aren't specified as optional.
    /// <para/>
    ///   You can use the <b>CallerMemberName</b> attribute to avoid specifying the member name as a <b>String</b> argument to the called method.
    ///   By using this technique, you avoid the problem that <b>Rename Refactoring</b> doesn't change the <b>String</b> values.
    ///   This is especially useful for the following tasks:
    ///   <list type="bullet">
    ///     <item>
    ///       Using tracing and diagnostic routines.
    ///     </item>
    ///     <item>
    ///       Implementing the <see cref="INotifyPropertyChanged"/> interface when binding data.
    ///       This interface allows the property of an object to notify a bound control that the property has changed,
    ///       so that the control can display the updated information.
    ///       Without the CallerMemberName attribute, you must specify the property name as a literal.
    ///     </item>
    ///   </list>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerMemberNameAttribute : Attribute
    {
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public CallerMemberNameAttribute()
        {
            // Empty
            // This constructor is not redundant
            // You cannot put attributes on the default constructor
        }
    }
}

#endif
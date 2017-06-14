using System;
using JetBrains.Annotations;

namespace Raptor.Api
{
    /// <summary>
    ///     Specifies the API version of a Raptor plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [MeansImplicitUse]
    [PublicAPI]
    public sealed class ApiVersionAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ApiVersionAttribute" /> class with the specified version.
        /// </summary>
        /// <param name="version">The version, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="version" /> is <c>null</c>.</exception>
        public ApiVersionAttribute([NotNull] Version version)
        {
            ApiVersion = version ?? throw new ArgumentNullException(nameof(version));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ApiVersionAttribute" /> class with the specified versions.
        /// </summary>
        /// <param name="major">The major version of the attribute.</param>
        /// <param name="minor">The minor version of the attribute.</param>
        public ApiVersionAttribute(int major, int minor)
            : this(new Version(major, minor))
        {
        }

        /// <summary>
        ///     Gets the API version.
        /// </summary>
        [NotNull]
        public Version ApiVersion { get; }
    }
}
